using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UBS {

	public class BuildProcess {
		#region data

		public List<UBS.BuildStep> mPreBuildSteps = new List<BuildStep>();
		
		public List<UBS.BuildStep> mPostBuildSteps = new List<BuildStep>();

		public string mName = "Build Step";

		public UnityEngine.RuntimePlatform mPlatform;

		#endregion

	}
}