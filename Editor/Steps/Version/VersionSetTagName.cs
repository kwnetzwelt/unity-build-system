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
    [BuildStepDescriptionAttribute("If no Parameter is given it used the last EditorPrefs.string(tagName)(added by jenkins), must be called after all version steps")]
    [BuildStepParameterFilterAttribute(BuildStepParameterType.None)]
    [BuildStepToggle("Save to PlayerSettings")]
    public class SetTagName : IBuildStepProvider
    {
		#region IBuildStepProvider implementation
		
        public void BuildStepStart(BuildConfiguration configuration)
        {
            var collection = configuration.GetCurrentBuildCollection();
            
            //comes from jenkins ubs.ubsProcess.BuildFromCommandline
            string tagName = EditorPrefs.GetString("tagName");
                
            //edit the version, because jenkins doesnt know the correct version at this step
            tagName = tagName.Replace("#version", collection.version.ToLabelString());
           
            UnityEngine.Debug.Log("Set TagName Step: " + tagName);
            
            collection.version.tagName = tagName;
            collection.SaveVersion(configuration.ToggleValue);
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