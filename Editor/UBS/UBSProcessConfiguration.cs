using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Editor.UBS.Commandline;
using UnityEngine;

namespace UBS
{
    [Serializable]
    public class UBSProcessConfiguration
    {
        [field: SerializeField]
        public CommandLineArgsCollection CommandLineArgs { get; set; } = new();
        [field: SerializeField]
        public List<string> SelectedBuildProcessNames { get; set; } = new();
        [field: SerializeField]
        public BuildCollection Collection { get; set; }
        [field: SerializeField]
        public bool BuildAndRun { get; set; }
        [field: SerializeField]
        public bool BatchMode { get; set; }
        [field: SerializeField]
        public bool DevelopmentBuild { get; set; }
        [field: SerializeField]
        public bool BuildAll { get; set; }
        [field: SerializeField]
        public string BuildTag { get; set; }
        [field: SerializeField]
        public CleanBuildArgument Clean { get; set; }

        [field: SerializeField]
        public List<BuildProcess> SelectedBuildProcesses { get; set; } = new();

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"UBSProcessConfiguration for Collection \"{(Collection != null ? Collection.name : "null")}\":");
            sb.AppendLine($"  - BatchMode: {BatchMode}");
            sb.AppendLine($"  - BuildAndRun: {BuildAndRun}");
            sb.AppendLine($"  - DevelopmentBuild: {DevelopmentBuild}");
            sb.AppendLine($"  - BuildAll: {BuildAll}");
            sb.AppendLine($"  - Clean: {Clean}");
            sb.AppendLine($"  - BuildTag: {(string.IsNullOrEmpty(BuildTag) ? "(none)" : BuildTag)}");
            sb.AppendLine($"  - CommandLineArgs: {(CommandLineArgs != null && CommandLineArgs.Arguments != null && CommandLineArgs.Arguments.Count > 0 ? string.Join(" ", CommandLineArgs.Arguments.Select(a => string.IsNullOrEmpty(a.Value) ? $"-{a.Name}" : $"-{a.Name} {a.Value}")) : "(none)")}");
            sb.AppendLine($"  - Requested Process Names: {(SelectedBuildProcessNames != null && SelectedBuildProcessNames.Count > 0 ? string.Join(", ", SelectedBuildProcessNames) : "(none)")}");
            sb.Append($"  - Selected Processes ({SelectedBuildProcesses?.Count ?? 0}):");
            if (SelectedBuildProcesses != null && SelectedBuildProcesses.Count > 0)
            {
                for (int i = 0; i < SelectedBuildProcesses.Count; i++)
                {
                    var bp = SelectedBuildProcesses[i];
                    sb.AppendLine();
                    sb.Append($"    [{i + 1}/{SelectedBuildProcesses.Count}] {bp.Name} (Platform: {bp.Platform}, OutputPath: \"{bp.OutputPath}\", Options: {bp.Options}, Pretend: {bp.Pretend})");
                }
            }
            return sb.ToString();
        }
    }
}