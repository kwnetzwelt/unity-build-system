using UnityEngine;
using System.Collections;
using UnityEditor;
using UBS;
namespace UBS
{
	internal class UBSEditorWindow : UBSWindowBase
	{
		#region window creation
		const int kMinWidth = 600;
		const int kMinHeight = 400;
		const int kListWidth = 250;



		public static void Init(BuildCollection pCollection)
		{
			var window = EditorWindow.GetWindow<UBSEditorWindow>("Build System", true);
			window.mData = pCollection;
			window.position = new Rect(50, 50, kMinWidth + 50 + kListWidth, kMinHeight + 50);

		}


		#endregion


		#region gui rendering
		string mSearchContent = "";
		string mStatusMessage = "Ready";
		Vector2[] mScrollPositions;
		BuildProcessEditor mEditor = new BuildProcessEditor();

		void SearchField()
		{
			GUILayout.BeginHorizontal(UBS.Styles.detailsGroup);
			{
				mSearchContent = GUILayout.TextField(mSearchContent, "SearchTextField");
				if (GUILayout.Button("", string.IsNullOrEmpty(mSearchContent) ? "SearchCancelButtonEmpty" : "SearchCancelButton"))
				{
					mSearchContent = "";
				}
			}
			GUILayout.EndHorizontal();
		}
		void OnEnable()
		{
			Initialize();
		}
		protected override void OnGUI()
		{
			base.OnGUI();

			Initialize();
			if (!mInitialized)
				return;

			GUILayout.BeginVertical();
			mScrollPositions [0] = GUILayout.BeginScrollView(mScrollPositions [0]);

			
			GUILayout.BeginHorizontal();
			//
			// selectable Build Processes
			//
			GUILayout.BeginVertical("GameViewBackground", UBS.Styles.ProcessColumn);
			SearchField();
			mScrollPositions [1] = GUILayout.BeginScrollView(mScrollPositions [1], GUILayout.ExpandWidth(true));
			bool odd = true;
			if (mData != null)
			{
				foreach (var process in mData.mProcesses)
				{
					if (process == null)
					{
						mData.mProcesses.Remove(process);
						GUIUtility.ExitGUI();
						return;
					}
					if (string.IsNullOrEmpty(mSearchContent) || process.mName.Contains(mSearchContent))
					{
						RenderSelectableBuildProcess(process, odd);
						odd = !odd;
					}
				}
			}
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
			GUILayout.BeginVertical(UBS.Styles.ToolColumn);
			{
				if (GUILayout.Button(UBS.Styles.GetToolIcon(Styles.ToolIcon.Add), UBS.Styles.ToolButton))
				{
					var el = new BuildProcess();
					Undo.RecordObject(mData, "Add Build Process");
					mData.mProcesses.Add(el);
				}
				GUI.enabled = mSelectedBuildProcess != null;
				if (GUILayout.Button(UBS.Styles.GetToolIcon(Styles.ToolIcon.Remove), UBS.Styles.ToolButton))
				{
					Undo.RecordObject(mData, "Add Build Process");
					mData.mProcesses.Remove(mSelectedBuildProcess);
				}
				if (GUILayout.Button(UBS.Styles.GetToolIcon(Styles.ToolIcon.Copy), UBS.Styles.ToolButton))
				{
					Undo.RecordObject(mData, "Duplicate Build Process");
					BuildProcess bp = new BuildProcess(mSelectedBuildProcess);
					mData.mProcesses.Add(bp);
				}
				if (GUILayout.Button(UBS.Styles.GetToolIcon(Styles.ToolIcon.Up), UBS.Styles.ToolButton))
				{
					if (mSelectedBuildProcess != null)
					{
						int idx = mData.mProcesses.IndexOf(mSelectedBuildProcess);
						if (idx > 0)
						{
							Undo.RecordObject(mData, "Sort Build Processes");
							mData.mProcesses.Remove(mSelectedBuildProcess);
							mData.mProcesses.Insert(idx - 1, mSelectedBuildProcess);
						}
					}
				}
				if (GUILayout.Button(UBS.Styles.GetToolIcon(Styles.ToolIcon.Down), UBS.Styles.ToolButton))
				{
					if (mSelectedBuildProcess != null)
					{
						int idx = mData.mProcesses.IndexOf(mSelectedBuildProcess);
						if (idx < mData.mProcesses.Count - 1)
						{
							Undo.RecordObject(mData, "Sort Build Processes");
							mData.mProcesses.Remove(mSelectedBuildProcess);
							mData.mProcesses.Insert(idx + 1, mSelectedBuildProcess);
						}
					}
				}
				GUI.enabled = true;
			}
			GUILayout.EndVertical();
			Styles.VerticalLine();
			//
			// selected Build Process
			//
			mScrollPositions [2] = GUILayout.BeginScrollView(mScrollPositions [2], UBS.Styles.buildProcessEditorBackground);

			mEditor.OnGUI(mSelectedBuildProcess, mData);

			GUILayout.EndScrollView();

			GUILayout.EndHorizontal();

			Styles.HorizontalLine();
			RenderBuildVersion();
			Styles.HorizontalLine();
			RenderStatusBar();
			
			GUILayout.EndVertical();

			GUILayout.EndScrollView();

		}

		void RenderSelectableBuildProcess(BuildProcess process, bool odd)
		{
			GUILayout.BeginHorizontal(odd ? UBS.Styles.selectableListEntryOdd : UBS.Styles.selectableListEntry);
			RenderPlatformIcon(process.mPlatform);

			bool selected = false; 

			if (mSelectedBuildProcess == process)
			{
				selected = GUILayout.Button(process.mName, Styles.selectedListEntryButton);
			} else
			{
				selected = GUILayout.Button(process.mName, odd ? Styles.selectableListEntryOdd : Styles.selectableListEntry);
			}

			if (selected)
			{
				DoSelectBuildProcess(process);
			}
			GUILayout.EndHorizontal();
		}

		void RenderPlatformIcon(BuildTarget platform)
		{
			Texture2D platformIcon = UBS.Styles.GetPlatformIcon(platform);
			var iconStyle = new GUIStyle(UBS.Styles.icon);
			iconStyle.contentOffset = new Vector2(2f, 4f);
			GUILayout.Box(platformIcon, iconStyle);
		}

		void RenderStatusBar()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(mStatusMessage, UBS.Styles.statusMessage);
			GUILayout.EndHorizontal();
		}

		void RenderBuildVersion()
		{
			GUILayout.BeginHorizontal(UBS.Styles.RightAligned);
			GUILayout.FlexibleSpace();
			GUILayout.Space(440f);
			GUILayout.Label("Build version:", UBS.Styles.statusMessage);

			int v;

			v = EditorGUILayout.IntField(mData.version.major, UBS.Styles.smallInput);
			if (v != mData.version.major)
			{
				mData.version.major = v;
				mData.SaveVersion();
			}

			v = EditorGUILayout.IntField(mData.version.minor, UBS.Styles.smallInput);
			if (v != mData.version.minor)
			{
				mData.version.minor = v;
				mData.SaveVersion();
			}

			v = EditorGUILayout.IntField(mData.version.build, UBS.Styles.smallInput);
			if (v != mData.version.build)
			{
				mData.version.build = v;
				mData.SaveVersion();
			}

			BuildVersion.BuildType type = (BuildVersion.BuildType)EditorGUILayout.EnumPopup(mData.version.type, UBS.Styles.smallInput);
			if (type != mData.version.type)
			{
				mData.version.type = type;
				mData.SaveVersion();
			}

			GUILayout.Label("Revision:", UBS.Styles.statusMessage);
			v = EditorGUILayout.IntField(mData.version.revision, UBS.Styles.smallInput);
			if (v != mData.version.revision)
			{
				mData.version.revision = v;
				mData.SaveVersion();
			}

			GUILayout.EndHorizontal();
		}
		#endregion


		#region data handling
		
		[SerializeField]
		BuildCollection
			mData;

		[System.NonSerialized]
		bool
			mInitialized = false;
		
		[System.NonSerialized]
		BuildProcess
			mSelectedBuildProcess;

		void Initialize()
		{
			if (mInitialized || mData == null)
				return;

			mScrollPositions = new Vector2[3];

			mInitialized = true;

			Undo.undoRedoPerformed += OnUndoRedoPerformed;
		}

		void OnUndoRedoPerformed()
		{
			this.Repaint();
		}

		void DoSelectBuildProcess(BuildProcess pProcess)
		{
			mSelectedBuildProcess = pProcess;
		}

		void OnDestroy()
		{
			if (mEditor != null)
				mEditor.OnDestroy();

			if (mData)
				EditorUtility.SetDirty(mData);

			AssetDatabase.SaveAssets();

			Undo.undoRedoPerformed -= OnUndoRedoPerformed;
			mData = null;
			mInitialized = false;
		}

		#endregion
	}
}