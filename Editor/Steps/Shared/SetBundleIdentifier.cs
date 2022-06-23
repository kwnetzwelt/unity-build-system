using UnityEngine;
using System.Collections;
using UnityEditor;

namespace UBS.Shared
{
	[BuildStepDescriptionAttribute("Sets the bundle identifier (shared between multiple platforms)")]
	[BuildStepParameterFilterAttribute(BuildStepParameterType.String)]
	public class SetBundleIdentifier : IBuildStepProvider
	{
		#region IBuildStepProvider implementation
		
		public void BuildStepStart (BuildConfiguration configuration)
		{
#if UNITY_5
            PlayerSettings.bundleIdentifier = configuration.Parameters;
#else
            PlayerSettings.applicationIdentifier = configuration.Parameters;
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
}

