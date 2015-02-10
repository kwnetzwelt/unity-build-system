using UnityEngine;
using UnityEditor;
using CodeWatchdog;
using System.IO;

namespace UBS
{
    /// <summary>
    /// A Buildstep for running the CodeWatchdog Coding Convention Compliance Checker on the C# source code of the project.
    /// </summary>
    public class WatchdogBuildStep : IBuildStepProvider
    {
        #region IBuildStepProvider implementation

        /// <summary>
        /// Called by the Buildsystem if your build step Provider is requested to start its build step
        /// </summary>
        public void BuildStepStart(BuildConfiguration pConfiguration)
        {
            CamelCaseCSharpWatchdog cswd = new CamelCaseCSharpWatchdog();
            
            cswd.Init();
            
            cswd.woff += Debug.LogError;

            // TODO: Offer configuration for directories and files to include / exclude.
            //
            foreach (string path in Directory.GetFiles(Path.Combine("Assets", "scripts"), "*.cs", SearchOption.AllDirectories))
            {
                Debug.Log("Checking " + path);
                
                cswd.Check(path);
            }
            
			WatchdogEditorWindow w = (WatchdogEditorWindow)EditorWindow.GetWindow(typeof(WatchdogEditorWindow));
            
            w.displayText = cswd.Summary();
            
            w.title = "CodeWatchdog Results";
            
            w.minSize = new Vector2(500, 500);
            
            w.Show();
            
            return;
        }
        
        /// <summary>
        /// Called as an update method every "frame" to allow continuous processing of async operations
        /// </summary>
        public void BuildStepUpdate()
        {
            return;
        }

        /// <summary>
        /// Determines whether this instance is done.
        /// </summary>
        public bool IsBuildStepDone()
        {
            return true;
        }

        #endregion
    }
    
    /// <summary>
    /// A window to display the CodeWatchdog results in the Unity editor.
    /// </summary>
    public class WatchdogEditorWindow : EditorWindow
    {
        public string displayText;
        
        void OnGUI()
        {
            GUILayout.Label("CodeWatchdog Results", EditorStyles.boldLabel);
            
            GUILayout.Label(displayText, GUILayout.Width(500));
            
            return;
        }
    }
}

