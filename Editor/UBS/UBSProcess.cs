using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Editor.UBS.Commandline;
using UnityEditor.Build.Reporting;
using UnityEngine.Serialization;

namespace UBS
{
    [Serializable]
    public class UBSProcess : ScriptableObject
    {
        public const string ProcessPath = "Assets/UBSProcess.asset";
        const string ProcessPathKey = "UBSProcessPath";


		#region data

		public UBSProcessConfiguration config;

        [SerializeField] BuildConfiguration currentBuildConfiguration;
        BuildConfiguration CurrentBuildConfiguration
        {
            get { return currentBuildConfiguration;}

        }

        public bool IsInBatchMode
        {
            get { return config.BatchMode; }
        }

        public BuildCollection BuildCollection
        {
            get { return config.Collection; }
        }


        [SerializeField]
        int
            currentBuildProcessIndex;

        [SerializeField]
        UBSState
            currentState = UBSState.invalid;

        [SerializeField]
        UBSStepListWalker
            preStepWalker = new UBSStepListWalker();

        [SerializeField]
        UBSStepListWalker
            postStepWalker = new UBSStepListWalker();

        public UBSState CurrentState
        {
            get { return currentState; }
        }

        public UBSStepListWalker SubPreWalker
        {
            get
            {
                return preStepWalker;
            }
        }

        public UBSStepListWalker SubPostWalker
        {
            get
            {
                return postStepWalker;
            }
        }

        public float Progress
        {
            get
            {

                return ((SubPreWalker.Progress + SubPostWalker.Progress) / 2.0f
                    + Math.Max(0, currentBuildProcessIndex - 1)) / config.SelectedBuildProcesses.Count;
            }
        }

        public string CurrentProcessName
        {
            get
            {
                if(CurrentProcess != null)
                {
                    return CurrentProcess.Name;
                }
                return "N/A";
            }
        }

        BuildProcess CurrentProcess
        {
            get
            {
                if(config.SelectedBuildProcesses == null || currentBuildProcessIndex >= config.SelectedBuildProcesses.Count)
                {
                    return null;
                }
                return config.SelectedBuildProcesses[currentBuildProcessIndex];
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
            return EditorPrefs.GetString(ProcessPathKey, ProcessPath);
        }
        /// <summary>
        /// You can overwrite where to store the build process. 
        /// </summary>
        /// <param name="pPath">P path.</param>
        public static void SetProcessPath(string pPath)
        {
            EditorPrefs.GetString(ProcessPathKey, pPath);
        }

		#region command line options


		
		/// <summary>
        /// Builds a given build collection from command line. Call this method directly from the command line using Unity in headless mode. 
        /// https://docs.unity3d.com/Documentation/Manual/CommandLineArguments.html
        /// <br /><br />
        /// Provide `collection` parameter to your command line build to specify the collection you want to build. 
        /// All selected build processes within the collection will be build.
        /// <br /><br />
        /// Provide `buildProcessByNames` parameter with a comma separated list of buildprocesses that are present
        /// in your collection. Only these build processes will be built. Note that the names are cast to lowercase
        /// for matching.
        /// <br /><br />
        /// Alternatively provide `buildAll` which will build all buildprocesses within a collection no matter
        /// if they were selected in the editor. 
        /// <br /><br />
        /// Provide `buildTag` parameter to add a string to the output name of your builds.
        /// <br /><br />
        /// Provide `commitID` parameter. It will be stored in Editorprefs for use in Buildsteps. 
        /// <br /><br />
        /// Provide `tagName` parameter. It will be stored in Editorprefs for use in Buildsteps. 
        /// <br /><br />
        /// Provide `clean`or `noclean` parameter to either force a clean build or prevent a clean build. If no parameter is found 
        /// Example: -batchmode  -collection "Assets/New\ BuildCollection.asset" -buildProcessByNames "Android,iOS"
        /// <br /><br />
        /// Other Arguments: android-sdk, android-ndk, jdk-path<br />
        /// </summary>
        /// <example>/Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -projectPath ~/UnityProjects/MyProject -collection "Assets/New\ BuildCollection.asset" -buildProcessByNames "Android,iOS" -executeMethod UBS.UBSProcess.BuildFromCommandLine</example>
		public static void BuildFromCommandLine()
		{

			string[] arguments = Environment.GetCommandLineArgs();
			BuildFromCommandLine(arguments);
		}

		/// <inheritdoc cref="BuildFromCommandLine()"/>
		/// <param name="args">Commandline Parameters as provided by Evironment.GetCommandLineArgs</param>
		/// <exception cref="Exception"></exception>
        public static void BuildFromCommandLine(string[] args)
        {
	        CommandLineArgsParser parser = new CommandLineArgsParser(args);
            foreach (var argument in parser.Collection.Arguments)
            {
	            UnityEngine.Debug.Log(argument.Name + " -> " + argument.Value);  
            }

            bool batchMode = parser.Collection.HasArgument("batchmode");
            CleanBuildArgument clean = CleanBuildArgument.NotAssigned;
            if(parser.Collection.HasArgument("clean"))
            {
	            clean = CleanBuildArgument.Clean;
            }else if (parser.Collection.HasArgument("noclean"))
            {
	            clean = CleanBuildArgument.NoClean;
            }

            string collectionPath = parser.Collection.GetValue("collection");
			string androidSdkPath = parser.Collection.GetValue("android-sdk");
			string buildTag = parser.Collection.GetValue("buildTag");            
            string commitID = parser.Collection.GetValue("commitID") ;
			string tagName = parser.Collection.GetValue("tagName");
			string androidNdkPath = parser.Collection.GetValue("android-ndk");
			string jdkPath = parser.Collection.GetValue("jdk-path");
            bool buildAll = parser.Collection.HasArgument("buildAll");
            
            string startBuildProcessByNames = parser.Collection.GetValue("buildProcessByNames");
			
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

            if (!string.IsNullOrEmpty(androidNdkPath))
            {
                EditorPrefs.SetString("AndroidNdkRoot", androidNdkPath);
                Debug.Log("Set Android NDK root to: " + androidNdkPath);
            }

            if (!string.IsNullOrEmpty(jdkPath))
            {
                EditorPrefs.SetString("JdkPath", jdkPath);
                Debug.Log("Set JDK-Path root to: " + jdkPath);
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

			UBSProcessConfiguration config = new UBSProcessConfiguration()
			{
				Collection = collection,
				BuildAndRun = false,
				BatchMode = batchMode,
				BuildAll = buildAll,
				BuildTag = buildTag,
				Clean = clean,
				CommandLineArgs = parser.Collection
			};
			
            if (!String.IsNullOrEmpty(startBuildProcessByNames))
            {
                string[] buildProcessNameList = startBuildProcessByNames.Split(',');
                config.SelectedBuildProcessNames.AddRange(buildProcessNameList);
            }
	        CreateFromConfig(config);
            
			
			
			UBSProcess process = LoadUBSProcess();

			try
			{
				while(true)
				{
					process.MoveNext();
					switch (process.CurrentState)
					{
						case UBSState.done:
							return;
						case UBSState.aborted:
							throw new Exception("Build aborted");
					}
				}
			}catch(Exception pException)
			{
				Debug.LogError("Build failed due to exception: ");
				Debug.LogException(pException);
				if(Application.isBatchMode)
					EditorApplication.Exit(1);
			}
		}

		public static string AddBuildTag(string outputPath, string tag)
		{
			List<string> splittedPath = new List<string>(outputPath.Split('/'));

			if(splittedPath[splittedPath.Count - 1].Contains("."))
			{

				splittedPath.Insert(splittedPath.Count - 2, tag);
			}
			else
			{
				splittedPath.Add(tag);
			}

			splittedPath.RemoveAll((str) => {
				return string.IsNullOrEmpty(str);
			});

			return string.Join("/", splittedPath.ToArray());
		}

#endregion

		public static void CreateFromCollection(BuildCollection pCollection, bool pBuildAndRun)
		{
			var config = new UBSProcessConfiguration()
			{
				BuildAndRun = pBuildAndRun,
				Collection = pCollection
			};
			CreateFromConfig(config);
		}
		public static void CreateFromConfig(UBSProcessConfiguration config)
		{
			UBSProcess p = CreateInstance<UBSProcess>();
			
			
			// clean build
			if(config.Clean != CleanBuildArgument.NotAssigned)
				config.Collection.cleanBuild = config.Clean == CleanBuildArgument.Clean;
			
			// build all or only selected ones?
			if(!config.BuildAll)
			{
				config.SelectedBuildProcesses= config.Collection.Processes.FindAll( obj => obj.Selected );
			}
			else if (config.SelectedBuildProcessNames.Count > 0)
			{
				var lowerCaseTrimmedBuildProcessNameList = config.SelectedBuildProcessNames.Select(x => x.ToLower()).Select(x => x.Trim()).ToArray();

				var selectedProcesses = config.Collection.Processes
					.Where(buildProcess => lowerCaseTrimmedBuildProcessNameList.Contains(buildProcess.Name.ToLower())).ToList();
				config.SelectedBuildProcesses = selectedProcesses;
			}
			else
			{
				config.SelectedBuildProcesses = config.Collection.Processes;
			}
			
			// add a tag to all the outputpaths
			if(!string.IsNullOrEmpty(config.BuildTag))
			{
				foreach(var sp in config.SelectedBuildProcesses)
				{
					sp.OutputPath = AddBuildTag(sp.OutputPath, config.BuildTag);
				}
			}
			
			// set up logging
			config.Collection.ActivateLogTypes();
			
			// set initial state to kick things off
			p.currentState = UBSState.invalid;


			// add config to the ubsprocess
			p.config = config;
			
			AssetDatabase.CreateAsset( p, GetProcessPath());
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
			}
			if(IsDone)
				OnDone();
		}
		
		public void Cancel(string pMessage)
		{
			if(pMessage.Length > 0 && !IsInBatchMode)
			{
				Debug.LogError("UBS: " + pMessage);
				EditorUtility.DisplayDialog("UBS: Error occured!", pMessage, "Ok - my fault.");
			}
			Cancel();
		}

		public void Cancel()
		{
			Debug.LogError( $"UBS: Build Process \"{currentBuildConfiguration.GetCurrentBuildProcess().Name}\" was cancelled. ");

			SetState(UBSState.aborted);
			
			
			preStepWalker.Clear();
			postStepWalker.Clear();
			
			config.Collection.RestoreLogTypes();

			if (IsInBatchMode)
			{
				EditorApplication.Exit(1);
			}
		}

		#endregion


		#region build process state handling

		void SetState(UBSState newState)
		{
			
			if (currentState != newState)
			{
				currentState = newState;
				Save();
			}
		}
		
		void OnDone()
		{
			if(IsInBatchMode && currentState == UBSState.aborted)
				EditorApplication.Exit(1);
			
			config.Collection.RestoreLogTypes();
			SetState(UBSState.done);
			Debug.Log("UBSProcess is done. ");
		}
		void NextBuild()
		{
			if(currentBuildProcessIndex >= config.SelectedBuildProcesses.Count)
			{
				SetState(UBSState.done);
			}else
			{
				SetState(UBSState.setup);
			}
		}

		void DoSetup()
		{
			currentBuildConfiguration = new BuildConfiguration();
            currentBuildConfiguration.Initialize();

			if(!CheckOutputPath(CurrentProcess))
				return;

			preStepWalker.Init( CurrentProcess.PreBuildSteps, currentBuildConfiguration );

			postStepWalker.Init(CurrentProcess.PostBuildSteps, currentBuildConfiguration );

			SetState(UBSState.preSteps);
		}

		void DoPreSteps()
		{
			preStepWalker.MoveNext();

			if(currentState == UBSState.aborted)
				return;

			if(preStepWalker.IsDone())
			{
				preStepWalker.End();

				SetState(UBSState.building);
			}
			Save();
		}

		void DoBuilding()
		{
            
			BuildOptions bo = CurrentProcess.Options;
			if(CurrentBuildConfiguration.GetCurrentBuildCollection().cleanBuild)
				bo &= BuildOptions.CleanBuildCache;
			
			if (config.BuildAndRun)
				bo |= BuildOptions.AutoRunPlayer;

			if (!CurrentProcess.Pretend)
			{
				var scenePaths = GetScenePaths();
				
				BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
				{
					scenes = scenePaths.ToArray(),
					locationPathName = CurrentProcess.OutputPath,
					target = CurrentProcess.Platform,
					options = bo,
					extraScriptingDefines = CurrentProcess.ScriptingDefines.ToArray()
				};
				BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
				config.Collection.ActivateLogTypes();
				Debug.Log("Playerbuild Result: " + report.summary.result);
				if (report.summary.result != BuildResult.Succeeded)
				{
					Cancel("Build failed with result: " + report.summary.result);
					return;
				}
			}

			OnBuildDone ();
		}

		private List<string> GetScenePaths()
		{
			var output = new List<string>();
			if (CurrentProcess.UseEditorScenes)
			{
				foreach (var sceneAsset in EditorBuildSettings.scenes)
				{
					if(!sceneAsset.enabled)
						continue;
					if (ReferenceEquals(null, sceneAsset.path))
						continue;
					if (string.IsNullOrEmpty(sceneAsset.path))
						continue;
					output.Add(sceneAsset.path);
				}
			}
			else
			{
				foreach (var sceneAsset in CurrentProcess.SceneAssets)
				{
					if (ReferenceEquals(null, sceneAsset))
						continue;
					var scenePath = AssetDatabase.GetAssetPath(sceneAsset);
					if (string.IsNullOrEmpty(scenePath))
						continue;
					output.Add(scenePath);
				}
			}

			return output;

		}

		void OnBuildDone() 
		{
			SetState(UBSState.postSteps);
		}
        
		void DoPostSteps()
		{
			postStepWalker.MoveNext();
			
			if(currentState == UBSState.aborted)
				return;

			if(postStepWalker.IsDone())
			{
				postStepWalker.End();
				Debug.Log($"Build Process \"{ postStepWalker.Configuration.GetCurrentBuildProcess().Name}\" is done. ");
				// this is invalid instead of done as we first need to check
				// if there is another build process to run.
				SetState(UBSState.invalid); 
				currentBuildProcessIndex++;
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
			
			
			if(pProcess.OutputPath.Length == 0) {
				error = "Please provide an output path.";
				Cancel(error);
				return false;
			}
			
			try
			{
				DirectoryInfo dir;
				if(pProcess.Platform == BuildTarget.Android
				   || pProcess.Platform == BuildTarget.StandaloneWindows
				   || pProcess.Platform == BuildTarget.StandaloneWindows64
				   || pProcess.Platform == BuildTarget.StandaloneLinux64)
					dir = new DirectoryInfo(Path.GetDirectoryName(UBS.Helpers.GetAbsolutePathRelativeToProject( pProcess.OutputPath )));
				else
					dir = new DirectoryInfo(pProcess.OutputPath);
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

    public enum CleanBuildArgument
    {
	    NotAssigned,
	    Clean,
	    NoClean
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


	[BuildStepTypeFilter(BuildStepType.invalid)]
	public class SkipBuildStepEntry : IBuildStepProvider
	{
		public void BuildStepStart(BuildConfiguration configuration)
		{
			
		}

		public void BuildStepUpdate()
		{
			
		}

		public bool IsBuildStepDone()
		{
			return true;
		}
	}

}

