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
		}

		public List<BuildProcess> mProcesses = new List<BuildProcess>();
		
		string mVersionName = "1.0.0";
		string mVersionCode = "1";
		public string versionName 
		{
			set{ mVersionName = value; UnityEditor.PlayerSettings.bundleVersion = mVersionName; }
			get{ return mVersionName; }
		}
		public string versionCode
		{
			set 
			{
					mVersionCode = value;
					int n = 0;
					if(Int32.TryParse(mVersionCode, out n))
						UnityEditor.PlayerSettings.Android.bundleVersionCode = n;
			}
			get { return mVersionCode; }
		}

	}
}

