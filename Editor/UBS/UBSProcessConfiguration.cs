using System;
using System.Collections.Generic;
using UnityEngine;

namespace UBS
{
    [Serializable]
    public class UBSProcessConfiguration
    {
        [field: SerializeField]
        public CommandlineArgsCollection CommandlineArgs { get; set; } = new();
        [field: SerializeField]
        public List<string> SelectedBuildProcessNames { get; set; } = new();
        [field: SerializeField]
        public BuildCollection Collection { get; set; }
        [field: SerializeField]
        public bool BuildAndRun { get; set; }
        [field: SerializeField]
        public bool BatchMode { get; set; }
        [field: SerializeField]
        public bool BuildAll { get; set; }
        [field: SerializeField]
        public string BuildTag { get; set; }
        [field: SerializeField]
        public CleanBuildArgument Clean { get; set; }

        [field: SerializeField]
        public List<BuildProcess> SelectedBuildProcesses { get; set; } = new();
    }
}