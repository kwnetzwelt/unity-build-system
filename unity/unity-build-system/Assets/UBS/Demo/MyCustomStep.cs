using UnityEngine;
using System.Collections;
using UBS;

public class MyCustomStep : IBuildStepProvider {


	public void BuildStepStart(BuildConfiguration pConfiguration)
	{

	}

	public void BuildStepUpdate()
	{

	}
	public bool IsBuildStepDone()
	{
		return true;
	}

}
