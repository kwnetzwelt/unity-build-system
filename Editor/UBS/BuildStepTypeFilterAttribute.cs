using UnityEngine;
using System.Collections;
using UBS;
using System.IO;
using UnityEditor;
namespace UBS
{
	/// <summary>
	/// Use this attribute to constrain availability of this build step to a specific build step type. 
	/// </summary>
	public class BuildStepTypeFilterAttribute : System.Attribute
	{
        public BuildStepType BuildStepType { get; } = BuildStepType.invalid;

        public BuildStepTypeFilterAttribute(BuildStepType buildStepType)
		{
			this.BuildStepType = buildStepType;
		}

	}
}
