using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace UBS
{
	/// <summary>
	/// Use a derived class of this to parse start up options (parameters) of a command line build with unity3d. 
	/// You don't provide a public constructor. The Build Configuration will do that. 
	/// You can get an instance of your implementation via <see cref="BuildConfiguration::GetProgramOptions<T>()"/>
	/// </summary>
	public abstract class ProgramOptions
	{
		
		private ProgramOptions(string[] args)
		{
			Parse(args);
		}

		internal static T CreateInstance<T>(string[] args) where T : ProgramOptions
		{
			T instance = (T)Activator.CreateInstance(typeof(T), args);
			return instance;
		}

		/// <summary>
		/// My little parsing logic which reflects all properties, check if an OptionAttribute is present and try to match a regex 
		/// with a argument option
		/// </summary>
		/// <param name="args"></param>
		private void Parse(string[] args)
		{
			foreach(var prop in this.GetType().GetProperties())
			{
				// only strings are cool
				if (prop.PropertyType != typeof(string)) continue;
				
				// Scan if a option attribute is present and try to grab a argument
				foreach(var attribute in prop.GetCustomAttributes(true))
				{
					OptionAttribute optionAttribute = attribute as OptionAttribute;
					if (optionAttribute != null)
					{
						// Set default to string.Empty
						prop.SetValue(this, Convert.ChangeType(string.Empty, prop.PropertyType), null);
						
						Regex attributeRegex = optionAttribute.GetRegex();
						
						foreach(string arg in args)
						{
							if (attributeRegex.IsMatch(arg))
							{
								string val = arg.Substring(attributeRegex.ToString().Length);
								prop.SetValue(this, Convert.ChangeType(val, prop.PropertyType), null);
							}
						}
					}
				}
			}
		}
		
	}
	
	[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
	
	public class OptionAttribute : Attribute
	{
		string _regexPattern = string.Empty;
		
		public OptionAttribute(string regexPattern)
		{
			_regexPattern = regexPattern;
		}
		
		public Regex GetRegex()
		{
			return new Regex(_regexPattern);
		}
		
	}
}

