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
		public BuildStepProviderEntry(System.Type pType)
		{
			if (pType == null)
			{
				mName = "None";
				return;
			}
			mType = pType;
			mName = mType.Name;
			mNamespace = mType.Namespace;

			foreach (var a in mType.GetCustomAttributes(true))
			{
				if (a is BuildStepDescriptionAttribute)
					mDescription = a as BuildStepDescriptionAttribute;
				else if (a is BuildStepPlatformFilterAttribute)
					mPlatformFilter = a as BuildStepPlatformFilterAttribute;
				else if (a is BuildStepTypeFilterAttribute)
					mTypeFilter = a as BuildStepTypeFilterAttribute;
				else if(a is BuildStepParameterFilterAttribute)
					mParameterFilter = a as BuildStepParameterFilterAttribute;

			}
		}
		public string mName;
		public string mNamespace;

		public System.Type mType;
		public BuildStepDescriptionAttribute mDescription;
		public BuildStepPlatformFilterAttribute mPlatformFilter;
		public BuildStepTypeFilterAttribute mTypeFilter;
		public BuildStepParameterFilterAttribute mParameterFilter;
		public override string ToString()
		{
			return mName;
		}

		public string ToMenuPath ()
		{
			if(mNamespace == null)
				return mName;

			return mNamespace.Replace(".","/") + "/" + mName;
		}

		public string GetDescription()
		{
			if (mDescription != null && mDescription.mDescription != null)
				return mDescription.mDescription;
			return "";
		}

		
		public EBuildStepParameterType GetParameterType()
		{
			if(mParameterFilter != null)
			{
				return mParameterFilter.BuildParameterType;
			}
			else
			{                
				// this is the default behavior, since the former buildsteps were designed as string parameters
				return EBuildStepParameterType.String;
			} 
		}
		
		public string[] GetParameterDropdownOptions()
		{
			if(mParameterFilter != null &&
			   mParameterFilter.BuildParameterType == EBuildStepParameterType.Dropdown)
			{
				return mParameterFilter.DropdownOptions;
			}
			else
			{
				return null;
			}
		}


		public bool CheckFilters(EBuildStepType pDrawingBuildStepType, BuildTarget pPlatform)
		{
			if (mPlatformFilter != null)
			{
				if (mPlatformFilter.mBuildTarget != pPlatform)
				{
					return false;
				}
			}
			if (mTypeFilter != null)
			{
				if (mTypeFilter.mBuildStepType != pDrawingBuildStepType)
				{
					return false;
				}
			}
			return true;
		}
	}

	public class BuildProcessEditor
	{
		EBuildStepType mDrawingBuildStepType = EBuildStepType.invalid;
		static List<System.Type> mBuildStepProviders;
		BuildStepProviderEntry[] mSelectableBuildStepProviders;

		bool mShowBuildOptions;
		BuildProcess mEditedBuildProcess;
		BuildCollection mCollection;

		public BuildProcessEditor()
		{
			//
			// create list of available Build SteP Providers
			//
			mBuildStepProviders = UBS.Helpers.FindClassesImplementingInterface(typeof(IBuildStepProvider));
#if UBS_DEBUG
			Debug.Log("Found " + mBuildStepProviders.Count + " BuildStepProviders");
#endif
			mSelectableBuildStepProviders = new BuildStepProviderEntry[mBuildStepProviders.Count + 1];
			mSelectableBuildStepProviders [0] = new BuildStepProviderEntry(null);
			for (int i = 0; i<mBuildStepProviders.Count; i++)
			{
				mSelectableBuildStepProviders [i + 1] = new BuildStepProviderEntry(mBuildStepProviders [i]);

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
			if (mEditedBuildProcess != null)
				SaveScenesToStringList();

			mEditedBuildProcess = null;
		}
        List<BuildOptions> mBuildOptions;
		string selectedOptionsString;

        private ReorderableList sceneList;
        private ReorderableList prebuildStepsList;
        private ReorderableList postbuildStepsList;

        void OnEnable()
		{
			var names = System.Enum.GetNames(typeof(BuildOptions));
			mBuildOptions = new List<BuildOptions>();

            mBuildOptions.Add(BuildOptions.Development);
            mBuildOptions.Add(BuildOptions.AutoRunPlayer);
            mBuildOptions.Add(BuildOptions.ShowBuiltPlayer);
            mBuildOptions.Add(BuildOptions.BuildAdditionalStreamedScenes);
            mBuildOptions.Add(BuildOptions.AcceptExternalModificationsToPlayer);
            mBuildOptions.Add(BuildOptions.ConnectWithProfiler);
            mBuildOptions.Add(BuildOptions.AllowDebugging);
            mBuildOptions.Add(BuildOptions.SymlinkLibraries);
            mBuildOptions.Add(BuildOptions.UncompressedAssetBundle);
            mBuildOptions.Add(BuildOptions.ConnectWithProfiler);
            mBuildOptions.Add(BuildOptions.ConnectToHost);
            mBuildOptions.Add(BuildOptions.EnableHeadlessMode);
            mBuildOptions.Add(BuildOptions.BuildScriptsOnly);
            mBuildOptions.Add(BuildOptions.ForceEnableAssertions);
            mBuildOptions.Add(BuildOptions.CompressWithLz4);
            mBuildOptions.Add(BuildOptions.StrictMode);
            
            UpdateSelectedOptions();

            sceneList = new ReorderableList(mEditedBuildProcess.mSceneAssets, typeof(SceneAsset));
            sceneList.drawHeaderCallback = SceneHeaderDrawer;
            sceneList.drawElementCallback = SceneDrawer;

            prebuildStepsList = new ReorderableList(mEditedBuildProcess.mPreBuildSteps, typeof(BuildStep));
            prebuildStepsList.drawHeaderCallback = PostStepHeaderDrawer;
            prebuildStepsList.drawElementCallback = PreStepDrawer;
            
            postbuildStepsList = new ReorderableList(mEditedBuildProcess.mPostBuildSteps, typeof(BuildStep));
            postbuildStepsList.drawHeaderCallback = PostStepHeaderDrawer;
            postbuildStepsList.drawElementCallback = PostStepDrawer;


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
			foreach (var buildOption in mBuildOptions)
			{
				if ((mEditedBuildProcess.mBuildOptions & buildOption) != 0)
				{
					sb.Append(buildOption.ToString() + ", ");
				}
			}
			selectedOptionsString = sb.ToString();
			if (selectedOptionsString.Length > 2)
				selectedOptionsString = selectedOptionsString.Substring(0, selectedOptionsString.Length - 2);
		}

		public void OnGUI(BuildProcess pProcess, BuildCollection pCollection)
		{

			if (pProcess == null)
				return;

			if (pProcess != mEditedBuildProcess)
			{
				if (mEditedBuildProcess != null)
					SaveScenesToStringList();

				mEditedBuildProcess = pProcess;
				mCollection = pCollection;

				LoadScenesFromStringList();
				OnEnable();
				
				// after switching to another process, we want to make sure to unfocus all possible controls
				// like textfields. This will remove an issue, where dangling focus between process switching could happen
				GUI.FocusControl("");

			}

			GUILayout.BeginVertical();

			GUILayout.Label("Build Process", Styles.detailsTitle);

			Styles.HorizontalSeparator();
			
			Undo.RecordObject(mCollection, "Edit Build Process Details");
			pProcess.mName = EditorGUILayout.TextField("Name", mEditedBuildProcess.mName);


			mEditedBuildProcess.mPlatform = (BuildTarget)EditorGUILayout.EnumPopup("Platform", mEditedBuildProcess.mPlatform);

			mEditedBuildProcess.mPretend = EditorGUILayout.Toggle(new GUIContent("Pretend Build", "Will not trigger a unity build, but run everything else. "), mEditedBuildProcess.mPretend);

			GUILayout.Space(5);
			mShowBuildOptions = EditorGUILayout.Foldout(mShowBuildOptions, "Build Options");
			GUILayout.BeginHorizontal();
			GUILayout.Space(25);

			if (mShowBuildOptions)
			{
				GUILayout.BeginVertical();

				foreach (var buildOption in mBuildOptions)
				{
					bool selVal = (mEditedBuildProcess.mBuildOptions & buildOption) != 0;
					{
						bool resVal = EditorGUILayout.ToggleLeft(buildOption.ToString(), selVal);
						if (resVal != selVal)
						{
							if (resVal)
								mEditedBuildProcess.mBuildOptions = mEditedBuildProcess.mBuildOptions | buildOption;
							else
								mEditedBuildProcess.mBuildOptions = mEditedBuildProcess.mBuildOptions & ~buildOption;
							UpdateSelectedOptions();
						}
					}
				}

				
				GUILayout.EndVertical();
			} else
			{
				GUILayout.Label(selectedOptionsString);
			}


			GUILayout.EndHorizontal();
			GUILayout.Space(5);

			DrawOutputPathSelector();
            DrawList(sceneList);

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			if (GUILayout.Button("Copy scenes from settings"))
				CopyScenesFromSettings();

			if (GUILayout.Button("Clear scenes"))
				mEditedBuildProcess.mSceneAssets.Clear();

			GUILayout.EndHorizontal();

			Styles.HorizontalSeparator();

			mDrawingBuildStepType = EBuildStepType.PreBuildStep;

            DrawList(prebuildStepsList);


			Styles.HorizontalSeparator();
			GUILayout.Label("Actual Unity Build", Styles.mediumHint);
			Styles.HorizontalSeparator();


			mDrawingBuildStepType = EBuildStepType.PostBuildStep;

            DrawList(postbuildStepsList);
			
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

            SceneAsset pScene = mEditedBuildProcess.mSceneAssets[index];
            var selected = EditorGUI.ObjectField(pRect, "Scene " + index, pScene, typeof(SceneAsset), false) as SceneAsset;
            
            if (selected != pScene)
                Undo.RecordObject(mCollection, "Set Scene Entry");

            mEditedBuildProcess.mSceneAssets[index] = selected;

		}
        void PreStepDrawer(Rect pRect, int index, bool isActive, bool isFocused)
        {
            UBS.BuildStep step = mEditedBuildProcess.mPreBuildSteps[index];
            step = StepDrawer(pRect, step);
            mEditedBuildProcess.mPreBuildSteps[index] = step;
        }

        void PostStepDrawer(Rect pRect, int index, bool isActive, bool isFocused)
        {
            UBS.BuildStep step = mEditedBuildProcess.mPostBuildSteps[index];
            step = StepDrawer(pRect, step);
            mEditedBuildProcess.mPreBuildSteps[index] = step;
        }

        UBS.BuildStep StepDrawer(Rect pRect, UBS.BuildStep pStep)
        {
            pRect.height = pRect.height - 4;
            pRect.y = pRect.y + 2;


            if (pStep == null)
				pStep = new BuildStep();

			var filtered = new List<BuildStepProviderEntry>(mSelectableBuildStepProviders);
			filtered = filtered.FindAll((obj) => {
				if (obj.mName == "None")
					return false;
				return obj.CheckFilters(mDrawingBuildStepType, mEditedBuildProcess.mPlatform);
			});
			
			int selectedIndex = 0; 
			int listIndex = 0; 
			if (pStep.mTypeName != null)
			{
				pStep.InferType();
				listIndex = filtered.FindIndex( (obj) => {return obj.mType == pStep.mType;});
				selectedIndex =  listIndex+1;
			}
			GUIContent[] displayedProviders = GetBuildStepProvidersFiltered();
			Rect r1 = new Rect(pRect.x, pRect.y + 1, 140, pRect.height); // drop down list
			Rect r2 = new Rect(r1.x + r1.width, pRect.y, 20, pRect.height); // gears
			Rect r3 = new Rect(r2.x + r2.width, pRect.y, 70, pRect.height); // parameters label
			Rect r4 = new Rect(r3.x + r3.width - 5, pRect.y, pRect.width - 230, pRect.height); // parameters input


			int idx = EditorGUI.Popup(r1, selectedIndex, displayedProviders);
			if (!EditorGUIUtility.isProSkin)
				GUI.color = Color.black;
			if (idx > 0 && GUI.Button(r2, Styles.gear, EditorStyles.miniLabel))
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
			GUI.Label(r3, "Parameters", EditorStyles.miniLabel);
			
			//r.x += r.width;

			// search for buildstepprovider
			EBuildStepParameterType parametersToDisplay = EBuildStepParameterType.None;
			BuildStepProviderEntry buildStepProvider = null;
			if(listIndex >= 0 && listIndex < filtered.Count())
			{                
				buildStepProvider = filtered[listIndex];
				parametersToDisplay = buildStepProvider.GetParameterType();
			}
			
			switch(parametersToDisplay)
			{
			case EBuildStepParameterType.None:
			{
				// dont show anything!
			}
				break;
            
            case EBuildStepParameterType.Boolean:
            {
                bool value;
                var succeeded = bool.TryParse(pStep.mParams, out value);
                pStep.mParams = Convert.ToString(succeeded ? EditorGUI.Toggle(r4, value) : EditorGUI.Toggle(r4, false));
            }
                break;
				
			case EBuildStepParameterType.String:
			{
				pStep.mParams = EditorGUI.TextField(r4, pStep.mParams );
			}
				break;
				
			case EBuildStepParameterType.Dropdown:
			{
				List<string> options = new List<string>(buildStepProvider.GetParameterDropdownOptions());                    
				int selectedValue = 0;
				if(!String.IsNullOrEmpty(pStep.mParams))
				{
					selectedValue = options.FindIndex((option) => {return option == pStep.mParams;});
				}
					
				if(selectedValue == -1) 
				{
					selectedValue = 0; // first index as fallback
					Debug.LogError("Invalid dropdown entry found: " + pStep.mParams + " for buildstep: " + buildStepProvider.mName + ". Fallback to index 0 applied!");
				}
				
				// create popup control and assign selected index
				int returnedIndex = EditorGUI.Popup(r4, selectedValue, GetBuildStepProvidersParameterOptions(buildStepProvider) );
				pStep.mParams = options[returnedIndex];
			}
				break;
			}
			
			if(idx != selectedIndex)
			{
				Undo.RecordObject(mCollection, "Set Build Step Class Reference");

				if (idx == 0)
					pStep.SetType(null);
				else
					pStep.SetType(filtered [idx - 1].mType);
			}

			return pStep;
		}

		GUIContent[] GetBuildStepProvidersFiltered()
		{
			List<GUIContent> outList = new List<GUIContent>();
			foreach (var bsp in mSelectableBuildStepProviders)
			{
				if (bsp.mName == "None" || bsp.CheckFilters(mDrawingBuildStepType, mEditedBuildProcess.mPlatform))
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

		void CopyScenesFromSettings()
		{
			mEditedBuildProcess.mScenes.Clear();
			EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
			foreach (EditorBuildSettingsScene scene in scenes)
			{
				mEditedBuildProcess.mScenes.Add(scene.path);
			}
			LoadScenesFromStringList();
		}

		public void SaveScenesToStringList()
		{
			mEditedBuildProcess.mScenes.Clear();

			for (int i = 0; i < mEditedBuildProcess.mSceneAssets.Count; i++)
			{
				mEditedBuildProcess.mScenes.Add(AssetDatabase.GetAssetPath(mEditedBuildProcess.mSceneAssets [i]));
			}
		}

		public void LoadScenesFromStringList()
		{
			mEditedBuildProcess.mSceneAssets.Clear();
			for (int i = 0; i< mEditedBuildProcess.mScenes.Count; i++)
			{
				try
				{
                    var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(mEditedBuildProcess.mScenes[i]);
					mEditedBuildProcess.mSceneAssets.Add(scene);
				} catch (Exception e)
				{
					Debug.LogError("Could not find scene file at: " + mEditedBuildProcess.mScenes [i]);
					Debug.LogException(e);
				}
			}
		}

#endregion


#region platform specific stuff
		
		
		void DrawOutputPathSelector()
		{
			GUILayout.BeginHorizontal();
			{
				mEditedBuildProcess.mOutputPath = EditorGUILayout.TextField("Output Path", mEditedBuildProcess.mOutputPath);
				if (GUILayout.Button("...", GUILayout.Width(40)))
				{
					mEditedBuildProcess.mOutputPath = UBS.Helpers.GetProjectRelativePath(OpenPlatformSpecificOutputSelector());
				}
			}
			GUILayout.EndHorizontal();
		}
		string OpenPlatformSpecificOutputSelector()
		{
			const string kTitle = "Select Output Path";
			string path = UBS.Helpers.GetAbsolutePathRelativeToProject(mEditedBuildProcess.mOutputPath);

			switch (mEditedBuildProcess.mPlatform)
			{
				
				case BuildTarget.Android: 
					return EditorUtility.SaveFilePanel(kTitle, path, "android", "apk");

				case BuildTarget.iOS:
					return EditorUtility.SaveFolderPanel(kTitle, path, "iOSDeployment");
					
				case BuildTarget.WSAPlayer:
					return EditorUtility.SaveFolderPanel(kTitle, path, "MetroDeployment");

				case BuildTarget.WebGL:
					return EditorUtility.SaveFolderPanel(kTitle, path, "WebGLDeployment");

				
				case BuildTarget.StandaloneOSXUniversal:
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

