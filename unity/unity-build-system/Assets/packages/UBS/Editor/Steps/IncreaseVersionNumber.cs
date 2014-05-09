using UnityEngine;
using System.Collections;
using UBS;
using UnityEditor;
using System.IO;
using System.Diagnostics;
using System.Linq;

public class IncreaseVersionNumber : IBuildStepProvider
{
	#region IBuildStepProvider implementation

	public void BuildStepStart (BuildConfiguration pConfiguration)
	{
		var collection = pConfiguration.GetCurrentBuildCollection();
		collection.version.Increase();


		
		#if UNITY_EDITOR_WIN
		collection.version.revision = GetSVNRevisionOnWindows(collection.version.revision);
		#endif

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

	string FindExecutable(string pExecutable)
	{
		var enviromentPath = System.Environment.GetEnvironmentVariable("PATH");
		UnityEngine.Debug.Log(enviromentPath);
		var paths = enviromentPath.Split(';');
		var exePath = paths.Select(x => Path.Combine(x, pExecutable))
			.Where(x => File.Exists(x))
				.FirstOrDefault();

		UnityEngine.Debug.Log(exePath);
		return exePath;
	}
	
	int GetSVNRevisionOnWindows(int pOrgRev)
	{
		const string kRevPrefix = "Revision: ";

		var process = new Process {
			StartInfo = new ProcessStartInfo {
				FileName = FindExecutable("svn.exe"),
				Arguments = "info",
				UseShellExecute = false,
				RedirectStandardOutput = true,
				CreateNoWindow = true
			}
		};

		string v = pOrgRev.ToString();


		process.Start();
		while (!process.StandardOutput.EndOfStream) {
			string line = process.StandardOutput.ReadLine();
			UnityEngine.Debug.Log(line);
			if(line.StartsWith(kRevPrefix))
				v = line.Substring(kRevPrefix.Length);
		}


		int result = -1;

		if(int.TryParse(v, out result))
		{
			return result;
		}
		return pOrgRev;
	}



}

