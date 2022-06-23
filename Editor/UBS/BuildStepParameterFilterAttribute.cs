using UnityEngine;
using System.Collections;
using UBS;
using System.IO;
using UnityEditor;
using System;

namespace UBS
{
	/// <summary>
	/// Use this attribute to constrain display of this build step to a specific parameter display type. 
	/// </summary>
	public class BuildStepParameterFilterAttribute : System.Attribute
	{
		/// <summary>
		/// Provide parameter information for this build step
		/// </summary>
		/// <value>The type of the build parameter.</value>
		public BuildStepParameterType BuildParameterType { get; set; }
		/// <summary>
		/// In case of BuildParameterType is of type BuildParameterType.Dropdown
		/// all possible entries can be taken from here.
		/// </summary>
		/// <value>List of possible dropdown entries.</value>
		public string[] DropdownOptions { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="UBS.BuildStepParameterFilterAttribute"/> class.
		/// Use this constructor for BuildStepParameterType.None or BuildStepParameterType.String
		/// </summary>
		/// <param name="buildParam">Build parameter.</param>
        /// <example>[BuildStepParameterFilterAttribute(BuildStepParameterType.None)]</example>
		public BuildStepParameterFilterAttribute(BuildStepParameterType buildParam)
		{
			BuildParameterType = buildParam;
			DropdownOptions = null;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UBS.BuildStepParameterFilterAttribute"/> class.
		/// The BuildStepParameterType property will be dropdown and support the passed options.
		/// </summary>
		/// <param name="options">List of possible dropdown entry possibilities.</param>
        /// /// <example>[BuildStepParameterFilterAttribute("first", "second", "third")]</example>
		public BuildStepParameterFilterAttribute(params string[] options)
		{
			this.BuildParameterType = BuildStepParameterType.Dropdown;
			this.DropdownOptions = options;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="UBS.BuildStepParameterFilterAttribute"/> class.
        /// The BuildStepParameterType property will be dropdown and support the passed options.
        /// </summary>
        /// <param name="options">Enumeration type reflecting possible dropdown entry possibilities.</param>
        /// <example>[BuildStepParameterFilterAttribute(typeof(MyEnum))]</example>
        public BuildStepParameterFilterAttribute(Type options )
        {
            this.BuildParameterType = BuildStepParameterType.Dropdown;

            if(!options.IsEnum)
            {
                throw new ArgumentException("Argument is not of type enum");
            }

            this.DropdownOptions = Enum.GetNames(options);
        }
	}
}
