using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

namespace UBS {
	[Serializable]
	public class BuildProcess {

		public BuildProcess()
		{

		}
		public BuildProcess(BuildProcess pOther)
		{
			foreach(var bs in pOther.mPreBuildSteps)
			{
				mPreBuildSteps.Add( new BuildStep(bs) );
			}

			
			foreach(var bs in pOther.mPostBuildSteps)
			{
				mPostBuildSteps.Add( new BuildStep(bs) );
			}

			mName = pOther.mName;
			mOutputPath = pOther.mOutputPath;
			mPlatform = pOther.mPlatform;
			mBuildOptions = pOther.mBuildOptions;
			mSelected = false;
			mScenes = new List<string>( pOther.mScenes.ToArray() );
			mSceneAssets = new List<SceneAsset>( pOther.mSceneAssets.ToArray() );
		}

		#region data
		public List<UBS.BuildStep> mPreBuildSteps = new List<BuildStep>();

		public List<UBS.BuildStep> mPostBuildSteps = new List<BuildStep>();

		public string mName = "Build Process";

		public string mOutputPath = "";

		/// <summary>
		/// If pretend mode is on, the process will not actually trigger a build. It will do everything else though. 
		/// </summary>
		public bool mPretend = false;

		public BuildTarget mPlatform;

		public BuildOptions mBuildOptions;



		public bool mSelected;
		
		public List<string> mScenes = new List<string>();

		public List<SceneAsset> mSceneAssets = new List<SceneAsset>();
		#endregion



	}
}