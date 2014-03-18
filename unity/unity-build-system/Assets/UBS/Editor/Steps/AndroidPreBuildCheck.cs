using UnityEngine;
using System.Collections;
using UBS;
using System.IO;
using UnityEditor;

[BuildStepPlatformFilter(BuildTarget.Android)]
[BuildStepTypeFilter(EBuildStepType.PreBuildStep)]
[BuildStepDescriptionAttribute("Checks for valid platform, valid output path and valid keystore settings. ")]
public class AndroidPreBuildCheck : IBuildStepProvider {

	bool mIsDone = false;
	BuildConfiguration mConfig;
	#region IBuildStepProvider implementation

	public void BuildStepStart (BuildConfiguration pConfiguration)
	{
		mConfig = pConfiguration;
		BuildProcess process = mConfig.GetCurrentBuildProcess();

		if(process.mPlatform == BuildTarget.Android)
		{
			if(!CheckAndroidPlayerSettings())
			{
				EditorApplication.ExecuteMenuItem("Edit/Project Settings/Player");
				return;
			}
		}

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



	bool CheckAndroidPlayerSettings()
	{
		if(PlayerSettings.Android.keystoreName.Length > 0 && 
			PlayerSettings.Android.keystorePass.Length == 0 &&
		    PlayerSettings.Android.keyaliasName.Length > 0)
		{
			mConfig.Cancel("Please provide a kestore password.");
			return false;
		}
		else if(PlayerSettings.Android.keyaliasName.Length > 0 &&
				PlayerSettings.Android.keyaliasPass.Length == 0)
		{
			mConfig.Cancel("Please provide a keyalias password.");
			return false;
		}
		return true;
	}


}
