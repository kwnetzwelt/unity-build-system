using System;
using System.Collections.Generic;
using System.Linq;
using UBS;
using UnityEditor;

namespace UBS.Compiler
{
    [BuildStepDescriptionAttribute("Clears all custom compiler flags but will allow a number of defines, when exists.")]
    [BuildStepParameterFilter(BuildStepParameterType.None)]
    public class ClearAllCompilerFlags : IBuildStepProvider
    {        
        #region IBuildStepProvider implementation

        /// <summary>
        /// System related Unity script symbol definitions. May change over time and are only used, when already included in script definition file.
        /// </summary>
        /// <returns>A list of all unity related script symbol definitions.</returns>
        private static List<string> UnitySystemRelatedSymbols()
        {
            return new List<string>{
                "MICRO", // micro windows store deployment
                "1", // ?????
                "NO_ECMASCRIPT" // IOS related IL2CPP flag
            };
        }
        
        public void BuildStepStart(BuildConfiguration configuration)
        {
            BuildTargetGroup btg = Helpers.GroupFromBuildTarget(configuration.GetCurrentBuildProcess().Platform);
            string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(btg);
            
            List<string> symbolsArray = new List<string>(symbols.Split(';'));
            List<string> systemSymbols = UnitySystemRelatedSymbols();

            var interesection = symbolsArray.Intersect(systemSymbols).ToArray();
           
            string clearedSymbols = String.Empty;
            if (interesection.Length > 0)
            {
                clearedSymbols = string.Join(";", interesection);
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(btg, clearedSymbols);
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