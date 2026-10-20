using System;
using System.Collections.Generic;
using UBS;
using UnityEditor;

namespace UBS.Compiler
{
	[BuildStepDescriptionAttribute("Removes a compiler flag from your build.")]
	[BuildStepParameterFilterAttribute(BuildStepParameterType.String)]
	public class RemoveCompilerFlag : IBuildStepProvider
	{
		
		#region IBuildStepProvider implementation
		
		public void BuildStepStart (BuildConfiguration configuration)
		{
			UnityEditor.Build.NamedBuildTarget btg = UBS.Helpers.NamedBuildTargetFromBuildTarget( configuration.GetCurrentBuildProcess().Platform );
			string symbols = PlayerSettings.GetScriptingDefineSymbols(btg);
			List<string> symbolsArray = new List<string>(symbols.Split(';'));
			
			if(symbolsArray.Contains(configuration.Parameters))
				symbolsArray.Remove(configuration.Parameters);
			symbols = string.Join(";", symbolsArray.ToArray());
			
			
			PlayerSettings.SetScriptingDefineSymbols(btg, symbols);
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