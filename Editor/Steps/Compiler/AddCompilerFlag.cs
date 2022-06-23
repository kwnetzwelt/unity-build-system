using System;
using System.Collections.Generic;
using UBS;
using UnityEditor;

namespace UBS.Compiler
{
	[BuildStepDescriptionAttribute("Adds a compiler flag to your build.")]
	[BuildStepParameterFilterAttribute(BuildStepParameterType.String)]
	public class AddCompilerFlag : IBuildStepProvider
	{
		
		#region IBuildStepProvider implementation
		
		public void BuildStepStart (BuildConfiguration configuration)
		{
			BuildTargetGroup btg = UBS.Helpers.GroupFromBuildTarget( configuration.GetCurrentBuildProcess().Platform );
			string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(btg);
			List<string> symbolsArray = new List<string>(symbols.Split(';'));

			if(!symbolsArray.Contains(configuration.Parameters))
				symbolsArray.Add(configuration.Parameters);
			symbols = string.Join(";", symbolsArray.ToArray());

			PlayerSettings.SetScriptingDefineSymbolsForGroup(btg, symbols);
		}
		
		public void BuildStepUpdate ()
		{
			
		}
		
		public bool IsBuildStepDone ()
		{
			return true;
		}
		
		#endregion
	}

}