using UnityEditor;

namespace UBS.Android
{
	[BuildStepPlatformFilterAttribute(BuildTarget.Android)]
	[BuildStepDescriptionAttribute("Sets the path to the used keystore to a given value")]
	[BuildStepParameterFilterAttribute(EBuildStepParameterType.String)]
	public class KeystorePath : IBuildStepProvider
	{
		#region IBuildStepProvider implementation
		
		public void BuildStepStart (BuildConfiguration pConfiguration)
		{
		    var path = pConfiguration.Params.Replace("$PROJECTDIR", pConfiguration.ProjectDirectory);
            path = path.Replace("$OUTDIR", path);
            PlayerSettings.Android.keystoreName = path;
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

