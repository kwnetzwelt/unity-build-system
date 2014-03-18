using System;
using UnityEngine;
using System.IO;

namespace UBS
{
	public class BuildVersion
	{
		const string kFileName = "Assets/Resources/buildVersion.txt";
		public int major = 1;
		public int minor = 0;
		public int build = 0;
		public int revision = 0;
		
		public void Increase()
		{
			build++;
		}


		public static BuildVersion Load()
		{
	#if UNITY_EDITOR
			if(!Application.isPlaying)
				return LoadEditor();
	#endif

			var res = Resources.LoadAssetAtPath<TextAsset>("buildVersion");
			return Serializer.Load<BuildVersion>(res.text);
		}

		#if UNITY_EDITOR
		static BuildVersion LoadEditor()
		{
			string content = null;
			
			if(!File.Exists(kFileName))
				return new BuildVersion();
			content = File.ReadAllText( kFileName);
			
			return Serializer.Load<BuildVersion>(content);
		}

		public void Save()
		{
			var content = Serializer.Save( this );
			FileInfo fi = new FileInfo(kFileName);
			if(!fi.Directory.Exists)
			{
				fi.Directory.Create();
			}
			File.WriteAllText( kFileName, content);
		}

		#endif

		#if !UNITY_EDITOR
		public void Save()
		{
			
		}
		#endif

		public static implicit operator System.Version(BuildVersion pVersion)
		{
			return new System.Version(pVersion.major, pVersion.minor, pVersion.build, pVersion.revision);
		}
		public override string ToString ()
		{
			return ((System.Version)this).ToString();
		}
	}
}