using System;
using UnityEditor;
using UnityEngine;

namespace UBS
{
	internal class UBSBuildWindow : UBSWindowBase
	{
		const float kHeight = 25;
		[NonSerialized]
		UBSProcess
			mProcess;
		[NonSerialized]
		bool
			mInit;
		[NonSerialized]
		bool
			mEmpty = false;

		public static void Init(BuildCollection pData, BuildProcess pProcess, bool pBuildAndRun = false)
		{
			foreach (var p in pData.mProcesses) {
				p.mSelected = (p == pProcess);
			}
			Init(pData, pBuildAndRun);
		}

		public static UBSBuildWindow CreateWindow()
		{
			
			var window = EditorWindow.GetWindow<UBSBuildWindow>(true, "Build", true);
			
			window.position = new Rect(50, 50, 350, 360);
			window.minSize = new Vector2(350, 360);
			window.maxSize = new Vector2(350, 360);
			return window;
		}

		public static void Init(BuildCollection pData, bool pBuildAndRun = false)
		{
			var window = CreateWindow();
			window.Run(pData, pBuildAndRun);
		}

		public void Run(BuildCollection pCollection, bool pBuildAndRun)
		{
			UBSProcess.Create(pCollection, pBuildAndRun);
		}

		void Initialize()
		{
			mProcess = UBSProcess.LoadUBSProcess();
			if (mProcess == null) {
				mEmpty = true;
				mInit = true;
			} else {
				EditorApplication.update -= OnUpdate;
				EditorApplication.update += OnUpdate;
			}
		}

		void OnUpdate()
		{
			//Debug.Log("Update");

			if (mProcess == null) {
				EditorApplication.update -= OnUpdate;
				mEmpty = true;
			}


			try {
				mProcess.MoveNext();
			} catch (Exception e) {
				Debug.LogException(e);
				EditorApplication.update -= OnUpdate;
				return;
			}

			Repaint();
			Focus();
		}

		void OnDestroy()
		{
			EditorApplication.update -= OnUpdate;
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			EditorApplication.update -= OnUpdate;
		}

		protected override void OnGUI()
		{
			base.OnGUI();

			if (!mInit) {
				Initialize();
			}

			if (mEmpty) {
				// still no process existing?
				GUILayout.Label("Nothing to build", Styles.bigHint);
				return;
			}

			GUI.Box(new Rect(25, 25, 300, 310), "", Styles.buildProcessEditorBackground);


			float fTop = 25;
			float fLeft = 30;
			GUI.BeginGroup(new Rect(fLeft, fTop, 300, kHeight));
			KeyValue("Collection:", mProcess.BuildCollection.name);
			GUI.EndGroup();

			fTop += kHeight;
			GUI.BeginGroup(new Rect(fLeft, fTop, 300, kHeight));
			KeyValue("CurrentProcess: ", mProcess.CurrentProcessName);
			GUI.EndGroup();

			fTop += kHeight;
			GUI.BeginGroup(new Rect(fLeft, fTop, 300, kHeight));
			KeyValue("CurrentState: ", mProcess.CurrentState);
			GUI.EndGroup();


			fTop += kHeight;
			GUI.BeginGroup(new Rect(fLeft, fTop, 300, kHeight));
			KeyProgress("Pre Steps Progress: ", mProcess.SubPreWalker.Progress);
			GUI.EndGroup();

			fTop += kHeight;
			GUI.BeginGroup(new Rect(fLeft, fTop, 300, kHeight));
			KeyValue("Pre Step Current: ", mProcess.SubPreWalker.Step);
			GUI.EndGroup();
			
			fTop += kHeight;
			GUI.BeginGroup(new Rect(fLeft, fTop, 300, kHeight));
			KeyProgress("Post Steps Progress: ", mProcess.SubPostWalker.Progress);
			GUI.EndGroup();
			
			fTop += kHeight;
			GUI.BeginGroup(new Rect(fLeft, fTop, 300, kHeight));
			KeyValue("Post Step Current: ", mProcess.SubPostWalker.Step);
			GUI.EndGroup();

			fTop += kHeight;
			GUI.BeginGroup(new Rect(fLeft, fTop, 300, kHeight));
			KeyProgress("Progress: ", mProcess.Progress);
			GUI.EndGroup();

			fTop += kHeight;
			GUILayout.BeginArea(new Rect(fLeft, fTop, 290, kHeight * 5));
			if (UBSProcess.BuildBehavior == UBSBuildBehavior.manual && mProcess.CurrentState == UBSState.building) {
				GUILayout.BeginVertical();
				EditorGUILayout.HelpBox("You use Unity free. Open build settings and press \"Build\" manually. ", MessageType.Info);
				EditorGUILayout.HelpBox("Output path: " + mProcess.GetCurrentProcess().mOutputPath, MessageType.Info);

				if (GUILayout.Button("Open Build Settings")) {
					System.Reflection.Assembly asm = System.Reflection.Assembly.GetAssembly(typeof(EditorWindow));
					var M = asm
						.GetType("UnityEditor.BuildPlayerWindow")
						.GetMethod("ShowBuildPlayerWindow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
					M.Invoke(null, null);
				}
				GUILayout.EndVertical();
			} else {
				if(GUILayout.Button("Cancel")) {
					mProcess.Cancel();
				}
			}
			GUILayout.EndArea();
		}

		void KeyValue(string pKey, object pValue)
		{
			GUI.Label(new Rect(0, 0, 140, 25), pKey, Styles.boldKey);
			GUI.Label(new Rect(150, 0, 140, 25), pValue.ToString(), Styles.normalValue);

		}

		void KeyProgress(string pKey, float pValue)
		{
			GUI.Label(new Rect(0, 0, 140, 25), pKey, Styles.boldKey);
			EditorGUI.ProgressBar(new Rect(150, 5, 140, 15), pValue, Mathf.RoundToInt(pValue * 100).ToString() + "%");
		}

	}
}

