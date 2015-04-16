using UnityEditor;
using System.IO;

namespace UBS
{
	[BuildStepTypeFilter(EBuildStepType.PostBuildStep)]
	[BuildStepDescription("Renames the build output file accordingly with identifier, bundleVersion and textureFormat.")]
	[BuildStepPlatformFilter(BuildTarget.Android)]
	[BuildStepParameterFilterAttribute(EBuildStepParameterType.None)]
	public class RenameOutput : IBuildStepProvider
	{
		#region IBuildStepProvider implementation
		public void BuildStepStart(BuildConfiguration pConfiguration)
		{
			BuildProcess p = pConfiguration.GetCurrentBuildProcess();
			FileInfo file = new FileInfo(p.mOutputPath);
			string newFilename = string.Format("{0}/{1}_{2}_{3}.apk", file.DirectoryName,
			                                   PlayerSettings.bundleIdentifier,
			                                   PlayerSettings.bundleVersion,
			                                   EditorUserBuildSettings.androidBuildSubtarget.ToString().ToLower());

			// maybe pretend build was enabled before
			if (file.Exists) {
				file.MoveTo(newFilename);
			}

			// user could have disabled the apk split already -> just check for an existing obb file.
			RenameOBB(p.mOutputPath);
		}
		public void BuildStepUpdate()
		{

		}
		public bool IsBuildStepDone()
		{
			return true;
		}
		#endregion

		void RenameOBB(string filePath)
		{
			FileInfo file = new FileInfo(filePath);
			string fileName = file.Name.Replace(file.Extension, "");
			var obbFile = new FileInfo(string.Format("{0}/{1}.main.obb", file.DirectoryName, fileName));
			if (obbFile.Exists) {
				var version = PlayerSettings.bundleVersion.Replace(".", "");
				string newFilename = string.Format("{0}/main.{1}_{2}_{3}.obb", file.DirectoryName,
				                                   version,
				                                   PlayerSettings.bundleIdentifier,
				                                   EditorUserBuildSettings.androidBuildSubtarget.ToString().ToLower());
				obbFile.MoveTo(newFilename);
			}
		}
			
	}
}

