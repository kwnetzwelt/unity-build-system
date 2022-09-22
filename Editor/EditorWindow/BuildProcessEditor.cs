using System;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using UnityEditor.Graphs;
using System.Linq;
using System.Text;

namespace UBS
{
	internal class BuildStepProviderEntry
	{
		public BuildStepProviderEntry(System.Type buildStepType)
		{
			if (buildStepType == null)
			{
				Name = "None";
				return;
			}
			StepType = buildStepType;
			Name = StepType.Name;
			Ns = StepType.Namespace;

			foreach (var a in StepType.GetCustomAttributes(true))
            {
                if (a is BuildStepPathAttribute)
                    PathAttribute = a as BuildStepPathAttribute;
				if (a is BuildStepDescriptionAttribute)
					DescriptionAttribute = a as BuildStepDescriptionAttribute;
				else if (a is BuildStepPlatformFilterAttribute)
					PlatformFilterAttribute = a as BuildStepPlatformFilterAttribute;
				else if (a is BuildStepTypeFilterAttribute)
					TypeFilterAttribute = a as BuildStepTypeFilterAttribute;
				else if(a is BuildStepParameterFilterAttribute)
					ParameterFilterAttribute = a as BuildStepParameterFilterAttribute;
				else if (a is BuildStepToggleAttribute)
					ToggleAttribute = a as BuildStepToggleAttribute;
			}
		}

		
		public string Name { get; }
        /// <summary>
        /// Namespace
        /// </summary>
        public string Ns { get; }

        public Type StepType { get; }
        public BuildStepPathAttribute PathAttribute { get; }
        public BuildStepDescriptionAttribute DescriptionAttribute { get; }
        public BuildStepPlatformFilterAttribute PlatformFilterAttribute { get; }
        public BuildStepTypeFilterAttribute TypeFilterAttribute { get; }
        public BuildStepParameterFilterAttribute ParameterFilterAttribute { get; }
        public BuildStepToggleAttribute ToggleAttribute { get; }

        public override string ToString()
		{
			return Name;
		}

		public string ToMenuPath ()
        {
            if (PathAttribute != null)
                return PathAttribute.Path;    
            if(Ns == null)
				return Name;
            
			return Ns.Replace(".","/") + "/" + Name;
		}

		public string GetDescription()
		{
			if (DescriptionAttribute != null && DescriptionAttribute.Description != null)
				return DescriptionAttribute.Description;
			return "";
		}

		
		public BuildStepParameterType GetParameterType()
		{
			if(ParameterFilterAttribute != null)
			{
				return ParameterFilterAttribute.BuildParameterType;
			}
			else
			{                
				// this is the default behavior, since the former buildsteps were designed as string parameters
				return BuildStepParameterType.String;
			} 
		}
		
		public string[] GetParameterDropdownOptions()
		{
			if(ParameterFilterAttribute != null &&
			   ParameterFilterAttribute.BuildParameterType == BuildStepParameterType.Dropdown)
			{
				return ParameterFilterAttribute.DropdownOptions;
			}
			else
			{
				return null;
			}
		}


		public bool CheckFilters(BuildStepType pDrawingBuildStepType, BuildTarget pPlatform)
		{
			if (PlatformFilterAttribute != null)
			{
				if (PlatformFilterAttribute.BuildTarget != pPlatform)
				{
					return false;
				}
			}
			if (TypeFilterAttribute != null)
			{
				if (TypeFilterAttribute.BuildStepType != pDrawingBuildStepType)
				{
					return false;
				}
			}
			return true;
		}
	}

	public class BuildProcessEditor
	{
		BuildStepType _drawingBuildStepType = BuildStepType.invalid;
		static List<System.Type> _buildStepProviders;
		BuildStepProviderEntry[] _selectableBuildStepProviders;

		bool _showBuildOptions;
		private bool _showScenesList;
		BuildProcess _editedBuildProcess;
		BuildCollection collection;

		private bool _showExtraScriptingDefines;

		
		public BuildProcessEditor()
		{
			//
			// create list of available Build SteP Providers
			//
			_buildStepProviders = UBS.Helpers.FindClassesImplementingInterface(typeof(IBuildStepProvider));
#if UBS_DEBUG
			Debug.Log("Found " + mBuildStepProviders.Count + " BuildStepProviders");
#endif
			_selectableBuildStepProviders = new BuildStepProviderEntry[_buildStepProviders.Count + 1];
			_selectableBuildStepProviders [0] = new BuildStepProviderEntry(null);
			for (int i = 0; i<_buildStepProviders.Count; i++)
			{
				_selectableBuildStepProviders [i + 1] = new BuildStepProviderEntry(_buildStepProviders [i]);

#if UBS_DEBUG
				Debug.Log(">" + mBuildStepProviders[i].Name);
#endif
			}

			//
			// create list of available build targets
			//




		}

		public void OnDestroy()
		{
			_editedBuildProcess = null;
		}
        List<BuildOptions> _buildOptions;
		string _selectedOptionsString;

        private ReorderableList _sceneList;
        private ReorderableList _extraScriptingDefinesList;
        private ReorderableList _prebuildStepsList;
        private ReorderableList _postbuildStepsList;

        void OnEnable()
		{
			_buildOptions = new List<BuildOptions>();
            var options = (BuildOptions[])Enum.GetValues(typeof(BuildOptions));
            foreach (var option in options)
            {
	            if (IsObsolete(option)) continue;
                int o = (int) option;
                if(o != 0)
                    _buildOptions.Add(option);
            }
            
            UpdateSelectedOptions();
            

            _sceneList = new ReorderableList(_editedBuildProcess.SceneAssets, typeof(SceneAsset));
            _sceneList.drawHeaderCallback = SceneHeaderDrawer;
            _sceneList.drawElementCallback = SceneDrawer;
            _sceneList.onAddCallback = delegate(ReorderableList list)
            {
	            if (list.selectedIndices.Count > 0 && _editedBuildProcess.SceneAssets.Count > 0)
		            _editedBuildProcess.SceneAssets.Insert(list.selectedIndices[0] + 1, null);
	            else
		            _editedBuildProcess.SceneAssets.Add(null);
            };

            _extraScriptingDefinesList = new ReorderableList(_editedBuildProcess.ScriptingDefines.ToList(), typeof(string))
	            {
		            drawHeaderCallback = ExtraScriptingDefinesHeaderDrawer,
		            drawElementCallback = ExtraScriptingDefinesDrawer,
		            onReorderCallback = ExtraScriptingDefinesReorder
	            };

            _prebuildStepsList = new ReorderableList(_editedBuildProcess.PreBuildSteps, typeof(BuildStep));
            _prebuildStepsList.drawHeaderCallback = PreStepHeaderDrawer;
            _prebuildStepsList.drawElementCallback = PreStepDrawer;
            
            _postbuildStepsList = new ReorderableList(_editedBuildProcess.PostBuildSteps, typeof(BuildStep));
            _postbuildStepsList.drawHeaderCallback = PostStepHeaderDrawer;
            _postbuildStepsList.drawElementCallback = PostStepDrawer;


        }

        private void ExtraScriptingDefinesReorder(ReorderableList list)
        {
	        for (var index = 0; index < list.count; index++)
	        {
		        _editedBuildProcess.ScriptingDefines[index] = list.list[index] as string;
	        }
        }

        // https://stackoverflow.com/a/44811113
        public static bool IsObsolete(Enum value)
        {  
	        var enumType = value.GetType();
	        var enumName = enumType.GetEnumName(value);
	        var fieldInfo = enumType.GetField(enumName);
	        return Attribute.IsDefined(fieldInfo, typeof(ObsoleteAttribute));
        }

        private void ExtraScriptingDefinesHeaderDrawer(Rect rect)
        {
	        GUI.Label(rect, "Extra Scripting Defines");
        }

        private void SceneHeaderDrawer(Rect rect)
        {
            GUI.Label(rect, "Selected Scenes");
        }
        private void PreStepHeaderDrawer(Rect rect)
        {
            GUI.Label(rect, "Pre Build Steps");
        }
        private void PostStepHeaderDrawer(Rect rect)
        {
            GUI.Label(rect, "Post Build Steps");
        }

        void UpdateSelectedOptions()
		{
			StringBuilder sb = new StringBuilder();
			foreach (var buildOption in _buildOptions)
			{
				if ((_editedBuildProcess.Options & buildOption) != 0)
				{
					sb.Append(buildOption.ToString() + ", ");
				}
			}
			_selectedOptionsString = sb.ToString();
			if (_selectedOptionsString.Length > 2)
				_selectedOptionsString = _selectedOptionsString.Substring(0, _selectedOptionsString.Length - 2);
		}

		public void OnGUI(BuildProcess pProcess, BuildCollection pCollection)
		{

			if (pProcess == null)
				return;

			if (pProcess != _editedBuildProcess)
			{
				_editedBuildProcess = pProcess;
				collection = pCollection;

				OnEnable();
				
				// after switching to another process, we want to make sure to unfocus all possible controls
				// like textfields. This will remove an issue, where dangling focus between process switching could happen
				GUI.FocusControl("");

			}

			GUILayout.BeginVertical();

			GUILayout.Label("Build Process", Styles.DetailsTitle);

			Styles.HorizontalSeparator();

			Undo.RecordObject(collection, "Edit Build Process Details");

			_editedBuildProcess.Pretend = EditorGUILayout.Toggle(new GUIContent("Pretend Build", 
				"Will not trigger a unity build, but run everything else. "), _editedBuildProcess.Pretend);
			pProcess.Name = EditorGUILayout.TextField("Name", _editedBuildProcess.Name);
			_editedBuildProcess.Platform = (BuildTarget)EditorGUILayout.EnumPopup("Platform", 
				_editedBuildProcess.Platform);
			DrawOutputPathSelector(); 
			_editedBuildProcess.UseEditorScenes = EditorGUILayout.Toggle(
				new GUIContent("Use Scenes from Editor", 
					"Instead of using its own Scene Collection, the Build Process will use whichever " +
					"scenes are set in the EditorBuildSettings when it runs. "), _editedBuildProcess.UseEditorScenes);
			
			GUILayout.Space(5);
			_showBuildOptions = EditorGUILayout.Foldout(_showBuildOptions, "Build Options");
			GUILayout.BeginHorizontal();
			GUILayout.Space(25);

			if (_showBuildOptions)
			{
				GUILayout.BeginVertical();

				foreach (var buildOption in _buildOptions)
				{
					bool selVal = (_editedBuildProcess.Options & buildOption) != 0;
                    {
                        string label = buildOption.ToString();
                        label = label.Substring(label.LastIndexOf(',')+2); // to avoid having all zeroed entries in the name
						bool resVal = EditorGUILayout.ToggleLeft(label, selVal);
						if (resVal != selVal)
						{
							if (resVal)
								_editedBuildProcess.Options = _editedBuildProcess.Options | buildOption;
							else
								_editedBuildProcess.Options = _editedBuildProcess.Options & ~buildOption;
							UpdateSelectedOptions();
						}
					}
				}

				
				GUILayout.EndVertical();
			} else
			{
				if(!string.IsNullOrEmpty(_selectedOptionsString))
					GUILayout.Label(_selectedOptionsString);
			}


			GUILayout.EndHorizontal();
			GUILayout.Space(5);

			
			
			GUI.enabled = !_editedBuildProcess.UseEditorScenes;
			_showScenesList = EditorGUILayout.Foldout(_showScenesList, "Scene Collection (" + _sceneList.count + ")");
			if (_showScenesList)
			{
				DrawList(_sceneList);
			

				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();

				
				
				if (GUILayout.Button("Copy scenes from settings"))
					CopyScenesFromSettings();

				if (GUILayout.Button("Clear scenes"))
					_editedBuildProcess.SceneAssets.Clear();

				GUILayout.EndHorizontal();
			}
			GUI.enabled = true;

			GUILayout.Space(5);

			_showExtraScriptingDefines = EditorGUILayout.Foldout(_showExtraScriptingDefines, "Extra Scripting Defines (" + _extraScriptingDefinesList.count + ")");
			if (_showExtraScriptingDefines)
			{
				DrawList(_extraScriptingDefinesList);
			
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();

				const string clearExtraScriptingDefines = "Clear Extra Scripting Defines";
				if (GUILayout.Button(clearExtraScriptingDefines))
				{
					Undo.RecordObject(collection, clearExtraScriptingDefines);
					_editedBuildProcess.ScriptingDefines.Clear();
					_extraScriptingDefinesList.list.Clear();
				}

				GUILayout.EndHorizontal();
			}

			Styles.HorizontalSeparator();

			_drawingBuildStepType = BuildStepType.PreBuildStep;

            DrawList(_prebuildStepsList);


			Styles.HorizontalSeparator();
			GUILayout.Label("Actual Unity Build", Styles.MediumHint);
			Styles.HorizontalSeparator();


			_drawingBuildStepType = BuildStepType.PostBuildStep;

            DrawList(_postbuildStepsList);
			
			GUILayout.EndVertical();

		}

        private void DrawList(ReorderableList list)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(4);
                GUILayout.BeginVertical();
                {
                    list.DoLayoutList();
                }
                GUILayout.EndVertical();
                GUILayout.Space(4);
            }
            GUILayout.EndHorizontal();
        }

        void SceneDrawer(UnityEngine.Rect pRect, int index, bool isActive, bool isFocused)
        {
            pRect.height = pRect.height - 4;
            pRect.y = pRect.y + 2;

            SceneAsset pScene = _editedBuildProcess.SceneAssets[index];
            var selected = EditorGUI.ObjectField(pRect, "Scene " + index, pScene, typeof(SceneAsset), false) as SceneAsset;
            
            if (selected != pScene)
                Undo.RecordObject(collection, "Set Scene Entry");

            _editedBuildProcess.SceneAssets[index] = selected;

		}
        
        
        
        private void ExtraScriptingDefinesDrawer(Rect pRect, int index, bool isActive, bool isFocused)
        {
	        while (index >= _editedBuildProcess.ScriptingDefines.Count)
	        {
		        var placeholder = $"NEW_SCRIPTING_DEFINE_{_editedBuildProcess.ScriptingDefines.Count}";
		        _editedBuildProcess.ScriptingDefines.Add(placeholder);
		        _extraScriptingDefinesList.list[index] = placeholder;
	        }
		        
	        pRect.height -= 4;
	        pRect.y += 2;
	        var currentScriptingDefineAtIndex = _editedBuildProcess.ScriptingDefines[index];
	        var newScriptingDefineAtIndex = EditorGUI.TextField(pRect, currentScriptingDefineAtIndex);

	        if (string.Equals(currentScriptingDefineAtIndex, newScriptingDefineAtIndex)) return;
	        Undo.RecordObject(collection, $"Update Scripting Define at index {index}");
	        _editedBuildProcess.ScriptingDefines[index] = newScriptingDefineAtIndex;
	        _extraScriptingDefinesList.list[index] = newScriptingDefineAtIndex;
        }
        
        void PreStepDrawer(Rect pRect, int index, bool isActive, bool isFocused)
        {
            UBS.BuildStep step = _editedBuildProcess.PreBuildSteps[index];
            step = StepDrawer(pRect, step);
            _editedBuildProcess.PreBuildSteps[index] = step;
        }

        void PostStepDrawer(Rect pRect, int index, bool isActive, bool isFocused)
        {
            UBS.BuildStep step = _editedBuildProcess.PostBuildSteps[index];
            step = StepDrawer(pRect, step);
            _editedBuildProcess.PostBuildSteps[index] = step;
        }

        UBS.BuildStep StepDrawer(Rect pRect, UBS.BuildStep pStep)
        {
            pRect.height = pRect.height - 4;
            pRect.y = pRect.y + 2;


            if (pStep == null)
				pStep = new BuildStep();

			var filtered = new List<BuildStepProviderEntry>(_selectableBuildStepProviders);
			filtered = filtered.FindAll((obj) => {
				if (obj.Name == "None")
					return false;
				return obj.CheckFilters(_drawingBuildStepType, _editedBuildProcess.Platform);
			});
			
			int selectedIndex = 0; 
			int listIndex = 0;
            bool enabled = pStep.Enabled;
            bool couldInferType = true;
			if (pStep.TypeName != null)
			{
				couldInferType = pStep.TryInferType(false);
				if (couldInferType)
				{
					listIndex = filtered.FindIndex((obj) => { return obj.StepType == pStep.StepType; });
					selectedIndex = listIndex + 1;
				}
				else
				{
					GUI.color = Color.red;
				}
			}
			
			BuildStepParameterType parametersToDisplay = BuildStepParameterType.None;
			BuildStepProviderEntry buildStepProvider = null;
			if(listIndex >= 0 && listIndex < filtered.Count())
			{                
				buildStepProvider = filtered[listIndex];
				parametersToDisplay = buildStepProvider.GetParameterType();
			}
			bool needsToggle = buildStepProvider?.ToggleAttribute != null;


			
			GUIContent[] displayedProviders = GetBuildStepProvidersFiltered();
            Rect r1 = new Rect(pRect.x, pRect.y + 1, 20, pRect.height);
			Rect r2 = new Rect(r1.x + r1.width,pRect.y + 1, 220, pRect.height); // drop down list
			Rect r3 = new Rect(r2.x + r2.width, pRect.y, 30, pRect.height); // gears
			Rect r4 = new Rect(r3.x + r3.width - 5, pRect.y, 70, pRect.height); // parameters input
			Rect r5 = new Rect(r4.x + r4.width - 5, pRect.y, pRect.width - 345, pRect.height); // parameters input
			Rect r6 = new Rect(r5.x + r5.width + 3, pRect.y, 17, pRect.height); // parameters label

			if (!needsToggle)
				r5.width += r6.width;
			
            pStep.Enabled = EditorGUI.Toggle(r1, enabled);
			int idx = EditorGUI.Popup(r2, selectedIndex, displayedProviders);
			if (!EditorGUIUtility.isProSkin)
				GUI.color = Color.black;
			if (idx > 0 && GUI.Button(r3, Styles.Gear, EditorStyles.miniLabel))
			{
				if (idx > 0)
				{
					EditorUtility.DisplayDialog(
						"Build Step Help",
						displayedProviders [idx].text + "\n\n" + displayedProviders [idx].tooltip,
						"Close"
					);
				}
			}
			GUI.color = Color.white;
			//r.x += r.width;
			
			
			
			//r.x += r.width;

			// search for buildstepprovider
			
			
			GUI.Label(r4, "Parameters", EditorStyles.miniLabel);

			
			switch(parametersToDisplay)
			{
			case BuildStepParameterType.None:
			{
				// dont show anything!
			}
				break;
            
            case BuildStepParameterType.Boolean:
            {
                bool value;
                bool succeeded = pStep.Parameters;
                pStep.Parameters.boolParameter = EditorGUI.Toggle(r5, pStep.Parameters);
            }
                break;
				
			case BuildStepParameterType.String:
			{
				pStep.Parameters.stringParameter = EditorGUI.TextField(r5, pStep.Parameters );
			}
				break;
				
			case BuildStepParameterType.Dropdown:
			{
				List<string> options = new List<string>(buildStepProvider.GetParameterDropdownOptions());                    
				int selectedValue = 0;
				if(!String.IsNullOrEmpty(pStep.Parameters))
				{
					selectedValue = options.FindIndex((option) => {return option == pStep.Parameters;});
				}
					
				if(selectedValue == -1) 
				{
					selectedValue = 0; // first index as fallback
					Debug.LogError("Invalid dropdown entry found: " + pStep.Parameters + " for buildstep: " + buildStepProvider.Name + ". Fallback to index 0 applied!");
				}
				
				// create popup control and assign selected index
				int returnedIndex = EditorGUI.Popup(r5, selectedValue, GetBuildStepProvidersParameterOptions(buildStepProvider) );
				pStep.Parameters.stringParameter = options[returnedIndex];
			}
				break;
				
            case BuildStepParameterType.UnityObject:
                {
                    UnityEngine.Object objectAssigned = pStep.Parameters;
                    
                    var assignedObject = EditorGUI.ObjectField(r5, objectAssigned, typeof(UnityEngine.Object), false);
	                pStep.Parameters.objectParameter = assignedObject;
                }
                break;

			}

			if (needsToggle)
			{
				pStep.ToggleValue = GUI.Toggle(r6,
					pStep.ToggleValue, new GUIContent("", buildStepProvider.ToggleAttribute.Tooltip));
			}

			if(idx != selectedIndex)
			{
				Undo.RecordObject(collection, "Set Build Step Class Reference");

				if (idx == 0)
					pStep.SetStepType(null);
				else
					pStep.SetStepType(filtered [idx - 1].StepType);
			}

			return pStep;
		}

		GUIContent[] GetBuildStepProvidersFiltered()
		{
			List<GUIContent> outList = new List<GUIContent>();
			foreach (var bsp in _selectableBuildStepProviders)
			{
				if (bsp.Name == "None" || bsp.CheckFilters(_drawingBuildStepType, _editedBuildProcess.Platform))
				{
					string desc = bsp.GetDescription();
					if (desc != null)
						outList.Add(new GUIContent(bsp.ToMenuPath(), desc));
					else
						outList.Add(new GUIContent(bsp.ToMenuPath()));
				}
			}
			return outList.ToArray();
		}

		GUIContent[] GetBuildStepProvidersParameterOptions(BuildStepProviderEntry entry)
		{
			List<GUIContent> outList = new List<GUIContent>();
			if(entry != null)
			{
				string[] entries = entry.GetParameterDropdownOptions();
				foreach(var option in entries)
				{
					outList.Add(new GUIContent(option));
				}
			}
			
			return outList.ToArray();
		}

#region data manipulation

		private void CopyScenesFromSettings()
		{
			_editedBuildProcess.SceneAssets.Clear();
			var scenes = EditorBuildSettings.scenes;
			foreach (var scene in scenes)
			{
				var scenePath = scene.path;
				AddSceneAssetFromScenePath(scenePath);
			}
		}

		private void AddSceneAssetFromScenePath(string scenePath)
		{
			try
			{
				var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
				if (ReferenceEquals(null, sceneAsset))
					Debug.LogError($"Scene asset file not found at path {scenePath}");
				else
					_editedBuildProcess.SceneAssets.Add(sceneAsset);
			}
			catch (Exception exception)
			{
				Debug.LogError($"Error adding scene with path {scenePath}");
				Debug.LogException(exception);
			}
		}
		
		private void AddSceneAssetFromSceneGuid(string sceneGuid)
		{
			try
			{
				var scenePath = AssetDatabase.GUIDToAssetPath(sceneGuid);
				if (string.IsNullOrEmpty(scenePath))
					Debug.LogError($"Scene asset file not found with GUID {sceneGuid}");
				else
					AddSceneAssetFromScenePath(scenePath);
			}
			catch (Exception exception)
			{
				Debug.LogError($"Error adding scene with GUID {sceneGuid}");
				Debug.LogException(exception);
			}
		}

#endregion


#region platform specific stuff
		
		
		void DrawOutputPathSelector()
		{
			GUILayout.BeginHorizontal();
			{
				_editedBuildProcess.OutputPath = EditorGUILayout.TextField("Output Path", _editedBuildProcess.OutputPath);
				if (GUILayout.Button("...", GUILayout.Width(40)))
				{
					_editedBuildProcess.OutputPath = UBS.Helpers.GetProjectRelativePath(OpenPlatformSpecificOutputSelector());
				}
			}
			GUILayout.EndHorizontal();
		}
		string OpenPlatformSpecificOutputSelector()
		{
			const string kTitle = "Select Output Path";
			string path = UBS.Helpers.GetAbsolutePathRelativeToProject(_editedBuildProcess.OutputPath);

			switch (_editedBuildProcess.Platform)
			{
				
				case BuildTarget.Android: 
					return EditorUtility.SaveFilePanel(kTitle, path, "android", "apk");

				case BuildTarget.iOS:
					return EditorUtility.SaveFolderPanel(kTitle, path, "iOSDeployment");
					
				case BuildTarget.WSAPlayer:
					return EditorUtility.SaveFolderPanel(kTitle, path, "MetroDeployment");

				case BuildTarget.WebGL:
					return EditorUtility.SaveFolderPanel(kTitle, path, "WebGLDeployment");

				
				case BuildTarget.StandaloneOSX:
				case BuildTarget.StandaloneOSXIntel64:
				case BuildTarget.StandaloneOSXIntel:

				//
				// special handle .app folders for OSX
				//
					string suffix = "/" + PlayerSettings.productName + ".app";

					if (path.EndsWith(suffix))
						path = path.Substring(0, path.Length - 4);
					System.IO.DirectoryInfo fi = new System.IO.DirectoryInfo(path);
					Debug.Log(fi.Parent.ToString());

					string outString = EditorUtility.SaveFolderPanel(kTitle, fi.Parent.ToString(), "");

					if (!string.IsNullOrEmpty(outString))
					{
						if (!outString.EndsWith(suffix))
						{
							outString = outString + suffix;
						}
					}

					return outString;

				case BuildTarget.StandaloneLinux:
				case BuildTarget.StandaloneLinux64:
				case BuildTarget.StandaloneLinuxUniversal:
				
				case BuildTarget.StandaloneWindows:
				case BuildTarget.StandaloneWindows64:
				
					return EditorUtility.SaveFilePanel(kTitle, path, "StandaloneDeployment", "exe");
			}
			return "";
		}

#endregion

	}
}

