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
			case BuildTarget.BB10: return BuildTargetGroup.BB10;
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
			case BuildTarget.Wii:
				return BuildTargetGroup.Wii;
			case BuildTarget.WP8Player:
				return BuildTargetGroup.WP8;
			case BuildTarget.XBOX360:
				return BuildTargetGroup.XBOX360;
			}
			return BuildTargetGroup.Unknown;
		}
	}
}

