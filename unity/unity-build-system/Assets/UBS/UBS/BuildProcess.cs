using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UBS {

	public class BuildProcess : ScriptableObject {
		#region data

		public List<UBS.BuildStep> mPreBuildSteps = new List<BuildStep>();
		
		public List<UBS.BuildStep> mPostBuildSteps = new List<BuildStep>();

		public string mName = "Build Step";

		public string mPlatform;
		public string mSubPlatform;


		public List<string> mScenes = new List<string>();
		public List<UnityEngine.Object> mSceneAssets = new List<Object>();
		#endregion



	}
}