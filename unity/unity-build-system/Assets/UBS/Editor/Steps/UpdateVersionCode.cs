using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UBS;

[BuildStepTypeFilter(EBuildStepType.PreBuildStep)]
[BuildStepDescriptionAttribute("Increases VersionCode from git or git svn info.")]	// adjust the .sh file
public class UpdateVersionCode :  IBuildStepProvider{

	bool mIsDone = false;
	string mPath;
	string mOutput = "";
	Process mProc;
	#region IBuildStepProvider implementation
	
	public void BuildStepStart (BuildConfiguration pConfiguration)
	{
		UnityEngine.Debug.Log("Check for new VersionCode...");
	#if UNITY_EDITOR_OSX
		mPath = Application.dataPath + "/Editor/";
		mProc = ShellProcess(mPath + "getRevision.sh", "");		// file have to be an executable one > chmod +x
		mOutput = mProc.StandardOutput.ReadToEnd();
		mProc.WaitForExit();
		UnityEngine.Debug.Log("Shell output: " + mOutput);
	#else
		mIsDone = true;
	#endif
	}

	public void BuildStepUpdate ()
	{
		if(mIsDone)
			return;

		string newVersionCode = "";
		if(mOutput.Length > 0)
		{
			newVersionCode = Regex.Replace(mOutput, "[^0-9]", "");
			if(newVersionCode.Length > 0) {
				UBSProcess ubs = UBSProcess.LoadUBSProcess();
				BuildCollection buildColl = ubs.BuildCollection;
				if(buildColl.versionCode.Length <= newVersionCode.Length)
					buildColl.versionCode = newVersionCode;
				else
				{
					string currentVersionCode = buildColl.versionCode.Substring(buildColl.versionCode.Length - newVersionCode.Length, newVersionCode.Length);
					buildColl.versionCode = currentVersionCode + newVersionCode;
					buildColl.SaveVersionCode();
				}
				UnityEngine.Debug.Log("Changed Version to: " + buildColl.versionCode);
			}
		}
		mIsDone = true;
	}

	public bool IsBuildStepDone ()
	{
		return mIsDone;
	}

	#endregion

	static Process ShellProcess(string pFilename, string pArgs)
	{
		Process p = new Process();
		p.StartInfo.Arguments = pArgs;
		p.StartInfo.CreateNoWindow = true;
		p.StartInfo.UseShellExecute = false;
		p.StartInfo.RedirectStandardOutput = true;
		p.StartInfo.RedirectStandardInput = true;
		p.StartInfo.RedirectStandardError = true;
		p.StartInfo.FileName = pFilename;
		p.Start();
		UnityEngine.Debug.Log("Executed shell: " + pFilename);
		return p;
	}
}
