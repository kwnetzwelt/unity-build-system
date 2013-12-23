using UnityEngine;
using System.Collections;
using UBS;
using System.IO;
using UnityEditor;

public class AndroidPreBuildCheck : IBuildStepProvider {

	bool mIsDone = false;

	#region IBuildStepProvider implementation

	public void BuildStepStart (BuildConfiguration pConfiguration)
	{
		UBSProcess ubs = UBSProcess.LoadUBSProcess();
		BuildProcess process = ubs.GetCurrentProcess();

		if(process.mPlatform == BuildTarget.Android)
		{
			if(!CheckAndroidPlayerSettings())
			{
				EditorApplication.ExecuteMenuItem("Edit/Project Settings/Player");
				return;
			}
		}

		CheckOutputPath(process);
	}

	public void BuildStepUpdate ()
	{
		return;
	}

	public bool IsBuildStepDone ()
	{
		return mIsDone;
	}

	#endregion

	void CancelProcess(string pMessage)
	{
		UBSProcess ubs = UBSProcess.LoadUBSProcess();
		ubs.Cancel(pMessage);
	}

	bool CheckAndroidPlayerSettings()
	{
		if(PlayerSettings.Android.keystoreName.Length > 0 && 
			PlayerSettings.Android.keystorePass.Length == 0 &&
		    PlayerSettings.Android.keyaliasName.Length > 0)
		{
			CancelProcess("Please provide a kestore password.");
			return false;
		}
		else if(PlayerSettings.Android.keyaliasName.Length > 0 &&
				PlayerSettings.Android.keyaliasPass.Length == 0)
		{
			CancelProcess("Please provide a keyalias password.");
			return false;
		}
		return true;
	}

	void CheckOutputPath(BuildProcess pProcess)
	{
		string error = "";
		

		if(pProcess.mOutputPath.Length == 0) {
			error = "Please provide an output path.";
			CancelProcess(error);
			return;
		}
		
		DirectoryInfo dir;
		if(pProcess.mPlatform == BuildTarget.Android)
			dir = new DirectoryInfo(Path.GetDirectoryName(pProcess.mOutputPath));
		else
			dir = new DirectoryInfo(pProcess.mOutputPath);
		
		try
		{
			if(dir.Exists)
				mIsDone = true;
			else
				error = "The given output path is invalid.";
		}
		catch (IOException pIO)
		{
			error = pIO.ToString();
		}
		
		if(error.Length > 0)
		{
			CancelProcess(error);
		}
	}
}
