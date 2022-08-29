using System;
using UnityEngine;
using System.IO;
using UnityEngine.Serialization;


namespace UBS
{
	[Serializable]
	public class BuildConfiguration
	{
		public void Initialize ()
		{
			DirectoryInfo di = new DirectoryInfo( Application.dataPath );

			AssetsDirectory = di.FullName;
			ResourcesDirectory = Path.Combine( AssetsDirectory , "Resources");
			ProjectDirectory = di.Parent.FullName;
		}

		/// <summary>
		/// Parameters can be set in the Build Collection editor for each build step. Your build step can read the parameters from the collection. 
		/// </summary>
		/// <value>The parameters.</value>
		public BuildStep.BuildStepParameters Parameters {
			get;
			private set;
		}
		
		public bool ToggleValue { get; private set; }

		internal void SetParams(BuildStep.BuildStepParameters parameters)
		{
			Parameters = parameters;
		}

        [field: FormerlySerializedAs("mResourcesDirectory")]
        public string ResourcesDirectory { get; set; }
        [field: FormerlySerializedAs("mAssetsDirectory")]
        public string AssetsDirectory { get; set; }

        [field: FormerlySerializedAs("mProjectDirectory")]

        public string ProjectDirectory { get; set; }



        public BuildProcess GetCurrentBuildProcess()
		{
			
			UBSProcess ubs = UBSProcess.LoadUBSProcess();
			BuildProcess process = ubs.GetCurrentProcess();
			return process;
		}

		public BuildCollection GetCurrentBuildCollection()
		{
			UBSProcess ubs = UBSProcess.LoadUBSProcess();
			return ubs.BuildCollection;
		}

		public void Cancel(string message)
		{
			UBSProcess ubs = UBSProcess.LoadUBSProcess();
			ubs.Cancel(message);
		}

		/// <summary>
		/// Returns an instance of a class you define, which has the start up parameters set in its properties. 
		/// The class should derive from ProgramOptions and have atleast one OptionAttribute on one of its properties. 
		/// </summary>
		/// <returns>The program options.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T GetProgramOptions<T>() where T : ProgramOptions
		{
			string[] arguments = System.Environment.GetCommandLineArgs();
			ProgramOptions po = ProgramOptions.CreateInstance<T>(arguments);
			return po as T;
		}
	}
}

