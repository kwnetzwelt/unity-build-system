using UnityEngine;
using System.Collections;

namespace UBS.Version
{
	[BuildStepDescriptionAttribute("Sets the major version of the project to a given value. ")]
	[BuildStepParameterFilterAttribute(BuildStepParameterType.String)]
	public class SetMajor : IBuildStepProvider
	{
		#region IBuildStepProvider implementation

		public void BuildStepStart (BuildConfiguration configuration)
		{
			var collection = configuration.GetCurrentBuildCollection();
			
			int major = collection.version.major;
			if(int.TryParse(configuration.Parameters, out major))
			{
				collection.version.major = major;
				collection.SaveVersion();
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