using System;
using UnityEngine;
namespace UBS
{
	[BuildStepParameterFilterAttribute(BuildStepParameterType.String)]
	public class EmptyBuildStep : IBuildStepProvider
	{
		#region IBuildStepProvider implementation

		public void BuildStepStart (BuildConfiguration configuration)
		{
			//example
			//Texture2D tex = configuration.Parameters.Cast<Texture2D>();
			//if(Application.isBatchMode)
			//	Console.WriteLine(configuration.GetCurrentBuildProcess().Platform + " " + configuration.Parameters.stringParameter);
			//else
				Debug.Log(configuration.GetCurrentBuildProcess().Platform + " " + configuration.Parameters.stringParameter);
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
