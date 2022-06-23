using UnityEngine;
using System.Collections;
using UnityEditor;

namespace UBS.Android
{
	[BuildStepPlatformFilterAttribute(BuildTarget.Android)]
	[BuildStepDescriptionAttribute("Sets the subtarget (texture compression type) for android. Values: Generic, DXT, PVRTC,	ATC, ETC, ETC2, ASTC")]
	[BuildStepParameterFilterAttribute(typeof(MobileTextureSubtarget))]
	public class Subtarget : IBuildStepProvider
	{
		#region IBuildStepProvider implementation
		
		public void BuildStepStart (BuildConfiguration configuration)
		{
			EditorUserBuildSettings.androidBuildSubtarget = (MobileTextureSubtarget)System.Enum.Parse(typeof(MobileTextureSubtarget),configuration.Parameters);
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

