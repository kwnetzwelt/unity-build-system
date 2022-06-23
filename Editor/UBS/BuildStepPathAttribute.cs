namespace UBS
{
    /// <summary>
    /// Use to customize where in the dropdown hierarchy this build step appears. 
    /// </summary>
    public class BuildStepPathAttribute : System.Attribute
    {
        public string Path { get; }
        public BuildStepPathAttribute(string path)
        {
            Path = path;
        }
    }
}