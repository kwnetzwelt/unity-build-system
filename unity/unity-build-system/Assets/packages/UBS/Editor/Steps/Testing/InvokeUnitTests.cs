using System.Collections;
using System.IO;
using System.Linq;
using UBS;
using UnityEditor;
using UnityEngine;
using UnityTest;
using UnityTest.UnitTestRunner;

namespace UBS.Testing
{
	[BuildStepDescriptionAttribute("Invokes Unit Tests from Unity Testing Tools. Use parameter to filter tests by full name. For multiple parameter use ';' to split.")]
	[BuildStepParameterFilter(EBuildStepParameterType.String)]
	public class InvokeUnitTests : IBuildStepProvider, ITestRunnerCallback
	{
		/// <summary>
		/// Relative path to project root.
		/// </summary>
		const string kResultFilePath = "UnitTestResults.xml";
		const int kReturnTestsFailed = 2;
		const int kReturnRunError = 3;

		bool mDone = false;
		UnitTestResult[] mResults;
		
		#region IBuildStepProvider implementation
		
		public void BuildStepStart (BuildConfiguration pConfiguration)
		{
			var filter  = new TestFilter();
			if(!string.IsNullOrEmpty(pConfiguration.Params))
			{
				filter.names = pConfiguration.Params.Split(';');
				filter.categories = null;
			}
			
			EditorApplication.NewScene();
			var engine = new NUnitTestEngine();
			string[] categories;
			engine.GetTests(out mResults, out categories);
			engine.RunTests(filter, this);
		}
		
		public void BuildStepUpdate ()
		{
			
		}
		
		public bool IsBuildStepDone ()
		{
			return mDone;
		}
		
		#endregion
		
		
		#region ITestRunnerCallback implementation
		public void TestStarted (string fullName)
		{
			
		}
		public void TestFinished (ITestResult fullName)
		{
			mResults.Single(r => r.Id == fullName.Id).Update(fullName, false);
		}
		public void RunStarted (string suiteName, int testCount)
		{
			
		}
		public void RunFinished ()
		{
			var baseDir = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);
			var fileName = Path.GetFileName(kResultFilePath);
			var resultDir = kResultFilePath.Substring(0, kResultFilePath.Length - fileName.Length);
			resultDir = Path.Combine(baseDir, resultDir);
	#if !UNITY_METRO
			var resultWriter = new XmlResultWriter("Unit Tests", "Editor", mResults.ToArray());
			resultWriter.WriteToFile(resultDir, fileName);
	#endif
			var executed = mResults.Where(result => result.Executed);
			if (!executed.Any())
			{
				Debug.LogError("Failed to run testrunner!");
				UBSProcess.LoadUBSProcess().Cancel();
				if(UBSProcess.LoadUBSProcess().IsInBatchMode)
				{
					EditorApplication.Exit(kReturnRunError);
				}
				return;
			}
			var failed = executed.Where(result => !result.IsSuccess);
			
			if(failed.Any())
			{
				//Stop the build process
				Debug.LogError("One or more tests failed! -->");
				foreach(var single in failed)
				{
					Debug.LogError(single.FullName);
				}
				Debug.LogError("<--");
				UBSProcess.LoadUBSProcess().Cancel();

				if(UBSProcess.LoadUBSProcess().IsInBatchMode)
				{
					EditorApplication.Exit(kReturnTestsFailed);
				}
			}
			else
			{
				//Continue the process
				Debug.Log("All tests passed!");
				mDone = true;
			}
		}
		public void RunFinishedException (System.Exception exception)
		{
			UBSProcess.LoadUBSProcess().Cancel();
			if(UBSProcess.LoadUBSProcess().IsInBatchMode)
			{
				EditorApplication.Exit(kReturnRunError);
			}
			throw exception;
		}
		#endregion
	}
}

