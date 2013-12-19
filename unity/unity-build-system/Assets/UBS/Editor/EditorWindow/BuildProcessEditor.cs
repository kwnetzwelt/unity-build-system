using System;
using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;
using System.Collections.Generic;
using UnityEditor.Graphs;
using System.Linq;

namespace UBS
{
	public class BuildProcessEditor
	{
		static List<System.Type> mBuildStepProviders;
		string[] mSelectableBuildStepProviders;


		BuildProcess mEditedBuildProcess;
		BuildCollection mCollection;

		public BuildProcessEditor ()
		{
			//
			// create list of available Build SteP Providers
			//
			mBuildStepProviders = Helpers.FindClassesImplementingInterface(typeof(IBuildStepProvider));
#if UBS_DEBUG
			Debug.Log("Found " + mBuildStepProviders.Count + " BuildStepProviders");
#endif
			mSelectableBuildStepProviders = new string[mBuildStepProviders.Count+1];
			mSelectableBuildStepProviders[0] = "None";
			for(int i = 0;i<mBuildStepProviders.Count;i++)
			{
				mSelectableBuildStepProviders[i+1] = mBuildStepProviders[i].Name;
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
			if(mEditedBuildProcess != null)
				SaveScenesToStringList();

			mEditedBuildProcess = null;
		}

		public void OnGUI(BuildProcess pProcess, BuildCollection pCollection)
		{

			if(pProcess == null)
				return;

			if(pProcess != mEditedBuildProcess)
			{
				if(mEditedBuildProcess != null)
					SaveScenesToStringList();

				mEditedBuildProcess = pProcess;
				mCollection = pCollection;

				LoadScenesFromStringList();
			}

			GUILayout.BeginVertical();

			GUILayout.Label("Build Process", Styles.detailsTitle);

			Styles.HorizontalSeparator();
			
			Undo.RecordObject(mCollection, "Edit Build Process Details");
			pProcess.mName = EditorGUILayout.TextField("Name", mEditedBuildProcess.mName);


			mEditedBuildProcess.mPlatform = (BuildTarget)EditorGUILayout.EnumPopup( "Platform", mEditedBuildProcess.mPlatform );
			mEditedBuildProcess.mBuildOptions = (BuildOptions)EditorGUILayout.EnumMaskField( "Build Options", mEditedBuildProcess.mBuildOptions );
			DrawOutputPathSelector();


			ReorderableListGUI.Title("Included Scenes");
			ReorderableListGUI.ListField(mEditedBuildProcess.mSceneAssets, SceneDrawer);

			Styles.HorizontalSeparator();

			ReorderableListGUI.Title("Pre Build Steps");
			ReorderableListGUI.ListField(mEditedBuildProcess.mPreBuildSteps, StepDrawer);


			Styles.HorizontalSeparator();
			GUILayout.Label("Actual Unity Build", Styles.mediumHint);
			Styles.HorizontalSeparator();


			ReorderableListGUI.Title("Post Build Steps");
			ReorderableListGUI.ListField(mEditedBuildProcess.mPostBuildSteps, StepDrawer);

			
			GUILayout.EndVertical();

		}


		UnityEngine.Object SceneDrawer(UnityEngine.Rect pRect, UnityEngine.Object pScene)
		{

			var selected = EditorGUI.ObjectField(pRect, "Scene", pScene, typeof(UnityEngine.Object), false);

			if(selected != null)
			{
				var assetPath = AssetDatabase.GetAssetPath(selected);
				if(!assetPath.EndsWith(".unity"))
				{
					return pScene;
				}
			}
			if(selected != pScene)
				Undo.RecordObject(mCollection, "Set Scene Entry");
			return selected;

		}

		UBS.BuildStep StepDrawer(UnityEngine.Rect pRect, UBS.BuildStep pStep)
		{
			if(pStep == null)
				pStep = new BuildStep();




			int selected = 0; 
			if(pStep.mTypeName != null)
			{
				pStep.InferType();
				selected = mBuildStepProviders.IndexOf(pStep.mType) +1;
			}
			int idx = EditorGUI.Popup(pRect, "Class", selected, mSelectableBuildStepProviders);

			if(idx != selected)
			{
				Undo.RecordObject(mCollection, "Set Build Step Class Reference");

				if(idx == 0)
					pStep.SetType(null);
				else
					pStep.SetType(mBuildStepProviders[idx-1]);
			}

			return pStep;
		}
#region data manipulation

		public void SaveScenesToStringList()
		{
			mEditedBuildProcess.mScenes.Clear();

			for(int i = 0; i < mEditedBuildProcess.mSceneAssets.Count;i++)
			{
				mEditedBuildProcess.mScenes.Add( AssetDatabase.GetAssetPath( mEditedBuildProcess.mSceneAssets[i] ) );
			}
		}

		public void LoadScenesFromStringList()
		{
			mEditedBuildProcess.mSceneAssets.Clear();
			for(int i = 0;i< mEditedBuildProcess.mScenes.Count;i++)
			{
				try
				{
					mEditedBuildProcess.mSceneAssets.Add( AssetDatabase.LoadAssetAtPath( mEditedBuildProcess.mScenes[i],typeof(UnityEngine.Object) ) );
				}catch(Exception e)
				{
					Debug.LogError("Could not find scene file at: " + mEditedBuildProcess.mScenes[i] );
					Debug.LogException(e);
				}
			}
		}

#endregion


#region platform specific stuff
		
		
		void DrawOutputPathSelector ()
		{
			GUILayout.BeginHorizontal();
			{
				EditorGUILayout.LabelField("Output Path", mEditedBuildProcess.mOutputPath);
				if(GUILayout.Button("...", GUILayout.Width(40)))
				{
					mEditedBuildProcess.mOutputPath = OpenPlatformSpecificOutputSelector();
				}
			}
			GUILayout.EndHorizontal();
		}
		string OpenPlatformSpecificOutputSelector()
		{
			const string kTitle = "Select Output Path";
			
			switch(mEditedBuildProcess.mPlatform)
			{
				
			case BuildTarget.Android: 
				return EditorUtility.SaveFilePanel(kTitle, mEditedBuildProcess.mOutputPath, "android", "apk");
				
			case BuildTarget.iPhone:
				return EditorUtility.SaveFolderPanel(kTitle, mEditedBuildProcess.mOutputPath, "iOSDeployment");
				
			case BuildTarget.MetroPlayer:
				return EditorUtility.SaveFolderPanel(kTitle, mEditedBuildProcess.mOutputPath, "MetroDeployment");
				
			case BuildTarget.BB10:
				return EditorUtility.SaveFolderPanel(kTitle, mEditedBuildProcess.mOutputPath,"BlackBerryDeployment");
				
			case BuildTarget.NaCl:
				return EditorUtility.SaveFolderPanel(kTitle, mEditedBuildProcess.mOutputPath,"NativeClientDeployment");
				
			case BuildTarget.WebPlayer:
				return EditorUtility.SaveFolderPanel(kTitle, mEditedBuildProcess.mOutputPath,"WebPlayerDeployment");
				
				
			case BuildTarget.StandaloneOSXUniversal:
			case BuildTarget.StandaloneOSXIntel64:
			case BuildTarget.StandaloneOSXIntel:
				
			case BuildTarget.StandaloneLinux:
			case BuildTarget.StandaloneLinux64:
			case BuildTarget.StandaloneLinuxUniversal:
				
			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneWindows64:
				return EditorUtility.SaveFolderPanel(kTitle, mEditedBuildProcess.mOutputPath, "StandaloneDeployment");
				
			}
			return "";
		}

#endregion

	}
}

