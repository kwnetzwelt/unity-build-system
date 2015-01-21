using UnityEngine;
using System.Collections;
using UnityEditor;

namespace UBS.Android
{
	[BuildStepPlatformFilterAttribute(BuildTarget.Android)]
	[BuildStepDescriptionAttribute("Sets the subtarget (texture compression type) for android. Values: Generic, DXT, PVRTC,	ATC, ETC, ETC2, ASTC")]
	public class Subtarget : IBuildStepProvider
	{
		#region IBuildStepProvider implementation
		
		public void BuildStepStart (BuildConfiguration pConfiguration)
		{
			EditorUserBuildSettings.androidBuildSubtarget = (AndroidBuildSubtarget)System.Enum.Parse(typeof(AndroidBuildSubtarget),pConfiguration.Params);
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
}

