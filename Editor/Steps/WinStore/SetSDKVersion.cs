using UnityEngine;
using UnityEditor;
using System.Collections;
using UBS;


[BuildStepPlatformFilter(BuildTarget.WSAPlayer)]
[BuildStepDescriptionAttribute("Sets the Windows SDK version. This is usually used for commandline build processes. Values: SDK80, SDK81, PhoneSDK81, UniversalSDK81")]
[BuildStepParameterFilterAttribute(BuildStepParameterType.String)]
public class SetSDKVersion : IBuildStepProvider 
{
#region IBuildStepProvider implementation
	public void BuildStepStart (BuildConfiguration configuration)
	{
#if UNITY_5
        EditorUserBuildSettings.wsaSDK = (WSASDK)System.Enum.Parse(typeof(WSASDK), configuration.Parameters);
		
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
