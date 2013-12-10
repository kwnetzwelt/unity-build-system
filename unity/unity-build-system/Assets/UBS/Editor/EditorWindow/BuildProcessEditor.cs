using System;
using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;
using System.Collections.Generic;

namespace UBS
{
	public class BuildProcessEditor
	{
		static List<System.Type> mBuildStepProviders;

		string[] mSelectableBuildStepProviders;
		public BuildProcessEditor ()
		{
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

		}

		public void OnGUI(BuildProcess pProcess)
		{
			if(pProcess == null)
				return;
			GUILayout.BeginVertical();

			GUILayout.Label("Build Process", Styles.detailsTitle);

			Styles.HorizontalSeparator();

			pProcess.mName = EditorGUILayout.TextField("Name", pProcess.mName);

			Styles.HorizontalSeparator();

			ReorderableListGUI.Title("Pre Build Steps");
			ReorderableListGUI.ListField(pProcess.mPreBuildSteps, StepDrawer);


			Styles.HorizontalSeparator();
			GUILayout.Label("Actual Unity Build", Styles.mediumHint);
			Styles.HorizontalSeparator();


			ReorderableListGUI.Title("Post Build Steps");
			ReorderableListGUI.ListField(pProcess.mPostBuildSteps, StepDrawer);

			
			GUILayout.EndVertical();

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

	}
}

