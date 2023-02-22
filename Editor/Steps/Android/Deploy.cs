using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace UBS.Android
{
	[BuildStepDescription("Your build apk will be deployed to an attached Android device. This scripts includes obb deployment. Include a parameter run to instantly run the apk. ")]
	[BuildStepPlatformFilter( UnityEditor.BuildTarget.Android )]
	[BuildStepParameterFilter(BuildStepParameterType.String)]
	public class Deploy : IBuildStepProvider
	{
		string scriptPath = "Assets/packages/UBS/Dependencies/PythonScripts/";
		const string scriptName = "android_deploy.py";
		string workingDirectory = "";

        #region IBuildStepProvider implementation
        
		//android_deploy.py --apk=UBS3-Android-Public/carcassonne.apk --packagename=com.exozet.game.carcassonne --vcode=5198 --obb=UBS3-Android-Public/carcassonne.main.obb
        
		public void BuildStepStart(BuildConfiguration configuration)
		{
			List<string> parameters = new List<string>(((string)configuration.Parameters).Split(';'));

			FileInfo path = new FileInfo(configuration.GetCurrentBuildProcess().OutputPath);
			DirectoryInfo workingDirectoryInfo = path.Directory;
			workingDirectory = path.Directory.FullName;

			DirectoryInfo di = new DirectoryInfo(scriptPath);
			scriptPath = di.FullName;

			string versionCode = UnityEditor.PlayerSettings.Android.bundleVersionCode.ToString();
#if UNITY_5
            string packageName = UnityEditor.PlayerSettings.bundleIdentifier;
#else
            string packageName = UnityEditor.PlayerSettings.applicationIdentifier;
#endif


            FileInfo[] apks = workingDirectoryInfo.GetFiles("*.apk");
			FileInfo[] obbs = workingDirectoryInfo.GetFiles("*.obb");

			FileInfo apk = null;
			FileInfo obb = null;

			if(obbs.Length > 0)
				obb = obbs[0];

			if(apks.Length > 0)
				apk = apks[0];

			//
			// some debug output 
			//

			UnityEngine.Debug.Log( apk.Name );
			if(obb != null)
				UnityEngine.Debug.Log( obb.Name );

			UnityEngine.Debug.Log( versionCode);
			UnityEngine.Debug.Log( packageName);
			UnityEngine.Debug.Log( scriptPath);
			UnityEngine.Debug.Log( workingDirectory);



			//
			// generate the arguments for our python script
			//
			List<string> args = new List<string>();

			args.Add("--apk=" + apk);
			args.Add("--packagename=" + packageName);
			args.Add("--vcode=" + versionCode);
			if(obb != null)
				args.Add("--obb=" + obb);

			// support run option
			string customRun = parameters.Find( (str) => str.StartsWith("run="));
			if(parameters.Contains("run"))
				args.Add("--run");
			else if(!string.IsNullOrEmpty( customRun ))
				args.Add("--" + customRun);

			// support clear option
			if(parameters.Contains("clear"))
				args.Add("--clear");

			string arguments = string.Join(" ", args.ToArray());

			UnityEngine.Debug.Log(arguments);

			Process p = ShellProcess(scriptName + " " + arguments);
			p.WaitForExit();
		}

		public void BuildStepUpdate()
		{

		}

		public bool IsBuildStepDone()
		{
			return true;
		}

#endregion

		public Process ShellProcess(string pArgs)
		{
			Process p = new Process();
			p.StartInfo.Arguments = pArgs;
			p.StartInfo.CreateNoWindow = false;
			p.StartInfo.UseShellExecute = true;
			p.StartInfo.RedirectStandardOutput = false;
			p.StartInfo.RedirectStandardInput = false;
			p.StartInfo.RedirectStandardError = false;
			p.StartInfo.WorkingDirectory = scriptPath;
			p.StartInfo.FileName = "python";
			p.Start();
			return p;
		}

	}
}

