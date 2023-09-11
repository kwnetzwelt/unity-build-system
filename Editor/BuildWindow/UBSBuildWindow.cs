using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace UBS
{
	public class UBSBuildWindow : UBSWindowBase
	{
		private const float Width = 500;
		private const float Margin = 25; 
		private const float Height = 25;
		
		[NonSerialized]
		UBSProcess _process;
		
		[NonSerialized]
		bool _init;
		
		[NonSerialized]
		bool _empty = false;

        [SerializeField]
        Vector2 _scrollPosition01;

        private float _elementWidth;
        private float _width;
        private float _buildProcessListHeight;
        public bool IsDone => _process != null && _process.IsDone;

        public static void Create(BuildCollection pData, BuildProcess pProcess, bool pBuildAndRun = false)
		{
			foreach (var p in pData.Processes) {
				p.Selected = (p == pProcess);
			}
			Create(pData, pBuildAndRun);
		}

		public static UBSBuildWindow CreateWindow()
		{
			
			var window = EditorWindow.GetWindow<UBSBuildWindow>(true, "Build", true);
			
			window.position = new Rect(50, 50, Width + 2*Margin, 460);
			window.minSize = new Vector2(Width + 2*Margin, 460);
			return window;
		}

		public static UBSBuildWindow Create(BuildCollection pData, bool pBuildAndRun = false)
		{
			var window = CreateWindow();
			window.Run(pData, pBuildAndRun);
			return window;
		}

		public void Run(BuildCollection collection, bool buildAndRun)
        {
            _scrollPosition01 = Vector2.zero;
            UBSProcess.CreateFromCollection(collection, buildAndRun);
		}

		public void Initialize()
		{
			_process = UBSProcess.LoadUBSProcess();
			if (_process == null) {
				_empty = true;
				_init = true;
			} else if(!_process.IsDone) {
				EditorApplication.update -= OnUpdate;
				EditorApplication.update += OnUpdate;
			}
		}

		public void OnUpdate()
		{
			//Debug.Log("Update");

			if (_process == null) {
				EditorApplication.update -= OnUpdate;
				_empty = true;
			}

			try {
				_process.MoveNext();
			} catch (Exception e) {
				Debug.LogException(e);
				EditorApplication.update -= OnUpdate;
				return;
			}

            if (_process != null && !_process.IsDone)
            {
                Repaint();
            }


            if (_process.IsDone)
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

			if (!_init) {
				Initialize();
			}
			_width = position.width;
			_elementWidth = _width - 2 * Margin;
			_buildProcessListHeight = position.height - 335;
			if (_empty) {
				// still no _process existing?
				GUILayout.Label("Nothing to build", Styles.BigHint);
				return;
			}

            GUILayout.BeginArea(new Rect(Margin, Margin, _elementWidth, 20));
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Build Processes", "BoldLabel");
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Select Build Collection"))
                    {
                        Selection.activeObject = _process.BuildCollection;
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(Margin, 50, _elementWidth, _buildProcessListHeight), EditorStyles.helpBox);
            {
                int selectedCount = 0;
                bool selected = false;
                _scrollPosition01 = GUILayout.BeginScrollView(_scrollPosition01);
                {
                    GUILayout.BeginVertical();
                    {
                        for (int i = 0; i < _process.BuildCollection.Processes.Count; i++)
                        {
                            var p = _process.BuildCollection.Processes[i];
                            bool odd = (i % 2) == 0;
                                
                            GUI.enabled = p.Selected;
                            UBSWindowBase.DrawBuildProcessEntry(_process.BuildCollection, p, odd, ref selectedCount, false, ref selected);
                            
                        }
                        GUI.enabled = true;
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndArea();

			float fTop = 85 + _buildProcessListHeight;
            GUI.Box(new Rect(Margin, fTop , _elementWidth, 230  ), "", EditorStyles.helpBox);


			float fLeft = 30;
			GUI.BeginGroup(new Rect(fLeft, fTop, _elementWidth, Height));
			KeyValue("Collection:", _process.BuildCollection.name);
			GUI.EndGroup();

			fTop += Height;
			GUI.BeginGroup(new Rect(fLeft, fTop, _elementWidth, Height));
			KeyValue("CurrentProcess: ", _process.CurrentProcessName);
			GUI.EndGroup();

			fTop += Height;
			GUI.BeginGroup(new Rect(fLeft, fTop, _elementWidth, Height));
			KeyValue("CurrentState: ", _process.CurrentState);
			GUI.EndGroup();


			fTop += Height;
			GUI.BeginGroup(new Rect(fLeft, fTop, _elementWidth, Height));
			KeyProgress("Pre Steps Progress: ", _process.SubPreWalker);
			GUI.EndGroup();

			fTop += Height;
			GUI.BeginGroup(new Rect(fLeft, fTop, _elementWidth, Height));
			KeyValue("Pre Step Current: ", _process.SubPreWalker.CurrentStep);
			GUI.EndGroup();
			
			fTop += Height;
			GUI.BeginGroup(new Rect(fLeft, fTop, _elementWidth, Height));
			KeyProgress("Post Steps Progress: ", _process.SubPostWalker);
			GUI.EndGroup();
			
			fTop += Height;
			GUI.BeginGroup(new Rect(fLeft, fTop, _elementWidth, Height));
			KeyValue("Post Step Current: ", _process.SubPostWalker.CurrentStep);
			GUI.EndGroup();

			fTop += Height;
			GUI.BeginGroup(new Rect(fLeft, fTop, _elementWidth, Height));
			KeyProgress("Progress: ", _process.Progress);
			GUI.EndGroup();

			fTop += Height;
			GUILayout.BeginArea(new Rect(fLeft, fTop, _elementWidth-10, Height * 5));

            GUILayout.Space(5);
            
            GUI.enabled = !_process.IsDone;
            if (GUILayout.Button("Cancel")) {
                _process.Cancel();
			}

            

            GUILayout.EndArea();

            if (_process.IsDone)
            {
	            Close();
            }
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

