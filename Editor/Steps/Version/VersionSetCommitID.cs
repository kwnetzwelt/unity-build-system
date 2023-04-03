using UnityEngine;
using System.Collections;
using UBS;
using UnityEditor;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

namespace UBS.Version
{
    [BuildStepDescriptionAttribute("If no Parameter is given it uses the last EditorPrefs.string(commitID)(added by jenkins). If that is also not set uses local GIT process to get lasted CommitID. Must be called after all version steps")]
    [BuildStepParameterFilterAttribute(BuildStepParameterType.String)]
    [BuildStepToggle("Save to PlayerSettings")]
    public class SetCommitID : IBuildStepProvider
    {
		#region IBuildStepProvider implementation
		
        public void BuildStepStart(BuildConfiguration configuration)
        {
	        var collection = configuration.GetCurrentBuildCollection();
            
            // Has this been set by Jenkins?
            string commitID = EditorPrefs.GetString("commitID");
            
            // If not, use locally installed git to get commit ID
            if (string.IsNullOrEmpty(commitID))
            {
	            try
	            {
		            commitID = GetGitCommitHash();
	            }
	            catch (System.Exception e)
	            {
		            UnityEngine.Debug.LogError("Could not call into git: " + e);
	            }
            }

	        UnityEngine.Debug.Log("Set CommitID Step: " + commitID);
           
            collection.version.commitID = commitID;
            collection.SaveVersion(configuration.ToggleValue);
        }
		
        public void BuildStepUpdate()
        {
			
        }
		
        public bool IsBuildStepDone()
        {
            return true;
        }
        
        
        private static string GetGitCommitHash()
        {
	        
	        // Implementation from https://github.com/LotteMakesStuff/LMS.Version
	        
	        // example of how to hardcode the git path, if its not in your system PATH.
	        // #if UNITY_EDITOR_WIN
	        //         const string gitPath = "C:\\Program Files\\Git\\bin\\git.exe";
	        // #endif
	        const string gitPath = "git";
	        var gitInfo = new ProcessStartInfo
	        {
		        CreateNoWindow = true,
		        RedirectStandardError = true,
		        RedirectStandardOutput = true,
		        FileName = gitPath,
		        UseShellExecute = false,
	        };

	        var gitProcess = new Process();
	        gitInfo.Arguments = "rev-parse --short HEAD"; // magic command to get the current commit hash
	        gitInfo.WorkingDirectory = Directory.GetCurrentDirectory();
	        gitProcess.StartInfo = gitInfo;
	        gitProcess.Start();
	        var stdout = gitProcess.StandardOutput.ReadToEnd();
	        gitProcess.WaitForExit();
	        gitProcess.Close();
	        stdout = stdout.Trim();
	        return stdout;
        }
        
		#endregion
    }
}