using UnityEditor;
using System.IO;

namespace UBS
{
	[BuildStepTypeFilter(BuildStepType.PostBuildStep)]
	[BuildStepDescription("Renames the build output file accordingly with identifier, bundleVersion and textureFormat.")]
	[BuildStepPlatformFilter(BuildTarget.Android)]
	[BuildStepParameterFilterAttribute(BuildStepParameterType.None)]
	public class RenameOutput : IBuildStepProvider
	{
		#region IBuildStepProvider implementation
		public void BuildStepStart(BuildConfiguration configuration)
		{
			BuildProcess p = configuration.GetCurrentBuildProcess();
			FileInfo file = new FileInfo(p.OutputPath);
			string newFilename = string.Format("{0}/{1}_{2}_{3}.apk", file.DirectoryName,
#if UNITY_5
                                                PlayerSettings.bundleIdentifier,
#else
                                                PlayerSettings.applicationIdentifier,
#endif
                                                PlayerSettings.bundleVersion,
                                                EditorUserBuildSettings.androidBuildSubtarget.ToString().ToLower());

			// maybe pretend build was enabled before
			if (file.Exists) {
				file.MoveTo(newFilename);
			}

			// user could have disabled the apk split already -> just check for an existing obb file.
			RenameOBB(p.OutputPath);
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
#if UNITY_5
                                        PlayerSettings.bundleIdentifier,
#else
                                        PlayerSettings.applicationIdentifier,
#endif
                                        EditorUserBuildSettings.androidBuildSubtarget.ToString().ToLower());
				obbFile.MoveTo(newFilename);
			}
		}
			
	}
}

