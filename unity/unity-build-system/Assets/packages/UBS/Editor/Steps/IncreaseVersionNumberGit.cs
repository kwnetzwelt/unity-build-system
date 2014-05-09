using UnityEngine;
using System.Collections;
using UBS;
using UnityEditor;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

public class IncreaseVersionNumberGit : IBuildStepProvider
{
	#region IBuildStepProvider implementation
	
	public void BuildStepStart (BuildConfiguration pConfiguration)
	{
		var collection = pConfiguration.GetCurrentBuildCollection();
		List<string> parameters = new List<string>(pConfiguration.Params.Split(';'));
		if(parameters.Contains("increaseBuild"))
			collection.version.Increase();

		collection.version.revision = collection.version.revision + 1;

		if(parameters.Contains("final"))
			collection.version.type = BuildVersion.BuildType.final;
		else
			collection.version.type = BuildVersion.BuildType.beta;

		collection.SaveVersion();

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

