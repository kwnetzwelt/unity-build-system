using UnityEngine;
using System.Collections;
using UnityEditor;

namespace UBS.Shared
{
	[BuildStepDescriptionAttribute("Sets the bundle identifier (shared between multiple platforms). Can be overridden by Commandline Argument `bundleIdentifier`. ")]
	[BuildStepParameterFilterAttribute(BuildStepParameterType.String)]
	public class SetBundleIdentifier : IBuildStepProvider
	{
		#region IBuildStepProvider implementation
		
		public void BuildStepStart (BuildConfiguration configuration)
		{
			var stringParameter = configuration.Parameters.stringParameter;
			configuration.CommandLineArgs.TryGetValue("bundleIdentifier", ref stringParameter);
            PlayerSettings.applicationIdentifier = stringParameter;
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

