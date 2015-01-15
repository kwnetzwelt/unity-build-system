using UnityEngine;
using System.Collections;

namespace UBS
{
	[BuildStepDescriptionAttribute("Sets the minor version of the project to a given value. ")]
	public class VersionSetMinor : IBuildStepProvider
	{
		#region IBuildStepProvider implementation

		public void BuildStepStart (BuildConfiguration pConfiguration)
		{
			var collection = pConfiguration.GetCurrentBuildCollection();
			
			int minor = collection.version.minor;
			if(int.TryParse(pConfiguration.Params, out minor))
			{
				collection.version.minor = minor;
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