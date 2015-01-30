using UnityEngine;
using UnityEditor;
using System.Collections;
using UBS;

#if !UNITY_5
[BuildStepPlatformFilter(BuildTarget.MetroPlayer)]
[BuildStepDescriptionAttribute("Sets the build types of Windows Store apps. This is usually used for commandline build processes. Values: VisualStudioCpp, VisualStudioCSharp")]
#else
[BuildStepPlatformFilter(BuildTarget.WSAPlayer)]
[System.Obsolete("This option is deprecated and can't be used anymore!")]
#endif
public class SetBuildType : IBuildStepProvider 
{
	#region IBuildStepProvider implementation

	public void BuildStepStart (BuildConfiguration pConfiguration)
	{
#if !UNITY_5
		EditorUserBuildSettings.metroBuildType = (MetroBuildType)System.Enum.Parse(typeof(MetroBuildType), pConfiguration.Params);

		if(UBSProcess.LoadUBSProcess().IsInBatchMode)
		{
			Debug.Log("Set build type to: " + EditorUserBuildSettings.metroBuildType.ToString());
		}
#endif
	}

	public void BuildStepUpdate ()
	{

	}

	public bool IsBuildStepDone ()
	{
		return true;
	}

	#endregion


}
