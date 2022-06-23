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
            BuildTargetGroup btg = Helpers.GroupFromBuildTarget(configuration.GetCurrentBuildProcess().Platform);            
            PlayerSettings.SetScriptingDefineSymbolsForGroup(btg, configuration.Parameters);
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