using System;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace UBS
{
	[CustomEditor(typeof(BuildCollection))]
	public class UBSAssetInspector : UnityEditor.Editor
	{
		public const string DefaultBuildCollectionAssetName = "/New Build Collection.asset";

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

			if(Application.platform == RuntimePlatform.WindowsEditor)
				GUILayout.Label("This asset can be accessed by hitting WIN+F11. ",
	                            EditorStyles.wordWrappedMiniLabel);
			else if(Application.platform == RuntimePlatform.OSXEditor)
				GUILayout.Label("This asset can be accessed by hitting CMD+F11. ",
					EditorStyles.wordWrappedMiniLabel);
			else
				GUILayout.Label("This asset can be accessed by hitting SUPER+F11. ",
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
			
			serializedObject.Update();
			var logtypes= serializedObject.FindPropertyByAutoPropertyName("LogTypes");
			EditorGUILayout.PropertyField(logtypes, new GUIContent("Logging Configuration"), true);
			serializedObject.ApplyModifiedProperties();

			data.cleanBuild = EditorGUILayout.Toggle("Run clean builds", data.cleanBuild);
			Tooltip("If set to true adds CleanBuildCache to the BuildOptions when calling the buildpipeline. ");
			
			
			GUILayout.BeginHorizontal();
			{
				if (GUILayout.Button("Edit"))
				{
					UBSEditorWindow.Init(data);
				}
				GUILayout.Space(5);
				GUI.enabled = selectedCount >= 1;

				if (GUILayout.Button($"Build selected ({selectedCount})"))
				{
					UBSBuildWindow.Create(data);
				}
				GUILayout.Space(5);

				GUI.enabled = selectedCount == 1;
				if (GUILayout.Button("Build and run"))
				{
					UBSBuildWindow.Create(data, true);
				}
				GUI.enabled = true;

				if (GUILayout.Button("?", GUILayout.Width(32)))
				{
					EditorUtility.OpenWithDefaultApp("https://github.com/kwnetzwelt/unity-build-system");
				}
			}
			GUILayout.EndHorizontal(); 
			
			
		}

		private void Tooltip(string text, MouseCursor cursor = MouseCursor.Arrow)
		{
			Rect rect = GUILayoutUtility.GetLastRect();
			EditorGUIUtility.AddCursorRect(rect, cursor);
			GUI.Label(rect, new GUIContent("", text));
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
				path = path + DefaultBuildCollectionAssetName;
			else
				path = path.Substring(0, path.Length - di.Name.Length - 1) + DefaultBuildCollectionAssetName;

			path = AssetDatabase.GenerateUniqueAssetPath(path);

			AssetDatabase.CreateAsset(asset, path);
			AssetDatabase.SaveAssets();
			Selection.activeObject = asset;
		}

	}

}