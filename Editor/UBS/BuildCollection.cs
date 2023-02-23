using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace UBS
{
	[Serializable]
	public class BuildCollection : ScriptableObject
	{
		void OnEnable()
		{
			version = BuildVersion.Load();
		}

		[field:SerializeField()]
		public List<UBSLogType> LogTypes { get; private set; } = new List<UBSLogType>();

		[field: FormerlySerializedAs("mProcesses")]
        [field:SerializeField()]
        public List<BuildProcess> Processes { get; private set; } = new List<BuildProcess>();

        public void SaveVersion(bool writeToPlayerSettings)
		{
			version.Save();
			if (writeToPlayerSettings)
			{
				UnityEditor.PlayerSettings.Android.bundleVersionCode = version.revision;
				UnityEditor.PlayerSettings.iOS.buildNumber = version.revision.ToString();
				UnityEditor.PlayerSettings.bundleVersion = version.ToShortString();
			}
		}

		public BuildVersion version = null;
		public string versionCode = "1";
		public bool cleanBuild = false;

		public void SaveVersionCode()
		{
			UnityEditor.PlayerSettings.Android.bundleVersionCode = int.Parse(versionCode);
		}

		public void LoadVersionFromSettings()
		{
			version.ParseFromBundleVersion(UnityEditor.PlayerSettings.bundleVersion);
		}

		public void ActivateLogTypes()
		{
			foreach (var logType in LogTypes)
			{
				if(Application.isBatchMode && logType.UseInBatchMode || !Application.isBatchMode && logType.UseInEditMode)
					Application.SetStackTraceLogType(logType.LogType, logType.StackTrace);
			}
		}

		public void RestoreLogTypes()
		{
			foreach (var logType in LogTypes)
			{
				if(Application.isBatchMode && logType.UseInBatchMode || !Application.isBatchMode && logType.UseInEditMode)
					Application.SetStackTraceLogType(logType.LogType, logType.StackTrackToRestore);
			}
		}
	}
}

