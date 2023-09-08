using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using UnityEngine.Serialization;

namespace UBS {
	[Serializable]
	public class BuildProcess {

		public BuildProcess()
		{

		}
		public BuildProcess(BuildProcess other)
		{
			foreach(var bs in other.PreBuildSteps)
			{
				PreBuildSteps.Add( new BuildStep(bs) );
			}

			
			foreach(var bs in other.PostBuildSteps)
			{
				PostBuildSteps.Add( new BuildStep(bs) );
			}

			Name = other.Name;
			OutputPath = other.OutputPath;
			Platform = other.Platform;
			Options = other.Options;
			Selected = false;
			ScriptingDefines = other.ScriptingDefines;
			SceneAssets = new List<SceneAsset>( other.SceneAssets );
		}


		#region data

		/// <summary>
		/// If true, uses scene collection set in EditorBuildSettings instead of custom scene collection in the process
		/// </summary>
		[field:SerializeField()]
		public bool UseEditorScenes { get; set; } = false;

		
        [field: FormerlySerializedAs("mPreBuildSteps")]
        [field:SerializeField()]
        public List<BuildStep> PreBuildSteps { get; private set; } = new List<BuildStep>();

        [field: FormerlySerializedAs("mPostBuildSteps")]
        [field:SerializeField()]
        public List<BuildStep> PostBuildSteps { get; private set;} = new List<BuildStep>();

        [field: FormerlySerializedAs("mName")]
        [field:SerializeField()]
        public string Name { get; set; } = "Build Process";

        [field: FormerlySerializedAs("mOutputPath")]
        [field:SerializeField()]
        public string OutputPath { get; set; } = "";

        /// <summary>
        /// If pretend mode is on, the process will not actually trigger a build. It will do everything else though. 
        /// </summary>
        [field: FormerlySerializedAs("mPretend")]
        [field:SerializeField()]
        public bool Pretend { get; set; } = false;

        [field: FormerlySerializedAs("mPlatform")]
        [field:SerializeField()]
        public BuildTarget Platform { get; set; }

        [field: FormerlySerializedAs("mBuildOptions")]
        [field:SerializeField()]
        public BuildOptions Options { get; set; }


        [field: FormerlySerializedAs("mSelected")]
        [field:SerializeField()]
        public bool Selected { get; set; }

        [field: FormerlySerializedAs("mSceneAssets")]
        [field:SerializeField()]
        public List<SceneAsset> SceneAssets { get; private set;} = new List<SceneAsset>();

        [field:SerializeField] 
        public List<string> ScriptingDefines { get; protected set; } = new();

        #endregion
	}
}