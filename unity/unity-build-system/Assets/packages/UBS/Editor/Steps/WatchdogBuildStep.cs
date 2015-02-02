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
			ExozetCSharpWatchdog cswd = new ExozetCSharpWatchdog();
			
			cswd.Init();
			
			cswd.Woff += Debug.LogError;

			foreach (string path in Directory.GetFiles(Path.Combine("Assets", "scripts"), "*.cs", SearchOption.AllDirectories))
			{
			    Debug.Log("Checking " + path);
			    
				cswd.Check(path);
			}
			
			WatchdogWindow w = ScriptableObject.CreateInstance<WatchdogWindow>();
			
			w.DisplayText = cswd.Summary();
			
			w.title = "CodeWatchdog Results";
			
			w.minSize = new Vector2(500, 300);
			
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
	        
			GUILayout.Label(DisplayText, GUILayout.Width(500));
			
			return;
	    }
	}
}

