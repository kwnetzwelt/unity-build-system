using UnityEngine;
using System.Collections;
using UnityEditor;
using UBS;

public class UBSEditorWindow : EditorWindow {
	#region window creation
	const int kMinWidth = 600;
	const int kMinHeight = 400;
	const int kListWidth = 200;

	[MenuItem ("Window/Build System")]
	public static void Init () 
	{
		var window = EditorWindow.GetWindow<UBSEditorWindow>("Build System",true);
		window.position = new Rect(50,50, kMinWidth + 50 + kListWidth,kMinHeight + 50);

	}


	#endregion



	#region gui rendering
	string mStatusMessage = "Ready";
	Vector2[] mScrollPositions;
	BuildProcessEditor mEditor = new BuildProcessEditor();
	void OnGUI()
	{
		Initialize();

		GUILayout.BeginVertical();
		mScrollPositions[0] = GUILayout.BeginScrollView(mScrollPositions[0]);

		
		GUILayout.BeginHorizontal();
		//
		// selectable Build Processes
		//

		mScrollPositions[1] = GUILayout.BeginScrollView(mScrollPositions[1],GUILayout.MaxWidth(kListWidth),GUILayout.MinWidth(kListWidth));

		bool odd = true;
		if(mData != null)
		{
			foreach(var process in mData.mProcesses)
			{
				RenderSelectableBuildProcess(process,odd);
				odd = !odd;
			}
		}
		GUILayout.EndScrollView();

		Styles.VerticalLine();
		//
		// selected Build Process
		//
		mScrollPositions[2] = GUILayout.BeginScrollView(mScrollPositions[2]);

		mEditor.OnGUI(mSelectedBuildProcess);

		GUILayout.EndScrollView();

		GUILayout.EndHorizontal();

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
	#endregion


	#region data handling
	
	[System.NonSerialized]
	BuildCollection mData;
	
	[System.NonSerialized]
	bool mInitialized = false;
	
	[System.NonSerialized]
	BuildProcess mSelectedBuildProcess;

	void Initialize()
	{
		if(mInitialized)
			return;

		mScrollPositions = new Vector2[3];

		mData = new BuildCollection();

		BuildProcess p = new BuildProcess();
		p.mName = "iOS Debug";
		
		mData.mProcesses.Add(p);

		p = new BuildProcess();
		p.mName = "iOS Release";
		mData.mProcesses.Add(p);

		mInitialized = true;

	}

	void DoSelectBuildProcess (BuildProcess pProcess)
	{
		mSelectedBuildProcess = pProcess;
	}

	void OnDestroy()
	{
		mData = null;
		mInitialized = false;
	}

	#endregion
}
