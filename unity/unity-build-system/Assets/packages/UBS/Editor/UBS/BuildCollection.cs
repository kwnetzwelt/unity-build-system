using System;
using System.Collections.Generic;
using UnityEngine;

namespace UBS
{
	[Serializable]
	public class BuildCollection : ScriptableObject
	{
		public BuildCollection ()
		{
			version = BuildVersion.Load();

		}

		public List<BuildProcess> mProcesses = new List<BuildProcess>();

		public void SaveVersion()
		{
			version.Save();
			UnityEditor.PlayerSettings.Android.bundleVersionCode = version.revision;
			UnityEditor.PlayerSettings.bundleVersion = version.ToString();
			UnityEditor.PlayerSettings.Metro.packageVersion = version;

		}

		public BuildVersion version = null;
		public string versionCode = "1";

		public void SaveVersionCode()
		{
			UnityEditor.PlayerSettings.Android.bundleVersionCode = int.Parse(versionCode);
		}
	}
}

