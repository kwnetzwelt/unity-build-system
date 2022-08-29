namespace UBS.Version
{
    [BuildStepDescription("Writes the current version information to player settings. ")]
    [BuildStepPath("UBS/Version/Write to Player Settings")]
    public class VersionWriteToPlayerSettings : IBuildStepProvider
    {
        public void BuildStepStart(BuildConfiguration configuration)
        {
            configuration.GetCurrentBuildCollection().SaveVersion(true);
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