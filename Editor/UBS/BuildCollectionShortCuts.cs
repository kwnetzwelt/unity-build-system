using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace UBS
{
    public class BuildCollectionShortCuts
    {

        [MenuItem("Window/UBS/Edit Last Build Collection %F11", true)]
        static bool EditBuildCollectionAvailable()
        {
            return GetLastBuildCollection() != null;
        }

        [MenuItem("Window/UBS/Edit Last Build Collection %F11")]
        public static void EditLastBuildCollection()
        {
            BuildCollection lastBuildcollection = GetLastBuildCollection();
            EditorGUIUtility.PingObject(lastBuildcollection);
            Selection.activeObject = lastBuildcollection;
            
            UBSEditorWindow.Init(lastBuildcollection);
        }
        
        [MenuItem("Window/UBS/Select Last Build Collection &%F11", true)]
        static bool SelectBuildCollectionAvailable()
        {
            return GetLastBuildCollection() != null;
        }

        [MenuItem("Window/UBS/Select Last Build Collection &%F11")]
        public static void ShowLastBuildCollection()
        {
            BuildCollection lastBuildcollection = GetLastBuildCollection();
            EditorGUIUtility.PingObject(lastBuildcollection);
            Selection.activeObject = lastBuildcollection;
        }

        private static BuildCollection GetLastBuildCollection()
        {
            var process = UBSProcess.LoadUBSProcess();
            if (process != null)
                return process.BuildCollection;
            return null;
        }
    }
}
