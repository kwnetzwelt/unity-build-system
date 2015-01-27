using UnityEngine;
using UnityEditor;
using CodeWatchdog;
using System.IO;

namespace UBS
{
    public class WatchdogBuildStep : IBuildStepProvider
    {
        #region IBuildStepProvider implementation

        public void BuildStepStart(BuildConfiguration pConfiguration)
        {
			CSharpWatchdog cswd = new CSharpWatchdog();
			
			cswd.Init();
			
			cswd.Woff += Debug.LogError;

			foreach (string path in Directory.GetFiles(Path.Combine("Assets", "scripts"), "*.cs", SearchOption.AllDirectories))
			{
			    Debug.Log("Checking " + path);
			    
				cswd.Check(path);
			}
			
			WatchdogWindow w = ScriptableObject.CreateInstance<WatchdogWindow>();
			
			w.DisplayText = cswd.Summary();
			
			w.Show();
			
			return;
		}
		
        public void BuildStepUpdate()
        {
            return;
        }

        public bool IsBuildStepDone()
        {
            return true;
        }

        #endregion
	}
	
	public class WatchdogWindow : EditorWindow
	{
	    public string DisplayText;
	    
	    void OnGUI()
	    {
	        GUILayout.Label("CodeWatchdog Results", EditorStyles.boldLabel);
	        
			GUILayout.Label(DisplayText);
			
			return;
	    }
	}
}

