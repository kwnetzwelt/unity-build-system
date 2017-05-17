using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using System.Diagnostics;
using System.IO;

namespace UBS
{
    internal class Helpers
    {
        public static Process ShellProcess(string pFilename, string pArgs)
        {
            Process p = new Process();
            p.StartInfo.Arguments = pArgs;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.FileName = pFilename;
            p.Start();
            UnityEngine.Debug.Log("Executed shell: " + pFilename);
            return p;
        }

        public static List<System.Type> FindClassesImplementingInterface(System.Type pInterface)
        {
            List<System.Type> mOutList = new List<System.Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {

                foreach (var t in assembly.GetTypes())
                {

                    if (t.GetInterfaces().Contains(pInterface))
                    {
                        mOutList.Add(t);
                    }

                }

            }
            return mOutList;
        }

        public static BuildTargetGroup GroupFromBuildTarget(BuildTarget pTarget)
        {

            switch (pTarget)
            {
                case BuildTarget.Android:
                    return BuildTargetGroup.Android;

#if !UNITY_5
				case BuildTarget.iPhone: return BuildTargetGroup.iPhone;
				case BuildTarget.NaCl: return BuildTargetGroup.NaCl;
				case BuildTarget.MetroPlayer: return BuildTargetGroup.Metro;
#else
                case BuildTarget.iOS: return BuildTargetGroup.iOS;
                case BuildTarget.WSAPlayer: return BuildTargetGroup.WSA;
                case BuildTarget.WebGL: return BuildTargetGroup.WebGL;

#endif

#if !UNITY_5_5_OR_NEWER
                case BuildTarget.PS3:
                    return BuildTargetGroup.PS3;
#endif
                case BuildTarget.PS4:
                    return BuildTargetGroup.PS4;
                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.StandaloneLinuxUniversal:
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSXUniversal:
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return BuildTargetGroup.Standalone;
                case BuildTarget.Tizen:
                    return BuildTargetGroup.Tizen;
#if !UNITY_5_4_OR_NEWER
			case BuildTarget.WebPlayer:
			case BuildTarget.WebPlayerStreamed:
				return BuildTargetGroup.WebPlayer;
#endif
#if !UNITY_5_4_OR_NEWER
			case BuildTarget.WP8Player:
				return BuildTargetGroup.WP8;
#endif
#if !UNITY_5_5_OR_NEWER
                case BuildTarget.XBOX360:
                    return BuildTargetGroup.XBOX360;
#endif
            }
            return BuildTargetGroup.Unknown;
        }

        public static string GetProjectRelativePath(string absolutePath)
        {
            // Otherwise, the Uri will retain the Assets-relative fix
            Uri projectUri = new Uri(new Uri(UnityEngine.Application.dataPath + "/../").ToString());
            Uri targetUri = new Uri(absolutePath);
            return projectUri.MakeRelativeUri(targetUri).ToString();
        }

        public static string GetAbsolutePathRelativeToProject(string relativePath)
        {
            // Otherwise, the Uri will retain the Assets-relative fix
            Uri projectUri = new Uri(new Uri(UnityEngine.Application.dataPath + "/../").ToString());

            // Use GetFullPath to resolve ../../ backreferences.
            // Hint from http://softwareblog.alcedo.com/post/2010/02/24/Resolving-relative-paths-in-C.aspx
            //
            return Path.GetFullPath(new Uri(projectUri + relativePath).AbsolutePath);
        }

        public static bool TryParseEnumFromBuildConfigurationParameter<T>(BuildConfiguration config, out T result)
        {
            try
            {
                var succeeded = (Enum.IsDefined(typeof (T), config.Params));
                if (succeeded)
                {
                    result = (T) Enum.Parse(typeof (T), config.Params, true);
                    return true;
                }
                else
                {
                    //UnityEngine.Debug.LogError("Unable to convert " + config.Params + " to " + typeof(T).Name);
                    result = default(T);
                    return false;
                }
            }
            catch
            {
                result = default(T);
                return false;
            }
            
        }


        public static bool TryGetUnityObjectTypeFromBuildConfigurationParameter<T>(BuildConfiguration config,
            out T result) where T : UnityEngine.Object
        {
            try
            {
                if (!String.IsNullOrEmpty(config.Params))
                {
                    int objectId;
                    UnityEngine.Object objectAssigned = null;
                    bool objectIdParseSucceeded = int.TryParse(config.Params, out objectId);
                    if (objectIdParseSucceeded)
                    {
                        objectAssigned = EditorUtility.InstanceIDToObject(objectId);
                    }

                    if (objectAssigned is T)
                    {
                        result = objectAssigned as T;
                        return true;
                    }
                }

                
                result = default(T);
                return false;
            }
            catch
            {
                result = default(T);
                return false;
            }
        }

    }
}

