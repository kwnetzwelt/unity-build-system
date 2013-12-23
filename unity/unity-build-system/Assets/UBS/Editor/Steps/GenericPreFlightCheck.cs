using UnityEngine;
using System.Collections;
using UBS;
using System.IO;
using UnityEditor;

public class GenericPreFlightCheck : IBuildStepProvider {

	bool mIsDone = false;

	#region IBuildStepProvider implementation

	public void BuildStepStart (BuildConfiguration pConfiguration)
	{
		CheckOutputPath();
	}

	public void BuildStepUpdate ()
	{
		return;
	}

	public bool IsBuildStepDone ()
	{
		return mIsDone;
	}

	#endregion

	void CheckOutputPath()
	{
		string error = "";
		UBSProcess ubs = UBSProcess.LoadUBSProcess();
		BuildProcess process = ubs.GetCurrentProcess();
		
		if(process.mOutputPath.Length == 0) {
			error = "Please provide an output path.";
			ubs.Cancel(error);
			return;
		}
		
		DirectoryInfo dir = new DirectoryInfo(process.mOutputPath);
		
		try
		{
			if(dir.Exists)
				mIsDone = true;
			else
				error = "The given output path is invalid.";
		}
		catch (IOException pIO)
		{
			error = pIO.ToString();
		}
		
		if(error.Length > 0)
		{
			ubs.Cancel(error);
		}
	}
}
