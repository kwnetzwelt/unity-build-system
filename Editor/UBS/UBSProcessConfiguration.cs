using System;
using System.Collections.Generic;

namespace UBS
{
    [Serializable]
    public class UBSProcessConfiguration
    {
        public CommandLineArgsParser.ArgsCollection CommandlineArgs { get; set; } = new();
        public List<string> SelectedBuildProcessNames { get; set; } = new();
        public BuildCollection Collection { get; set; }
        public bool BuildAndRun { get; set; }
        public bool BatchMode { get; set; }
        public bool BuildAll { get; set; }
        public string BuildTag { get; set; }
        public CleanBuildArgument Clean { get; set; }

        public List<BuildProcess> SelectedBuildProcesses { get; set; } = new();
    }
}