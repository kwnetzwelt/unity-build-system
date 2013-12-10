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

		string[] mBuildTargets;

		BuildProcess mEditedBuildProcess;

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

			mBuildTargets = System.Enum.GetNames(typeof( BuildTarget ));



		}

		public void OnGUI(BuildProcess pProcess)
		{

			if(pProcess == null)
				return;

			if(pProcess != mEditedBuildProcess)
			{
				mEditedBuildProcess = pProcess;
				LoadScenesFromStringList();
			}

			GUILayout.BeginVertical();

			GUILayout.Label("Build Process", Styles.detailsTitle);

			Styles.HorizontalSeparator();

			pProcess.mName = EditorGUILayout.TextField("Name", mEditedBuildProcess.mName);

			int platformIdx = mBuildTargets.ToList().IndexOf( mEditedBuildProcess.mPlatform );
			if(platformIdx == -1)
				platformIdx = 0;
			int idx = EditorGUILayout.Popup( "Platform", platformIdx, mBuildTargets );
			if(idx != platformIdx)
			{
				mEditedBuildProcess.mPlatform = mBuildTargets[idx];
			}

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
				if(!assetPath.EndsWith(".scene"))
					return pScene;
			}

			return selected;

		}

		UBS.BuildStep StepDrawer(UnityEngine.Rect pRect, UBS.BuildStep pStep)
		{
			if(pStep == null)
				pStep = new BuildStep();




			int selected = 0; 
			if(pStep.mTypeName != null)
			{
				selected = mBuildStepProviders.IndexOf(pStep.mType) +1;
			}
			int idx = EditorGUI.Popup(pRect, "Class", selected, mSelectableBuildStepProviders);

			if(idx != selected)
			{
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
	}
}

