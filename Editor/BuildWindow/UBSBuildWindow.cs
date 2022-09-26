using System;
using UnityEditor;
using UnityEngine;

namespace UBS
{
	internal class UBSBuildWindow : UBSWindowBase
	{
		private const float kWdith = 500;
		private const float kMargin = 25; 
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

        private float _elementWidth;
        private float _width;
        private float _buildProcessListHeight;

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
			
			window.position = new Rect(50, 50, kWdith + 2*kMargin, 460);
			window.minSize = new Vector2(kWdith + 2*kMargin, 460);
			//window.maxSize = new Vector2(550, 460);
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
			} else if(!mProcess.IsDone) {
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


            if (mProcess.IsDone)
	            EditorApplication.update -= OnUpdate;
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
			_width = position.width;
			_elementWidth = _width - 2 * kMargin;
			_buildProcessListHeight = position.height - 335;
			if (mEmpty) {
				// still no process existing?
				GUILayout.Label("Nothing to build", Styles.BigHint);
				return;
			}

            GUILayout.BeginArea(new Rect(kMargin, kMargin, _elementWidth, 20));
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

            GUILayout.BeginArea(new Rect(kMargin, 50, _elementWidth, _buildProcessListHeight), EditorStyles.helpBox);
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

			float fTop = 85 + _buildProcessListHeight;
            GUI.Box(new Rect(kMargin, fTop , _elementWidth, 230  ), "", EditorStyles.helpBox);


			float fLeft = 30;
			GUI.BeginGroup(new Rect(fLeft, fTop, _elementWidth, kHeight));
			KeyValue("Collection:", mProcess.BuildCollection.name);
			GUI.EndGroup();

			fTop += kHeight;
			GUI.BeginGroup(new Rect(fLeft, fTop, _elementWidth, kHeight));
			KeyValue("CurrentProcess: ", mProcess.CurrentProcessName);
			GUI.EndGroup();

			fTop += kHeight;
			GUI.BeginGroup(new Rect(fLeft, fTop, _elementWidth, kHeight));
			KeyValue("CurrentState: ", mProcess.CurrentState);
			GUI.EndGroup();


			fTop += kHeight;
			GUI.BeginGroup(new Rect(fLeft, fTop, _elementWidth, kHeight));
			KeyProgress("Pre Steps Progress: ", mProcess.SubPreWalker);
			GUI.EndGroup();

			fTop += kHeight;
			GUI.BeginGroup(new Rect(fLeft, fTop, _elementWidth, kHeight));
			KeyValue("Pre Step Current: ", mProcess.SubPreWalker.CurrentStep);
			GUI.EndGroup();
			
			fTop += kHeight;
			GUI.BeginGroup(new Rect(fLeft, fTop, _elementWidth, kHeight));
			KeyProgress("Post Steps Progress: ", mProcess.SubPostWalker);
			GUI.EndGroup();
			
			fTop += kHeight;
			GUI.BeginGroup(new Rect(fLeft, fTop, _elementWidth, kHeight));
			KeyValue("Post Step Current: ", mProcess.SubPostWalker.CurrentStep);
			GUI.EndGroup();

			fTop += kHeight;
			GUI.BeginGroup(new Rect(fLeft, fTop, _elementWidth, kHeight));
			KeyProgress("Progress: ", mProcess.Progress);
			GUI.EndGroup();

			fTop += kHeight;
			GUILayout.BeginArea(new Rect(fLeft, fTop, _elementWidth-10, kHeight * 5));

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
			GUI.Label(new Rect(150, 0, _elementWidth-160, 25), pValue.ToString(), Styles.NormalValue);

		}

		void KeyProgress(string pKey, float pValue)
		{
			GUI.Label(new Rect(0, 0, 140, 25), pKey, Styles.BoldKey);
            EditorGUI.ProgressBar(new Rect(150, 5, _elementWidth-160, 15), pValue, Mathf.RoundToInt(pValue * 100).ToString() + "%");
		}

		void KeyProgress(string pKey, UBSStepListWalker walker)
		{
			GUI.Label(new Rect(0, 0, 140, 25), pKey, Styles.BoldKey);
			EditorGUI.ProgressBar(new Rect(150, 5, _elementWidth-160, 15), walker.Progress, $"{walker.Index} / {walker.Count}");
		}
	}
}

