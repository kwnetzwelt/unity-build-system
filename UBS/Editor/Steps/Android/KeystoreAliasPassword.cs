using UnityEngine;
using System.Collections;
using UnityEditor;

namespace UBS.Android
{
	[BuildStepPlatformFilterAttribute(BuildTarget.Android)]
	[BuildStepDescriptionAttribute("Sets the password for the alias (name) for the used keystore to a given value")]
	[BuildStepParameterFilterAttribute(EBuildStepParameterType.String)]
	public class KeystoreAliasPassword : IBuildStepProvider
	{
		#region IBuildStepProvider implementation
		
		public void BuildStepStart (BuildConfiguration pConfiguration)
		{
			PlayerSettings.Android.keyaliasPass = pConfiguration.Params;
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

