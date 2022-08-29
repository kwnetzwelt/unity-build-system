using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using UnityEngine.Serialization;

namespace UBS
{
	[Serializable]
	public class BuildStep {

		[Serializable]
		public class BuildStepParameters
		{
			public bool boolParameter;
			public string stringParameter;
			public UnityEngine.Object objectParameter;
			public int intParameter;
			public static implicit operator string(BuildStepParameters p)
			{
				return p.stringParameter;
			}
			public static implicit operator int(BuildStepParameters p)
			{
				return p.intParameter;
			}
			public static implicit operator UnityEngine.Object(BuildStepParameters p)
			{
				return p.objectParameter;
			}
			public static implicit operator bool(BuildStepParameters p)
			{
				return p.boolParameter;
			}

			public T Cast<T>() where T : UnityEngine.Object
			{
				return (T) objectParameter;
			}
		}
		public BuildStep()
		{

		}

		public BuildStep(BuildStep other)
        {
            Enabled = other.Enabled;
			StepType = other.StepType;
			TypeName = other.TypeName;
			AssemblyName = other.AssemblyName;
			Parameters = other.Parameters;
		}

        [field: FormerlySerializedAs("mEnabled")]
        [field: SerializeField()]
        public bool Enabled { get; set; } = false;

        [field: FormerlySerializedAs("mParams")]
        [field: SerializeField()]
        public BuildStepParameters Parameters { get; set; } = new BuildStepParameters();
        [field: SerializeField()]
        public bool ToggleValue { get; set; }

        [field: FormerlySerializedAs("mTypeName")]
        [field: SerializeField()]
        public string TypeName { get; set; } = "";

        [field:FormerlySerializedAs("mAssemblyName")]
        [field: SerializeField()]
        public string AssemblyName { get; set; } = "";

        public Type StepType { get; set; }

        public bool TryInferType(bool logError = true)
        {
	        try
	        {

	        
            if (!string.IsNullOrEmpty(AssemblyName))
            {
                Assembly a = Assembly.Load(AssemblyName);
                StepType = a.GetType(TypeName);
            }
            else
            {
                StepType = Type.GetType(TypeName);
            }
	        }
	        catch (Exception e)
	        {
		        if(logError)
					UnityEngine.Debug.LogError($"Could not infer type from {AssemblyName} -> {TypeName}");
		        StepType = null;
		        return false;
	        }

	        return true;
        }

		public void SetStepType(System.Type stepType)
		{

			StepType = stepType;
			if(stepType == null)
				TypeName = "";
            else
            {
                TypeName = StepType.ToString();
                AssemblyName = StepType.Assembly.ToString();
            }
        }

	}
}