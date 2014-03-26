using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace UBS
{
	public class Helpers
	{


		public static List<System.Type> FindClassesImplementingInterface( System.Type pInterface)
		{
			List<System.Type> mOutList = new List<System.Type>();
			foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{

				foreach(var t in assembly.GetTypes())
				{

					if(t.GetInterfaces().Contains( pInterface ))
					{
						mOutList.Add(t);
					}

				}

			}
			return mOutList;
		}

		public static BuildTargetGroup GroupFromBuildTarget(BuildTarget pTarget)
		{

			switch( pTarget )
			{
			case BuildTarget.Android: return BuildTargetGroup.Android;
#if UNITY_4_6
			case BuildTarget.BlackBerry: return BuildTargetGroup.BlackBerry;
#else
			case BuildTarget.BB10: return BuildTargetGroup.BB10;
#endif
			case BuildTarget.FlashPlayer: return BuildTargetGroup.FlashPlayer;
			case BuildTarget.iPhone: return BuildTargetGroup.iPhone;
			case BuildTarget.MetroPlayer: return BuildTargetGroup.Metro;
			case BuildTarget.NaCl: return BuildTargetGroup.NaCl;
			case BuildTarget.PS3: return BuildTargetGroup.PS3;
			case BuildTarget.StandaloneGLESEmu:
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
			case BuildTarget.WebPlayer:
			case BuildTarget.WebPlayerStreamed:
				return BuildTargetGroup.WebPlayer;
#if !UNITY_4_6
			case BuildTarget.Wii:
				return BuildTargetGroup.Wii;
#endif
			case BuildTarget.WP8Player:
				return BuildTargetGroup.WP8;
			case BuildTarget.XBOX360:
				return BuildTargetGroup.XBOX360;
			}
			return BuildTargetGroup.Unknown;
		}

		public static string GetProjectRelativePath(string absolutePath)
		{	
			// Otherwise, the Uri will retain the Assets-relative fix
			Uri projectUri = new Uri(new Uri(UnityEngine.Application.dataPath + "/../").ToString());
			Uri targetUri = new Uri (absolutePath);
			return projectUri.MakeRelativeUri(targetUri).ToString ();
		}

		public static string GetAbsolutePathRelativeToProject(string relativePath) 
		{
			// Otherwise, the Uri will retain the Assets-relative fix
			Uri projectUri = new Uri(new Uri(UnityEngine.Application.dataPath + "/../").ToString());
			return new Uri (projectUri + relativePath).AbsolutePath;
		}

	}
}

