using UnityEngine;
using System.Collections;
using UBS;
using UnityEditor;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
namespace UBS
{
	[BuildStepDescriptionAttribute("Increases the build revision by one. ")]
	public class VersionIncreaseRevision : IBuildStepProvider
	{
		#region IBuildStepProvider implementation
		
		public void BuildStepStart (BuildConfiguration pConfiguration)
		{
			var collection = pConfiguration.GetCurrentBuildCollection();

			collection.version.Increase();

			collection.SaveVersion();

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