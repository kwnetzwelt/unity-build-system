using UnityEngine;
using System.Collections;

namespace UBS
{
	[BuildStepDescriptionAttribute("Sets the revision of the project to a given value. ")]
	public class VersionSetRevision : IBuildStepProvider
	{
		#region IBuildStepProvider implementation

		public void BuildStepStart (BuildConfiguration pConfiguration)
		{
			var collection = pConfiguration.GetCurrentBuildCollection();
			
			int revision = collection.version.revision;
			if(int.TryParse(pConfiguration.Params, out revision))
			{
				collection.version.revision = revision;
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
