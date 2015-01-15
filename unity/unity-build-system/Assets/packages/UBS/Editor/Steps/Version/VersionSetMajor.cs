using UnityEngine;
using System.Collections;

namespace UBS
{
	[BuildStepDescriptionAttribute("Sets the major version of the project to a given value. ")]
	public class VersionSetMajor : IBuildStepProvider
	{
		#region IBuildStepProvider implementation

		public void BuildStepStart (BuildConfiguration pConfiguration)
		{
			var collection = pConfiguration.GetCurrentBuildCollection();
			
			int major = collection.version.major;
			if(int.TryParse(pConfiguration.Params, out major))
			{
				collection.version.major = major;
				collection.SaveVersion();
			}else
			{
				Debug.LogError("Could not parse parameter: " + pConfiguration.Params);
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