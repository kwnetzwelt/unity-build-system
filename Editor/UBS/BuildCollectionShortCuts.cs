using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace UBS
{
    public class BuildCollectionShortCuts
    {

        [MenuItem("Window/UBS/Select Last Build Collection #%C", true)]
        static bool BuildCollectionAvailable()
        {
            return GetLastBuildCollection() != null;
        }

        [MenuItem("Window/UBS/Select Last Build Collection #%C")]
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
