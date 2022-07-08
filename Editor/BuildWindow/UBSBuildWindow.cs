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

        [SerializeField]
        Vector2 scrollPosition01;

        public static void Init(BuildCollection pData, BuildProcess pProcess, bool pBuildAndRun = false)
		{
			foreach (var p in pData.Processes) {
				p.Selected = (p == pProcess);
			}
			Init(pData, pBuildAndRun);
		}

		public static UBSBuildWindow CreateWindow()
		{
			
			var window = EditorWindow.GetWindow<UBSBuildWindow>(true, "Build", true);
			
			window.position = new Rect(50, 50, 350, 460);
			window.minSize = new Vector2(350, 460);
			window.maxSize = new Vector2(350, 460);
			return window;
		}

		public static void Init(BuildCollection pData, bool pBuildAndRun = false)
		{
			var window = CreateWindow();
			window.Run(pData, pBuildAndRun);

        }

		public void Run(BuildCollection pCollection, bool pBuildAndRun)
        {
            scrollPosition01 = Vector2.zero;
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

            if (mProcess != null && !mProcess.IsDone)
            {
                Repaint();
            }
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
				GUILayout.Label("Nothing to build", Styles.BigHint);
				return;
			}

            GUILayout.BeginArea(new Rect(25, 25, 300, 20));
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Build Processes", "BoldLabel");
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Select Build Collection"))
                    {
                        Selection.activeObject = mProcess.BuildCollection;
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(25, 50, 300, 125), EditorStyles.helpBox);
            {
                int selectedCount = 0;
                bool selected = false;
                scrollPosition01 = GUILayout.BeginScrollView(scrollPosition01);
                {
                    GUILayout.BeginVertical();
                    {
                        for (int i = 0; i < mProcess.BuildCollection.Processes.Count; i++)
                        {
                            var p = mProcess.BuildCollection.Processes[i];
                            bool odd = (i % 2) == 0;
                                
                            GUI.enabled = p.Selected;
                            UBSWindowBase.DrawBuildProcessEntry(mProcess.BuildCollection, p, odd, ref selectedCount, false, ref selected);
                            
                        }
                        GUI.enabled = true;
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndArea();

            GUI.Box(new Rect(25, 205, 300, 230  ), "", EditorStyles.helpBox);


			float fTop = 205;
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

            GUILayout.Space(5);
            
            GUI.enabled = !mProcess.IsDone;
            if (GUILayout.Button("Cancel")) {
                mProcess.Cancel();
			}

            

            GUILayout.EndArea();
		}

		void KeyValue(string pKey, object pValue)
		{
			GUI.Label(new Rect(0, 0, 140, 25), pKey, Styles.BoldKey);
			GUI.Label(new Rect(150, 0, 140, 25), pValue.ToString(), Styles.NormalValue);

		}

		void KeyProgress(string pKey, float pValue)
		{
			GUI.Label(new Rect(0, 0, 140, 25), pKey, Styles.BoldKey);
            EditorGUI.ProgressBar(new Rect(150, 5, 140, 15), pValue, Mathf.RoundToInt(pValue * 100).ToString() + "%");
		}

	}
}

