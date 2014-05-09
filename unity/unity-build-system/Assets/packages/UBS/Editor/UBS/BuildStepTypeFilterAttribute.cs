using UnityEngine;
using System.Collections;
using UBS;
using System.IO;
using UnityEditor;
namespace UBS
{
	/// <summary>
	/// Use this attribute to constrain display of this build step to a specific build step type. 
	/// </summary>
	public class BuildStepTypeFilterAttribute : System.Attribute
	{
		public EBuildStepType mBuildStepType = EBuildStepType.invalid;
		
		public BuildStepTypeFilterAttribute(EBuildStepType buildStepType)
		{
			mBuildStepType = buildStepType;
		}

	}
}
