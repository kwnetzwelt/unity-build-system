using UnityEngine;
using System.Collections;
using UnityEditor;

namespace UBS.Shared
{
	[BuildStepDescriptionAttribute("Sets the bundle identifier (shared between multiple platforms)")]
	[BuildStepParameterFilterAttribute(EBuildStepParameterType.String)]
	public class SetBundleIdentifier : IBuildStepProvider
	{
		#region IBuildStepProvider implementation
		
		public void BuildStepStart (BuildConfiguration pConfiguration)
		{
			PlayerSettings.bundleIdentifier = pConfiguration.Params;
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

