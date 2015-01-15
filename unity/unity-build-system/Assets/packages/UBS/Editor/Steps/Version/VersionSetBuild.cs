using UnityEngine;
using System.Collections;

namespace UBS.Version
{
	[BuildStepDescriptionAttribute("Sets the build version of the project to a given value. ")]
	public class SetBuild : IBuildStepProvider
	{
		#region IBuildStepProvider implementation

		public void BuildStepStart (BuildConfiguration pConfiguration)
		{
			var collection = pConfiguration.GetCurrentBuildCollection();

			int build = collection.version.build;
			if(int.TryParse(pConfiguration.Params, out build))
			{
				collection.version.build = build;
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