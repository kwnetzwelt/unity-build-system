using UnityEngine;
using System.Collections;

namespace UBS.Version
{
	[BuildStepDescriptionAttribute("Sets the build version of the project to a given value. ")]
	[BuildStepParameterFilterAttribute(BuildStepParameterType.String)]
	[BuildStepToggle("Save to PlayerSettings")]
	public class SetBuild : IBuildStepProvider
	{
		#region IBuildStepProvider implementation

		public void BuildStepStart (BuildConfiguration configuration)
		{
			var collection = configuration.GetCurrentBuildCollection();

			int build = collection.version.build;
			if(int.TryParse(configuration.Parameters, out build))
			{
				collection.version.build = build;
				collection.SaveVersion(configuration.ToggleValue);
			}else
			{
				Debug.LogError("Could not parse parameter: " + configuration.Parameters);
			}
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