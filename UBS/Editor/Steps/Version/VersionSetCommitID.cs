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
    [BuildStepDescriptionAttribute("If no Parameter is given it used the last EditorPrefs.string(commitID)(added by jenkins), must be called after all version steps")]
    [BuildStepParameterFilterAttribute(EBuildStepParameterType.String)]
    public class SetCommitID : IBuildStepProvider
    {
		#region IBuildStepProvider implementation
		
        public void BuildStepStart(BuildConfiguration pConfiguration)
        {
            var collection = pConfiguration.GetCurrentBuildCollection();
            
            string commitID = EditorPrefs.GetString("commitID");
           
            UnityEngine.Debug.Log("Set CommitID Step: " + commitID);
           
            collection.version.commitID = commitID;
            collection.SaveVersion();
        }
		
        public void BuildStepUpdate()
        {
			
        }
		
        public bool IsBuildStepDone()
        {
            return true;
        }
		#endregion
    }
}