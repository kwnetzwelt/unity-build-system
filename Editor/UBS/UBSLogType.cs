using System;
using System.Text;
using UnityEngine;

namespace UBS
{
    [Serializable]
    public class UBSLogType : ISerializationCallbackReceiver
    {
        [HideInInspector]
        public string Name = "";
		
        [field:SerializeField]
        public LogType LogType { get; set; }
		
        [field:SerializeField]
        public StackTraceLogType StackTrace { get; set; }
		
        [field:SerializeField]
        [Tooltip("After build or if build is cancelled the StackTraceLogType will be restored to this value")]
        public StackTraceLogType StackTrackToRestore { get; set; }

        [field:SerializeField]
        public bool UseInBatchMode { get; set; }
		
        [field:SerializeField]
        public bool UseInEditMode { get; set; }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            StringBuilder builder = new StringBuilder();
            if (UseInBatchMode)
            {
                builder.Append("BatchMode");
				
            }

            if (UseInEditMode)
            {
                if(UseInBatchMode)
                    builder.Append(", ");
                builder.Append("EditMode");
            }

            if (UseInBatchMode || UseInEditMode)
                builder.Append(": ");
            builder.Append(LogType.ToString() );
            builder.Append(", ");
            builder.Append("StackTrace: " + StackTrace.ToString());
            Name = builder.ToString();
        }
    }
}