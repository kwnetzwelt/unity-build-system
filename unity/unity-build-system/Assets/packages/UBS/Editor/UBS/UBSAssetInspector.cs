using System;
using UnityEditor;
using UnityEngine;
using UBS;
using System.IO;

namespace UBS
{
	[CustomEditor(typeof(BuildCollection))]
	public class UBSAssetInspector : Editor
	{

		public override void OnInspectorGUI()
		{
			var data = target as BuildCollection;
			
			int selectedCount = 0;

			GUILayout.Label("Build Processes", "BoldLabel");

			GUILayout.BeginVertical("HelpBox", GUILayout.MinHeight(40));

			{
				if (data.mProcesses.Count == 0) 
					GUILayout.Label("None", UBS.Styles.bigHint);
				bool odd = false;
				foreach (var e in data.mProcesses)
				{
					if (e == null)
						break;
					GUILayout.BeginHorizontal(odd ? UBS.Styles.selectableListEntryOdd : UBS.Styles.selectableListEntry);
					{
						Texture2D platformIcon = GetPlatformIcon(e.mPlatform);
						GUILayout.Box(platformIcon, UBS.Styles.icon);
						GUILayout.Label(e.mName, odd ? UBS.Styles.selectableListEntryTextOdd : UBS.Styles.selectableListEntryText);
						GUILayout.FlexibleSpace();
						var sel = GUILayout.Toggle(e.mSelected, "");
						if (sel != e.mSelected)
						{
							e.mSelected = sel;
							EditorUtility.SetDirty(data);
						}
						selectedCount += e.mSelected ? 1 : 0;
					}
					GUILayout.EndHorizontal();
					if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
					{
						Rect r = GUILayoutUtility.GetLastRect();
						if (r.Contains(Event.current.mousePosition))
						{
							GenericMenu menu = new GenericMenu();
							menu.AddItem(new GUIContent(e.mName), false, null);
							menu.AddSeparator("");
							menu.AddItem(new GUIContent("Open target folder"), false, () => {

								DirectoryInfo di = new DirectoryInfo(UBS.Helpers.GetAbsolutePathRelativeToProject(e.mOutputPath));

								string path;
								if ((di.Attributes & FileAttributes.Directory) != 0)
									path = di.FullName;
								else
									path = di.Parent.FullName;

								OpenInFileBrowser(path);
							});
							menu.AddSeparator("");
							menu.AddItem(new GUIContent("Build and run"), false, BuildAndRun, e);
							menu.AddItem(new GUIContent("Build"), false, Build, e);

							menu.ShowAsContext();
						}

					}
					odd = !odd;
				}
			}
			GUILayout.EndVertical();

			GUILayout.BeginHorizontal();
			{
				if (GUILayout.Button("Edit"))
				{
					UBSEditorWindow.Init(data);
				}
				GUILayout.Space(5);
				GUI.enabled = selectedCount >= 1;

				if (GUILayout.Button("Run selected builds"))
				{
					UBSBuildWindow.Init(data);
				}
				GUILayout.Space(5);

				GUI.enabled = selectedCount == 1;
				if (GUILayout.Button("Build and run"))
				{
					UBSBuildWindow.Init(data, true);
				}
				GUI.enabled = true;

				if (GUILayout.Button("?", GUILayout.Width(32)))
				{
					EditorUtility.OpenWithDefaultApp("http://kwnetzwelt.net/unity-build-system");
				}
			}
			GUILayout.EndHorizontal(); 
		}

		Texture2D GetPlatformIcon(BuildTarget mPlatform)
		{
			switch (mPlatform)
			{
#if !UNITY_5
				case BuildTarget.iPhone:
					return UBS.Styles.icoIOS;
#else
			case BuildTarget.iOS:
				return UBS.Styles.icoIOS;
#endif
				case BuildTarget.Android:
					return UBS.Styles.icoAndroid;
				case BuildTarget.StandaloneWindows:
					return UBS.Styles.icoWindows;
				case BuildTarget.StandaloneWindows64:
					return UBS.Styles.icoWindows;
				default:
					return new Texture2D(0, 0);
			}
		}

		void Build(object pProcess)
		{
			var data = target as BuildCollection;
			UBSBuildWindow.Init(data, pProcess as BuildProcess, false);
		}
		void BuildAndRun(object pProcess)
		{
			var data = target as BuildCollection;
			UBSBuildWindow.Init(data, pProcess as BuildProcess, true);
		}

		[MenuItem("Assets/Create/UBS Build Collection")]
		public static void CreateBuildCollectionMenuCommand()
		{
			var asset = new BuildCollection();
			asset.hideFlags = HideFlags.None;
			var path = AssetDatabase.GetAssetPath(Selection.activeObject);

			if (String.IsNullOrEmpty(path))
			{
				path = "Assets";
			}

			var di = new FileInfo(path);
			if ((di.Attributes & FileAttributes.Directory) != 0)
				path = path + "/New Build Collection.asset";
			else
				path = path.Substring(0, path.Length - di.Name.Length - 1) + "/New Build Collection.asset";

			path = AssetDatabase.GenerateUniqueAssetPath(path);

			AssetDatabase.CreateAsset(asset, path);
			AssetDatabase.SaveAssets();
			Selection.activeObject = asset;
		}



		
		#region open folder
		#if UNITY_EDITOR

		//
		// source: http://answers.unity3d.com/questions/43422/how-to-implement-show-in-explorer.html
		//

		static void OpenInMacFileBrowser(string path)
		{
			bool openInsidesOfFolder = false;
			
			// try mac
			string macPath = path.Replace("\\", "/"); // mac finder doesn't like backward slashes
			
			if (Directory.Exists(macPath))
			{ // if path requested is a folder, automatically open insides of that folder
				openInsidesOfFolder = true;
			}
			
			//Debug.Log("macPath: " + macPath);
			//Debug.Log("openInsidesOfFolder: " + openInsidesOfFolder);
			
			if (!macPath.StartsWith("\""))
			{
				macPath = "\"" + macPath;
			}
			if (!macPath.EndsWith("\""))
			{
				macPath = macPath + "\"";
			}
			string arguments = (openInsidesOfFolder ? "" : "-R ") + macPath;
			//Debug.Log("arguments: " + arguments);
			try
			{
				System.Diagnostics.Process.Start("open", arguments);
			} catch (System.ComponentModel.Win32Exception e)
			{
				// tried to open mac finder in windows
				// just silently skip error
				// we currently have no platform define for the current OS we are in, so we resort to this
				e.HelpLink = ""; // do anything with this variable to silence warning about not using it
			}
		}
		
		static void OpenInWinFileBrowser(string path)
		{
			bool openInsidesOfFolder = false;
			
			// try windows
			string winPath = path.Replace("/", "\\"); // windows explorer doesn't like forward slashes
			
			if (Directory.Exists(winPath))
			{ // if path requested is a folder, automatically open insides of that folder
				openInsidesOfFolder = true;
			}
			try
			{
				System.Diagnostics.Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + winPath);
			} catch (System.ComponentModel.Win32Exception e)
			{
				// tried to open win explorer in mac
				// just silently skip error
				// we currently have no platform define for the current OS we are in, so we resort to this
				e.HelpLink = ""; // do anything with this variable to silence warning about not using it
			}
		}
		
		public static void OpenInFileBrowser(string path)
		{
			OpenInWinFileBrowser(path);
			OpenInMacFileBrowser(path);
		}
		#endif
		#endregion

	}

}