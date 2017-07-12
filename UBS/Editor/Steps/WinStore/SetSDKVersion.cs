using UnityEngine;
using UnityEditor;
using System.Collections;
using UBS;


[BuildStepPlatformFilter(BuildTarget.WSAPlayer)]
[BuildStepDescriptionAttribute("Sets the Windows SDK version. This is usually used for commandline build processes. Values: SDK80, SDK81, PhoneSDK81, UniversalSDK81")]
[BuildStepParameterFilterAttribute(EBuildStepParameterType.String)]
public class SetSDKVersion : IBuildStepProvider 
{
#region IBuildStepProvider implementation
	public void BuildStepStart (BuildConfiguration pConfiguration)
	{
#if UNITY_5
        EditorUserBuildSettings.wsaSDK = (WSASDK)System.Enum.Parse(typeof(WSASDK), pConfiguration.Params);
		
		if(UBSProcess.LoadUBSProcess().IsInBatchMode)
		{
			Debug.Log("Set Windows SDK to: " + EditorUserBuildSettings.wsaSDK.ToString());
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
