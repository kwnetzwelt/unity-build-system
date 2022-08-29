namespace UBS
{
    public class BuildStepToggleAttribute : System.Attribute
    {
        public string Tooltip { get; set; }
        
        public BuildStepToggleAttribute(string tooltip)
        {
            Tooltip = tooltip;
        }
    }
}