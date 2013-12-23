using UnityEngine;
using System.Collections;
namespace UBS
{
	/// <summary>
	/// Use this attribute to add a custom description to a build step. 
	/// It is displayed as a tooltip, when the build step is selected and you hover the dropdown box. 
	/// </summary>
	public class BuildStepDescriptionAttribute : System.Attribute
	{
		internal string mDescription;
		public BuildStepDescriptionAttribute(string description)
		{
			mDescription = description;
		}
	}
}
