using UnityEngine;
namespace UBS
{
	[BuildStepParameterFilterAttribute(BuildStepParameterType.None)]
	public class EmptyBuildStep : IBuildStepProvider
	{
		#region IBuildStepProvider implementation

		public void BuildStepStart (BuildConfiguration configuration)
		{

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
