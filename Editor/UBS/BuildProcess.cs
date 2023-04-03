using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine.Serialization;

namespace UBS {
	
	[Serializable]
	public class BuildProcess {

		/// <summary>
		/// Wrapper class for addressable groups, to allow them to be disabled/enabled for each build
		/// </summary>
		[Serializable]
		public class AddressableGroupStatus
		{
			public string Guid;
			public string Name;
			public bool Enabled;
		}

		public BuildProcess()
		{
			
		}

		public void CheckAddressableGroupStatus()
		{
			// Check addressable groups are added to entries
			if (AddressableAssetSettingsDefaultObject.Settings == null)
			{
				// No addressable asset settings, ignore
				return;
			}
			
			var allGroups = AddressableAssetSettingsDefaultObject.Settings.groups;

			foreach (var addressableGroup in allGroups)
			{
				// Get schema for inclusion in bundled asset group 
				var schema = addressableGroup.GetSchema<BundledAssetGroupSchema>();
				//  if this is not present, we can ignore
				if (schema==null) continue;
				// Do we already have an entry for this group?
				var matchingEntry = AddressableGroups.FirstOrDefault(group => group.Guid == addressableGroup.Guid);
				// If not, create a new entry
				if (matchingEntry==null)
				{
					AddressableGroups.Add(new AddressableGroupStatus() {Guid = addressableGroup.Guid, Enabled = schema.IncludeInBuild, Name = addressableGroup.Name});
				}
				else
				{
					matchingEntry.Name=addressableGroup.Name;
				}
			}
			// Remove any entries that are no longer valid (group doesnt exist any more)
			for (var i=0;i<AddressableGroups.Count;i++)
			{
				var entry = AddressableGroups[i];
				if (allGroups.FirstOrDefault(group=>group.Guid==entry.Guid)==null)
				{
					AddressableGroups.RemoveAt(i);
					i--;
				}
			}
		}
		
		public BuildProcess(BuildProcess other)
		{
			foreach(var bs in other.PreBuildSteps)
			{
				PreBuildSteps.Add( new BuildStep(bs) );
			}

			
			foreach(var bs in other.PostBuildSteps)
			{
				PostBuildSteps.Add( new BuildStep(bs) );
			}

			Name = other.Name;
			OutputPath = other.OutputPath;
			Platform = other.Platform;
			Options = other.Options;
			Selected = false;
			ScriptingDefines = other.ScriptingDefines;
			SceneAssets = new List<SceneAsset>( other.SceneAssets );
		}

		#region data

		/// <summary>
		/// If true, uses scene collection set in EditorBuildSettings instead of custom scene collection in the process
		/// </summary>
		[field:SerializeField()]
		public bool UseEditorScenes { get; set; } = false;

		
        [field: FormerlySerializedAs("mPreBuildSteps")]
        [field:SerializeField()]
        public List<BuildStep> PreBuildSteps { get; private set; } = new List<BuildStep>();

        [field: FormerlySerializedAs("mPostBuildSteps")]
        [field:SerializeField()]
        public List<BuildStep> PostBuildSteps { get; private set;} = new List<BuildStep>();

        [field: FormerlySerializedAs("mName")]
        [field:SerializeField()]
        public string Name { get; set; } = "Build Process";

        [field: FormerlySerializedAs("mOutputPath")]
        [field:SerializeField()]
        public string OutputPath { get; set; } = "";

        /// <summary>
        /// If pretend mode is on, the process will not actually trigger a build. It will do everything else though. 
        /// </summary>
        [field: FormerlySerializedAs("mPretend")]
        [field:SerializeField()]
        public bool Pretend { get; set; } = false;

        [field: FormerlySerializedAs("mPlatform")]
        [field:SerializeField()]
        public BuildTarget Platform { get; set; }

        [field: FormerlySerializedAs("mBuildOptions")]
        [field:SerializeField()]
        public BuildOptions Options { get; set; }


        [field: FormerlySerializedAs("mSelected")]
        [field:SerializeField()]
        public bool Selected { get; set; }

        [field: FormerlySerializedAs("mSceneAssets")]
        [field:SerializeField()]
        public List<SceneAsset> SceneAssets { get; private set;} = new List<SceneAsset>();

        [field:SerializeField] 
        public List<string> ScriptingDefines { get; protected set; } = new();
        
        [field:SerializeField] 
        public List<AddressableGroupStatus> AddressableGroups { get; protected set; } = new();

        #endregion

        public void ApplyAddressableGroupSettingsForBuild()
        {
	        // Get all addressable asset groups
	        var allGroups = AddressableAssetSettingsDefaultObject.Settings.groups;
	        
	        foreach (var groupEntry in AddressableGroups)
	        {
		       
		        // Get schema for inclusion in bundled asset group 
		        var matchingGroup = allGroups.FirstOrDefault(group => group.Guid == groupEntry.Guid);
		        if (matchingGroup != null)
		        {
			        var schema = matchingGroup.GetSchema<BundledAssetGroupSchema>();
			        if (schema != null)
			        {
				        schema.IncludeInBuild= groupEntry.Enabled;
			        }
			        else
			        {
				        Debug.Log($"Couldn't find BundledAssetGroupSchema - {groupEntry.Name}");
			        }
		        }
		        else
		        {
			        Debug.LogWarning($"Couldn't find matching addressable group - {groupEntry.Name}");
		        }
	        }
        }
	}
}