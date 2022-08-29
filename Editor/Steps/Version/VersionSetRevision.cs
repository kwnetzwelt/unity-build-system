using UnityEngine;
using System.Collections;

namespace UBS.Version
{
	[BuildStepDescriptionAttribute("Sets the revision of the project to a given value. ")]
	[BuildStepParameterFilterAttribute(BuildStepParameterType.String)]
	[BuildStepToggle("Save to PlayerSettings")]
	public class SetRevision : IBuildStepProvider
	{
		#region IBuildStepProvider implementation

		public void BuildStepStart (BuildConfiguration configuration)
		{
			var collection = configuration.GetCurrentBuildCollection();
			
			int revision = collection.version.revision;
			if(int.TryParse(configuration.Parameters, out revision))
			{
				collection.version.revision = revision;
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
