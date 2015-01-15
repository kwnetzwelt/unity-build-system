using UnityEngine;
using System.Collections;
using UnityEditor;

namespace UBS.Shared
{
	[BuildStepDescriptionAttribute("Sets the product name (shared between multiple platforms)")]
	public class SetProductName : IBuildStepProvider
	{
		#region IBuildStepProvider implementation
		
		public void BuildStepStart (BuildConfiguration pConfiguration)
		{
			PlayerSettings.productName = pConfiguration.Params;
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

