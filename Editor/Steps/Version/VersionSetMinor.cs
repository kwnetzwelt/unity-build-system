using UnityEngine;
using System.Collections;

namespace UBS.Version
{
	[BuildStepDescriptionAttribute("Sets the minor version of the project to a given value. ")]
	[BuildStepParameterFilterAttribute(BuildStepParameterType.String)]
	public class SetMinor : IBuildStepProvider
	{
		#region IBuildStepProvider implementation

		public void BuildStepStart (BuildConfiguration configuration)
		{
			var collection = configuration.GetCurrentBuildCollection();
			
			int minor = collection.version.minor;
			if(int.TryParse(configuration.Parameters, out minor))
			{
				collection.version.minor = minor;
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