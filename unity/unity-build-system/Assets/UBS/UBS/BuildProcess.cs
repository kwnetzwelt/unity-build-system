using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace UBS {
	[Serializable]
	public class BuildProcess {
		#region data
		public List<UBS.BuildStep> mPreBuildSteps = new List<BuildStep>();

		public List<UBS.BuildStep> mPostBuildSteps = new List<BuildStep>();

		public string mName = "Build Step";

		public string mPlatform;

		public string mSubPlatform;

		public bool mSelected;
		
		public List<string> mScenes = new List<string>();

		public List<UnityEngine.Object> mSceneAssets = new List<UnityEngine.Object>();
		#endregion



	}
}