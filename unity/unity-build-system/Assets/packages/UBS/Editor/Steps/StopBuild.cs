using UnityEngine;
namespace UBS
{
	public class StopBuild : IBuildStepProvider
	{
		#region IBuildStepProvider implementation

		public void BuildStepStart (BuildConfiguration pConfiguration)
		{
			pConfiguration.Cancel("Cancelled by StopBuild. ");
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
