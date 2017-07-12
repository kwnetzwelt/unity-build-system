using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UBS
{
    [Serializable]
    public class UBSProcess : ScriptableObject
    {
        const string kProcessPath = "Assets/UBSProcess.asset";
        const string kProcessPathKey = "UBSProcessPath";


		#region data
        

        [SerializeField]
        BuildConfiguration
            mCurrentBuildConfiguration;
        BuildConfiguration CurrentBuildConfiguration
        {
            get { return mCurrentBuildConfiguration;}

        }

        [SerializeField]
        bool
            mBuildAndRun;

        [SerializeField]
        bool
            mBatchMode;
        public bool IsInBatchMode
        {
            get { return mBatchMode; }
        }

        [SerializeField]
        BuildCollection
            mCollection;
        public BuildCollection BuildCollection
        {
            get { return mCollection; }
        }

        [SerializeField]
        List<BuildProcess>
            mSelectedProcesses;


        [SerializeField]
        int
            mCurrentBuildProcessIndex;

        [SerializeField]
        int
            mCurrent;

        [SerializeField]
        UBSState
            mCurrentState = UBSState.invalid;

        [SerializeField]
        UBSStepListWalker
            mPreStepWalker = new UBSStepListWalker();

        [SerializeField]
        UBSStepListWalker
            mPostStepWalker = new UBSStepListWalker();

        public UBSState CurrentState
        {
            get { return mCurrentState; }
        }

        public UBSStepListWalker SubPreWalker
        {
            get
            {
                return mPreStepWalker;
            }
        }

        public UBSStepListWalker SubPostWalker
        {
            get
            {
                return mPostStepWalker;
            }
        }

        public float Progress
        {
            get
            {

                return ((SubPreWalker.Progress + SubPostWalker.Progress) / 2.0f
                    + System.Math.Max(0, mCurrentBuildProcessIndex - 1)) / (float)mSelectedProcesses.Count;
            }
        }

        public string CurrentProcessName
        {
            get
            {
                if(CurrentProcess != null)
                {
                    return CurrentProcess.mName;
                }
                return "N/A";
            }
        }

        BuildProcess CurrentProcess
        {
            get
            {
                if(mSelectedProcesses == null || mCurrentBuildProcessIndex >= mSelectedProcesses.Count)
                {
                    return null;
                }
                return mSelectedProcesses[mCurrentBuildProcessIndex];
            }
        }

        public bool IsDone { get
            {
                return CurrentState == UBSState.done || CurrentState == UBSState.aborted;
            }
        }

        #endregion

        #region public interface

        public BuildProcess GetCurrentProcess()
        {
            return CurrentProcess;
        }

        public static string GetProcessPath()
        {
            return EditorPrefs.GetString(kProcessPathKey, kProcessPath);
        }
        /// <summary>
        /// You can overwrite where to store the build process. 
        /// </summary>
        /// <param name="pPath">P path.</param>
        public static void SetProcessPath(string pPath)
        {
            EditorPrefs.GetString(kProcessPathKey, kProcessPath);
        }

		#region command line options

        /// <summary>
        /// Builds a given build collection from command line. Call this method directly from the command line using Unity in headless mode. 
        /// <https://docs.unity3d.com/Documentation/Manual/CommandLineArguments.html>
        /// 
        /// Provide `collection` parameter to your command line build to specify the collection you want to build. 
        /// All selected build processes within the collection will be build. 
        /// 
        /// Example: -collection=Assets/New\ BuildCollection.asset
        /// </summary>
        public static void BuildFromCommandLine()
        {
            bool batchMode = false;

            string[] arguments = System.Environment.GetCommandLineArgs();
            string[] availableArgs = { "-batchmode", "-collection=", "-android-sdk=", "-buildTag=", "-buildAll", "-commitID=", "-tagName=", "-buildProcessByNames=" };
			string collectionPath = "";
			string androidSdkPath = "";
			string buildTag = "";            
            string commitID = "" ;
            string tagName = "";
            bool buildAll = false;
            string startBuildProcessByNames = String.Empty;
			foreach(var s in arguments)
			{
				if(s.StartsWith("-batchmode"))
				{
					batchMode = true;
					Debug.Log("UBS process started in batchmode!");
				}

				if(s.StartsWith("-collection="))
				{
					collectionPath = s.Substring(availableArgs[1].Length);
				}

				if(s.StartsWith("-android-sdk="))
				{
					androidSdkPath = s.Substring(availableArgs[2].Length);
				}

				if(s.StartsWith("-buildTag="))
				{
					buildTag = s.Substring(availableArgs[3].Length);
				}

				if(s.StartsWith("-buildAll"))
				{
					buildAll = true;
					Debug.Log("Selection override: building whole collection!");
				}
				
                if(s.StartsWith("-commitID="))
                {
                    commitID = s.Substring(availableArgs[5].Length);
                    
                }
                
                if(s.StartsWith("-tagName="))
                {
                    tagName = s.Substring(availableArgs[6].Length);
                }

                if (s.StartsWith("-buildProcessByNames="))
                {
                    startBuildProcessByNames = s.Substring(availableArgs[7].Length);
                }
			}
			if(collectionPath == null)
			{
				Debug.LogError("NO BUILD COLLECTION SET");
				return;
			}
			
			if(!string.IsNullOrEmpty(androidSdkPath))
			{
				EditorPrefs.SetString("AndroidSdkRoot", androidSdkPath);
				Debug.Log("Set Android SDK root to: " + androidSdkPath);
			}
            
            if(!string.IsNullOrEmpty(commitID))
            {
                EditorPrefs.SetString("commitID", commitID);
                Debug.Log("Set commitID to: " + commitID);
            }
            
            if(!string.IsNullOrEmpty(tagName))
            {
                EditorPrefs.SetString("tagName", tagName);
                Debug.Log("Set tagName to: " + tagName);
            }

			Debug.Log("Loading Build Collection: " + collectionPath);

			// Load Build Collection
			BuildCollection collection = AssetDatabase.LoadAssetAtPath(collectionPath, typeof(BuildCollection)) as BuildCollection;
			// Run Create Command

            if (!String.IsNullOrEmpty(startBuildProcessByNames))
            {
                string[] buildProcessNameList = startBuildProcessByNames.Split(',');
                var lowerCaseTrimmedBuildProcessNameList = buildProcessNameList.Select(x => x.ToLower()).Select(x => x.Trim()).ToArray();
                Create(collection, false, lowerCaseTrimmedBuildProcessNameList, batchMode, buildTag);
            }
            else
            {
                Create(collection, false, buildAll, batchMode, buildTag);
            }
			
			
			UBSProcess process = LoadUBSProcess();

			try
			{
				while(true)
				{
					process.MoveNext();
					Debug.Log("Wait..");
					Debug.Log ("Process state: " + process.CurrentState);
					if(process.CurrentState == UBSState.done)
					{
						return;
					}
				}
			}catch(Exception pException)
			{
				Debug.LogError("Build failed due to exception: ");
				Debug.LogException(pException);
				EditorApplication.Exit(1);
			}
		}

		public static string AddBuildTag(string pOutputPath, string pTag)
		{
			List<string> splittedPath = new List<string>(pOutputPath.Split('/'));

			if(splittedPath[splittedPath.Count - 1].Contains("."))
			{

				splittedPath.Insert(splittedPath.Count - 2, pTag);
			}
			else
			{
				splittedPath.Add(pTag);
			}

			splittedPath.RemoveAll((str) => {
				return string.IsNullOrEmpty(str);
			});

			return string.Join("/", splittedPath.ToArray());
		}

#endregion

		public static void Create(BuildCollection pCollection, bool pBuildAndRun, bool pBatchMode = false, bool pBuildAll = false, string pBuildTag = "")
		{
			UBSProcess p = ScriptableObject.CreateInstance<UBSProcess>();
			p.mBuildAndRun = pBuildAndRun;
			p.mBatchMode = pBatchMode;
			p.mCollection = pCollection;
			if(!pBuildAll)
			{
				p.mSelectedProcesses = p.mCollection.mProcesses.FindAll( obj => obj.mSelected );
			}
			else
			{
				p.mSelectedProcesses = p.mCollection.mProcesses;
			}
			p.mCurrentState = UBSState.invalid;

			if(!string.IsNullOrEmpty(pBuildTag))
			{
				foreach(var sp in p.mSelectedProcesses)
				{
					sp.mOutputPath = AddBuildTag(sp.mOutputPath, pBuildTag);
				}
			}

			AssetDatabase.CreateAsset( p, GetProcessPath());
			AssetDatabase.SaveAssets();
		}

        /// <summary>
        /// Builds a buildcollection by using an array of build process names (',' seperated!)
        /// By using a list of build process names, we reconfigure and retarget the actual build collection.
        /// </summary>
        public static void Create(BuildCollection pCollection, bool pBuildAndRun, string[] toBeBuildedNames, bool pBatchMode = false, string pBuildTag = "")
        {
            UBSProcess p = ScriptableObject.CreateInstance<UBSProcess>();
            p.mBuildAndRun = pBuildAndRun;
            p.mBatchMode = pBatchMode;
            p.mCollection = pCollection;
            if (toBeBuildedNames != null && toBeBuildedNames.Length > 0)
            {
                var selectedProcesses = p.mCollection.mProcesses
                    .Where(buildProcess => toBeBuildedNames.Contains(buildProcess.mName.ToLower())).ToList();
                p.mSelectedProcesses = selectedProcesses;
            }
            else
            {
                p.mSelectedProcesses = p.mCollection.mProcesses;
            }
            p.mCurrentState = UBSState.invalid;

            if (!string.IsNullOrEmpty(pBuildTag))
            {
                foreach (var sp in p.mSelectedProcesses)
                {
                    sp.mOutputPath = AddBuildTag(sp.mOutputPath, pBuildTag);
                }
            }

            AssetDatabase.CreateAsset(p, GetProcessPath());
            AssetDatabase.SaveAssets();
        }

		public static bool IsUBSProcessRunning()
		{
			var asset = AssetDatabase.LoadAssetAtPath( GetProcessPath(), typeof(UBSProcess) );
			return asset != null;
		}
		public static UBSProcess LoadUBSProcess()
		{
			var process = AssetDatabase.LoadAssetAtPath( GetProcessPath(), typeof(UBSProcess));
			return process as UBSProcess;
		}
		

		public void MoveNext()
		{

			switch(CurrentState)
			{
				case UBSState.setup: DoSetup(); break;
				case UBSState.preSteps: DoPreSteps(); break;
				case UBSState.building: DoBuilding(); break;
				case UBSState.postSteps: DoPostSteps(); break;
				case UBSState.invalid: NextBuild(); break;
				case UBSState.done: OnDone(); break;
			}
		}
		
		public void Cancel(string pMessage)
		{
			if(pMessage.Length > 0)
			{
				EditorUtility.DisplayDialog("UBS: Error occured!", pMessage, "Ok - my fault.");
			}
			Cancel();
		}

		public void Cancel()
		{
			mCurrentState = UBSState.aborted;
			mPreStepWalker.Clear();
			mPostStepWalker.Clear();
			Save();
		}

		#endregion


		#region build process state handling
		void OnDone()
		{

		}
		void NextBuild()
		{

			if(mCurrentBuildProcessIndex >= mSelectedProcesses.Count)
			{
				mCurrentState = UBSState.done;
				Save();
			}else
			{
				mCurrentState = UBSState.setup;
				Save ();
			}
		}

		void DoSetup()
		{
			mCurrentBuildConfiguration = new BuildConfiguration();
            mCurrentBuildConfiguration.Initialize();

			if(!CheckOutputPath(CurrentProcess))
				return;

			if (!EditorUserBuildSettings.SwitchActiveBuildTarget (CurrentProcess.mPlatform)) {
                Cancel();
				throw new Exception("Could not switch to build target: " + CurrentProcess.mPlatform);
			}
			
			var scenes = new EditorBuildSettingsScene[CurrentProcess.mScenes.Count];
			for(int i = 0;i< scenes.Length;i++)
			{
				EditorBuildSettingsScene ebss = new EditorBuildSettingsScene( CurrentProcess.mScenes[i] ,true );
				scenes[i] = ebss;
			}
			EditorBuildSettings.scenes = scenes;


			mPreStepWalker.Init( CurrentProcess.mPreBuildSteps, mCurrentBuildConfiguration );

			mPostStepWalker.Init(CurrentProcess.mPostBuildSteps, mCurrentBuildConfiguration );

			mCurrentState = UBSState.preSteps;
			
			Save();
		}

		void DoPreSteps()
		{
			mPreStepWalker.MoveNext();

			if(mCurrentState == UBSState.aborted)
				return;

			if(mPreStepWalker.IsDone())
			{
				mCurrentState = UBSState.building;
			}
			Save();
		}

		void DoBuilding()
		{
            
			List<string> scenes = new List<string>();

			foreach(var scn in EditorBuildSettings.scenes)
			{
				if(scn.enabled)
					scenes.Add(scn.path);
			}
			BuildOptions bo = CurrentProcess.mBuildOptions;
			if(mBuildAndRun)
				bo = bo | BuildOptions.AutoRunPlayer;

			if(!CurrentProcess.mPretend)
			{
				BuildPipeline.BuildPlayer(
					scenes.ToArray(),
					CurrentProcess.mOutputPath,
					CurrentProcess.mPlatform,
					bo );
			}

			OnBuildDone ();
		}

		void OnBuildDone() 
		{
			mCurrentState = UBSState.postSteps;
			Save();
		}
        
		void DoPostSteps()
		{
			mPostStepWalker.MoveNext();
			
			if(mCurrentState == UBSState.aborted)
				return;

			if(mPostStepWalker.IsDone())
			{
				mCurrentState = UBSState.invalid;
				mCurrentBuildProcessIndex++;
			}
			Save();
		}
		#endregion

		void Save()
		{
			if(this != null) {
				EditorUtility.SetDirty(this);
				AssetDatabase.SaveAssets();
			}
		}

		bool CheckOutputPath(BuildProcess pProcess)
		{
			string error = "";
			
			
			if(pProcess.mOutputPath.Length == 0) {
				error = "Please provide an output path.";
				Cancel(error);
				return false;
			}
			
			try
			{
				DirectoryInfo dir;
				if(pProcess.mPlatform == BuildTarget.Android
				   || pProcess.mPlatform == BuildTarget.StandaloneWindows
				   || pProcess.mPlatform == BuildTarget.StandaloneWindows64)
					dir = new DirectoryInfo(Path.GetDirectoryName(UBS.Helpers.GetAbsolutePathRelativeToProject( pProcess.mOutputPath )));
				else
					dir = new DirectoryInfo(pProcess.mOutputPath);
				dir.Create();

				if(!dir.Exists)
					error = "The given output path is invalid.";
			}
			catch (Exception e)
			{
				error = e.ToString();
			}
			
			if(error.Length > 0)
			{
				Cancel(error);
				return false;
			}
			return true;

		}
	}
    
	public enum UBSState
	{
		invalid,
		setup,
		preSteps,
		building,
		postSteps,
		done,
		aborted
	}



	[Serializable]
	public class UBSStepListWalker
	{
		[SerializeField]
		int mIndex = 0;
		[SerializeField]
		List<BuildStep> mSteps;


		IBuildStepProvider mCurrentStep;
		
		[SerializeField]
		BuildConfiguration mConfiguration;
		
		public UBSStepListWalker()
		{

		}

		public void Init ( List<BuildStep> pSteps, BuildConfiguration pConfiguration)
		{
			mIndex = 0;
			mSteps = pSteps;
			mConfiguration = pConfiguration;
		}
		public void Clear()
		{
			mIndex = 0;
			mSteps = null;
			mConfiguration = null;
		}

		public void MoveNext()
		{
			if(mCurrentStep == null || mCurrentStep.IsBuildStepDone())
			{
				NextStep();
			}else
			{
				mCurrentStep.BuildStepUpdate();
			}
		}
		
		void NextStep()
		{
			if(mSteps == null)
				return;
			if(IsDone())
			{
				return;
			}

			if(mCurrentStep != null)
				mIndex++;

			if(mIndex >= mSteps.Count)
				return;

			mSteps[mIndex].InferType();

			if (mSteps [mIndex].mType != null) 
			{
				mCurrentStep = System.Activator.CreateInstance( mSteps[mIndex].mType ) as IBuildStepProvider;
				mConfiguration.SetParams( mSteps[mIndex].mParams );
			} 
			else 
			{
				mCurrentStep = new EmptyBuildStep();
			}			

			mCurrentStep.BuildStepStart(mConfiguration);			
		}

		public bool IsDone()
		{
			if(mSteps != null)
				return mIndex == mSteps.Count;
			else
				return false;
		}

		public float Count {
			get
			{
				if(mSteps == null || mSteps.Count == 0)
					return 0;
				return (float)mSteps.Count;
			}
		}

		public float Progress
		{
			get
			{
				if(mIndex == 0 && Count == 0)
					return 1;
				return mIndex / Count;
			}
		}
		public string Step
		{
			get
			{
				if(mSteps == null || mIndex >= mSteps.Count)
					return "N/A";

				return mSteps[mIndex].mTypeName;
			}
		}
	}



}

