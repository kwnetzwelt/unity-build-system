using UnityEngine;
using System.Collections;
using UBS;

public class MyCustomStep : IBuildStepProvider {

	int i = 0;
	int target = 0;
	public void BuildStepStart(BuildConfiguration pConfiguration)
	{
		target = (new System.Random()).Next(100);

		Debug.Log(pConfiguration.AssetsDirectory);
		Debug.Log(pConfiguration.ProjectDirectory);
		Debug.Log(pConfiguration.ResourcesDirectory);
	}

	public void BuildStepUpdate()
	{
		i++;
	}
	public bool IsBuildStepDone()
	{
		return i >= target;
	}

}
