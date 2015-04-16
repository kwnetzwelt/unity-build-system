using UnityEngine;
namespace UBS
{
	[BuildStepParameterFilterAttribute(EBuildStepParameterType.None)]
	public class EmptyBuildStep : IBuildStepProvider
	{
		#region IBuildStepProvider implementation

		public void BuildStepStart (BuildConfiguration pConfiguration)
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
