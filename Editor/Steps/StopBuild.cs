using UnityEngine;
namespace UBS
{
	[BuildStepParameterFilterAttribute(BuildStepParameterType.None)]
	public class StopBuild : IBuildStepProvider
	{
		#region IBuildStepProvider implementation

		public void BuildStepStart (BuildConfiguration configuration)
		{
			configuration.Cancel("Cancelled by StopBuild. ");
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
