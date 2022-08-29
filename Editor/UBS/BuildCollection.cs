using System;
using System.Collections.Generic;
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

		public void SaveVersionCode()
		{
			UnityEditor.PlayerSettings.Android.bundleVersionCode = int.Parse(versionCode);
		}

		public void LoadVersionFromSettings()
		{
			version.ParseFromBundleVersion(UnityEditor.PlayerSettings.bundleVersion);
		}
	}
}

