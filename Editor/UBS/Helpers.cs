using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using System.Diagnostics;
using System.IO ;

namespace UBS
{
	internal class Helpers
	{
		public static Process ShellProcess(string fileName, string args)
		{
			Process p = new Process();
			p.StartInfo.Arguments = args;
			p.StartInfo.CreateNoWindow = true;
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.RedirectStandardInput = true;
			p.StartInfo.RedirectStandardError = true;
			p.StartInfo.FileName = fileName;
			p.Start();
			UnityEngine.Debug.Log("Executed shell: " + fileName);
			return p;
		}

		public static List<System.Type> FindClassesImplementingInterface(System.Type interfaceType)
		{
			List<System.Type> outList = new List<System.Type>();
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{

				foreach (var t in assembly.GetTypes())
				{

					if (t.GetInterfaces().Contains(interfaceType))
					{
						outList.Add(t);
					}

				}

			}
			return outList;
		}

		public static BuildTargetGroup GroupFromBuildTarget(BuildTarget target)
		{

			switch (target)
			{
			case BuildTarget.Android:
				return BuildTargetGroup.Android;

				
			case BuildTarget.iOS: return BuildTargetGroup.iOS;
			case BuildTarget.WSAPlayer: return BuildTargetGroup.WSA;
			case BuildTarget.WebGL: return BuildTargetGroup.WebGL;
            case BuildTarget.PS4:
				return BuildTargetGroup.PS4;
			case BuildTarget.StandaloneLinux64:
			case BuildTarget.StandaloneOSX:
			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneWindows64:
				return BuildTargetGroup.Standalone;
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
                var succeeded = (Enum.IsDefined(typeof (T), config.Parameters));
                if (succeeded)
                {
                    result = (T) Enum.Parse(typeof (T), config.Parameters, true);
                    return true;
                }
                else
                {
                    //UnityEngine.Debug.LogError("Unable to convert " + config.Parameters + " to " + typeof(T).Name);
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


        public static bool TryGetUnityObjectTypeFromBuildConfigurationParameter<T>(BuildConfiguration config, out T result) 
            where T : UnityEngine.Object
        {
            try
            {
                if (!String.IsNullOrEmpty(config.Parameters))
                {
                    int objectId;
                    UnityEngine.Object objectAssigned = null;
                    bool objectIdParseSucceeded = int.TryParse(config.Parameters, out objectId);
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

