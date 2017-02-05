using System;
using System.Collections.Generic;
using System.Linq;
using UBS;
using UnityEditor;

namespace UBS.Compiler
{
    [BuildStepDescriptionAttribute("Overwrites the compiler script definitions inside the unity player platform specific settings.")]
    [BuildStepParameterFilter(EBuildStepParameterType.String)]
    public class SetCompilerFlags : IBuildStepProvider
    {        
        #region IBuildStepProvider implementation
               
        public void BuildStepStart(BuildConfiguration pConfiguration)
        {
            BuildTargetGroup btg = Helpers.GroupFromBuildTarget(pConfiguration.GetCurrentBuildProcess().mPlatform);            
            PlayerSettings.SetScriptingDefineSymbolsForGroup(btg, pConfiguration.Params);
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