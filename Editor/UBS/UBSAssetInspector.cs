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

			GUILayout.Label("With a Build Collection you can have multiple Build Processes in one list. " +
			                "They can target different platforms and each have their own pre- and post-steps. ", 
			                EditorStyles.wordWrappedMiniLabel);
			GUILayout.Label("You can either run one Build Process manually or run a set of selected " +
				"Build Processes at once. ", 
			                EditorStyles.wordWrappedMiniLabel);

            GUILayout.Label("This asset can be accessed by hitting CTRL+SHIFT+C. ",
                            EditorStyles.wordWrappedMiniLabel);

            if (data.Processes.Count == 0)
			{
				GUI.color = Color.yellow;
				GUILayout.Label("You should start by adding a Build Process. Hit \"Edit\" to do so. ", 
				                EditorStyles.wordWrappedLabel);
				GUI.color = Color.white;
			}

			GUILayout.Label("Build Processes", "BoldLabel");

			GUILayout.BeginVertical("HelpBox", GUILayout.MinHeight(40), GUILayout.ExpandWidth(true));
			{
				if (data.Processes.Count == 0) 
					GUILayout.Label("None", UBS.Styles.BigHint);
				bool odd = false;
                bool selected = false;
				foreach (var e in data.Processes)
				{
					if (e == null)
						break;
				    UBSWindowBase.DrawBuildProcessEntry(data, e, odd, ref selectedCount, true, ref selected);
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
					EditorUtility.OpenWithDefaultApp("https://github.com/kwnetzwelt/unity-build-system");
				}
			}
			GUILayout.EndHorizontal(); 
		}

        
		[MenuItem("Assets/Create/UBS Build Collection")]
		public static void CreateBuildCollectionMenuCommand()
		{
			var asset = BuildCollection.CreateInstance<BuildCollection>();
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

	}

}