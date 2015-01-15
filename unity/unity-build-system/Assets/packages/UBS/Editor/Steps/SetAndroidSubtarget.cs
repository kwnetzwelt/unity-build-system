using UnityEngine;
using System.Collections;
using UnityEditor;

namespace UBS
{
	[BuildStepPlatformFilterAttribute(BuildTarget.Android)]
	[BuildStepDescriptionAttribute("Sets the subtarget (texture compression type) for android. Values: Generic, DXT, PVRTC,	ATC, ETC, ETC2, ASTC")]
	public class SetAndroidSubtarget : IBuildStepProvider
	{
		#region IBuildStepProvider implementation
		
		public void BuildStepStart (BuildConfiguration pConfiguration)
		{
			EditorUserBuildSettings.androidBuildSubtarget = System.Enum.Parse(typeof(AndroidBuildSubtarget),pConfiguration.Params);
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

