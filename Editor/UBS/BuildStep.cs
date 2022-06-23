using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using UnityEngine.Serialization;

namespace UBS
{
	[Serializable]
	public class BuildStep {

		public BuildStep()
		{

		}

		public BuildStep(BuildStep other)
        {
            Enabled = other.Enabled;
			StepType = other.StepType;
			TypeName = other.TypeName;
			Parameters = other.Parameters;
		}

        [field: FormerlySerializedAs("mEnabled")]
        [field: SerializeField()]
        public bool Enabled { get; set; } = false;

        [field: FormerlySerializedAs("mParams")]
        [field: SerializeField()]
        public string Parameters { get; set; } = "";

        [field: FormerlySerializedAs("mTypeName")]
        [field: SerializeField()]
        public string TypeName { get; set; } = "";

        [field:FormerlySerializedAs("mAssemblyName")]
        [field: SerializeField()]
        public string AssemblyName { get; set; } = "";

        public Type StepType { get; set; }

        public void InferType()
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