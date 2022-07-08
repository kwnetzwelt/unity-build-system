using UnityEngine;
namespace UBS
{
	[BuildStepParameterFilterAttribute(BuildStepParameterType.UnityObject)]
	public class EmptyBuildStep : IBuildStepProvider
	{
		#region IBuildStepProvider implementation

		public void BuildStepStart (BuildConfiguration configuration)
		{
			//example
			//Texture2D tex = configuration.Parameters.Cast<Texture2D>();
		}

		public void BuildStepUpdate ()
		{
			Debug.Log(UBSProcess.LoadUBSProcess().CurrentState == UBSState.aborted);
		}

		public bool IsBuildStepDone ()
		{
			return true;
		}

		#endregion


	}
}
