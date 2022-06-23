using UnityEngine;
using System.Collections;
using UBS;
using System.IO;
using UnityEditor;

namespace UBS
{
	/// <summary>
	/// Use this attribute to constrain display of a buildstep to a specific platform. 
	/// </summary>
	public class BuildStepPlatformFilterAttribute : System.Attribute
	{
        internal BuildTarget BuildTarget { get; }

        public BuildStepPlatformFilterAttribute(BuildTarget buildTarget)
		{
			this.BuildTarget = buildTarget;
		}
	}
}
