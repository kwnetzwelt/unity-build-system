using System;
using System.Collections.Generic;
using System.Linq;
using UBS;
using UnityEditor;

namespace UBS.Compiler
{
    [BuildStepDescriptionAttribute("Overwrites the compiler script definitions inside the unity player platform specific settings.")]
    [BuildStepParameterFilter(BuildStepParameterType.String)]
    public class SetCompilerFlags : IBuildStepProvider
    {        
        #region IBuildStepProvider implementation
               
        public void BuildStepStart(BuildConfiguration configuration)
        {
            UnityEditor.Build.NamedBuildTarget btg = Helpers.NamedBuildTargetFromBuildTarget(configuration.GetCurrentBuildProcess().Platform);            
            PlayerSettings.SetScriptingDefineSymbols(btg, configuration.Parameters);
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