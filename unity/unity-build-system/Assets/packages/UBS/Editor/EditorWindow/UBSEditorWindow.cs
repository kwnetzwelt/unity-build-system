using UnityEngine;
using System.Collections;
using UnityEditor;
using UBS;

public class UBSEditorWindow : EditorWindow {
	#region window creation
	const int kMinWidth = 600;
	const int kMinHeight = 400;
	const int kListWidth = 250;



	public static void Init (BuildCollection pCollection) 
	{
		var window = EditorWindow.GetWindow<UBSEditorWindow>("Build System",true);
		window.mData = pCollection;
		window.position = new Rect(50,50, kMinWidth + 50 + kListWidth,kMinHeight + 50);

	}


	#endregion


	#region gui rendering
	string mSearchContent = "";
	string mStatusMessage = "Ready";
	Vector2[] mScrollPositions;
	BuildProcessEditor mEditor = new BuildProcessEditor();

	void SearchField()
	{
		GUILayout.BeginHorizontal( UBS.Styles.detailsGroup );
		{
			mSearchContent = GUILayout.TextField(mSearchContent,"SearchTextField");
			if(GUILayout.Button("", string.IsNullOrEmpty(mSearchContent)? "SearchCancelButtonEmpty" : "SearchCancelButton"))
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
	void OnGUI()
	{
		Initialize();
		if(!mInitialized)
			return;

		GUILayout.BeginVertical();
		mScrollPositions[0] = GUILayout.BeginScrollView(mScrollPositions[0]);

		
		GUILayout.BeginHorizontal();
		//
		// selectable Build Processes
		//
		GUILayout.BeginVertical("GameViewBackground",GUILayout.MaxWidth(kListWidth));
		SearchField();
		mScrollPositions[1] = GUILayout.BeginScrollView(mScrollPositions[1], GUILayout.ExpandWidth(true));
		bool odd = true;
		if(mData != null)
		{
			foreach(var process in mData.mProcesses)
			{
				if(process == null)
				{
					mData.mProcesses.Remove(process);
					GUIUtility.ExitGUI();
					return;
				}
				if( string.IsNullOrEmpty(mSearchContent) || process.mName.StartsWith(mSearchContent))
				{
					RenderSelectableBuildProcess(process,odd);
					odd = !odd;
				}
			}
		}
		GUILayout.EndScrollView();
		GUILayout.EndVertical();
		GUILayout.BeginVertical(GUILayout.Width(32));
		{
			if(GUILayout.Button(new GUIContent("+","Add"),UBS.Styles.toolButton))
			{
				var el = new BuildProcess();
				Undo.RecordObject(mData, "Add Build Process");
				mData.mProcesses.Add(el);
			}
			GUI.enabled = mSelectedBuildProcess != null;
			if(GUILayout.Button(new GUIContent("-","Remove"),UBS.Styles.toolButton))
			{
				Undo.RecordObject(mData, "Add Build Process");
				mData.mProcesses.Remove(mSelectedBuildProcess);
			}
			if(GUILayout.Button(new GUIContent("#","Duplicate"),UBS.Styles.toolButton))
			{
				Undo.RecordObject(mData, "Duplicate Build Process");
				BuildProcess bp = new BuildProcess(mSelectedBuildProcess);
				mData.mProcesses.Add(bp);
			}

			GUI.enabled = true;
		}
		GUILayout.EndVertical();
		Styles.VerticalLine();
		//
		// selected Build Process
		//
		mScrollPositions[2] = GUILayout.BeginScrollView(mScrollPositions[2]);

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

	void RenderSelectableBuildProcess (BuildProcess pProcess, bool pOdd)
	{
		bool selected = false; 

		if(mSelectedBuildProcess == pProcess)
		{
			selected = GUILayout.Button(pProcess.mName, Styles.selectedListEntry);
		}else
		{
			selected = GUILayout.Button(pProcess.mName, pOdd ? Styles.selectableListEntryOdd : Styles.selectableListEntry);
		}

		if(selected)
		{
			DoSelectBuildProcess( pProcess );
		}

	}

	void RenderStatusBar()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label(mStatusMessage, UBS.Styles.statusMessage);
		GUILayout.EndHorizontal();
	}

	void RenderBuildVersion()
	{
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("Build version:");

		int v;

		v = EditorGUILayout.IntField( mData.version.major, GUILayout.Width(50));
		if(v != mData.version.major)
		{
			mData.version.major = v;
			mData.SaveVersion();
		}

		v = EditorGUILayout.IntField( mData.version.minor, GUILayout.Width(50));
		if(v != mData.version.minor)
		{
			mData.version.minor = v;
			mData.SaveVersion();
		}

		v = EditorGUILayout.IntField( mData.version.build, GUILayout.Width(50));
		if(v != mData.version.build)
		{
			mData.version.build = v;
			mData.SaveVersion();
		}

		BuildVersion.BuildType type = (BuildVersion.BuildType)EditorGUILayout.EnumPopup( mData.version.type, GUILayout.Width(50));
		if(type != mData.version.type)
		{
			mData.version.type = type;
			mData.SaveVersion();
		}

		GUILayout.Label("Revision:");
		v = EditorGUILayout.IntField( mData.version.revision, GUILayout.Width(80));
		if(v != mData.version.revision)
		{
			mData.version.revision = v;
			mData.SaveVersion();
		}

		GUILayout.EndHorizontal();
	}
	#endregion


	#region data handling
	
	[SerializeField]
	BuildCollection mData;

	[System.NonSerialized]
	bool mInitialized = false;
	
	[System.NonSerialized]
	BuildProcess mSelectedBuildProcess;

	void Initialize()
	{
		if(mInitialized || mData == null)
			return;

		mScrollPositions = new Vector2[3];

		mInitialized = true;

		Undo.undoRedoPerformed += OnUndoRedoPerformed;
	}

	void OnUndoRedoPerformed()
	{
		this.Repaint();
	}

	void DoSelectBuildProcess (BuildProcess pProcess)
	{
		mSelectedBuildProcess = pProcess;
	}

	void OnDestroy()
	{
		if(mEditor != null)
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
