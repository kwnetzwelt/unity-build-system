namespace UBS.Version
{
    [BuildStepDescription("Loads version information from player settings. ")]
    [BuildStepPath("UBS/Version/Load from Player Settings")]
    public class VersionLoadFromPlayerSettings : IBuildStepProvider
    {
        public void BuildStepStart(BuildConfiguration configuration)
        {
            configuration.GetCurrentBuildCollection().LoadVersionFromSettings();
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