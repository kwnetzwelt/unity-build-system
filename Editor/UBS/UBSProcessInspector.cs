using UnityEditor;
using UnityEngine;

namespace UBS
{
	[CustomEditor(typeof(UBSProcess))]
	public class UBSProcessInspector : UnityEditor.Editor
	{
		UBSProcess process;

		public override void OnInspectorGUI ()
		{
			process = target as UBSProcess;

			GUILayout.Label("This asset contains progress information about the last run Build Collection. " +
				"Build Processes can be continued, if they did not finish and have not been aborted. ", 
			                EditorStyles.wordWrappedMiniLabel);
			GUILayout.Label("This is an auto generated asset. It should be excluded from version control and " +
				"will be generated each time a build collection is built. ",
			                EditorStyles.wordWrappedMiniLabel);

			GUILayout.Label("This asset was responsible for building the following build collection. ", 
			                EditorStyles.wordWrappedMiniLabel);

			EditorGUILayout.ObjectField("Build Collection",	process.BuildCollection, typeof(BuildCollection), false);


			GUILayout.Space(5);
			GUILayout.BeginHorizontal();
			if(GUILayout.Button("Select Build Collection"))
			{
				Selection.activeObject = process.BuildCollection;
			}

			GUI.enabled = process.Progress < 1 && 
				(process.CurrentState != UBSState.done && process.CurrentState != UBSState.aborted);

			if(GUILayout.Button("Continue Build"))
			{
				var window = UBSBuildWindow.CreateWindow();
				window.Focus();
			}
			GUI.enabled = true;

			GUILayout.EndHorizontal();
		}
	}
}

