using System;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UBS
{
    public class BuildVersion
    {        
        public enum BuildType
        {
            beta,
            final
        }
        const string kFileName = "Assets/Resources/buildVersion.txt";
        const string kPlainFileName = "Assets/Resources/buildVersionPlain.txt";
        public int major = 1;
        public int minor = 0;
        public int build = 0;
        public int revision = 0;
        public BuildType type = BuildType.beta;

        public string commitID = "";
        public string tagName = "";

        /// <summary>
        /// Increases the build revision. 
        /// </summary>
        public void Increase()
        {
            revision++;
        }

        public static BuildVersion Load()
        {
            #if UNITY_EDITOR && !UNITY_WEBPLAYER
            if(!Application.isPlaying)
            {
                return LoadEditor();
            }
            #endif

            var res = Resources.Load("buildVersion") as TextAsset;
            return Serializer.Load<BuildVersion>(res.text);
        }

		#if UNITY_EDITOR && !UNITY_WEBPLAYER
        static BuildVersion LoadEditor()
        {
            string content = null;
			
            if(!File.Exists(kFileName))
            {
                var bv = new BuildVersion();
                bv.Save();
            }
            content = File.ReadAllText(kFileName);
			
            return Serializer.Load<BuildVersion>(content);
        }

        public void Save()
        {
            // Save XML serialization
            var content = Serializer.Save(this);
            FileInfo fi = new FileInfo(kFileName);
            if(!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            File.WriteAllText(kFileName, content);
            #if UNITY_EDITOR
            AssetDatabase.ImportAsset(kFileName);
            #endif

            // While we're at it, write version as a string to a plain text file.
            // Useful for continuous integration tools etc.
            File.WriteAllText(kPlainFileName, ToLabelString());
            #if UNITY_EDITOR
            AssetDatabase.ImportAsset(kPlainFileName);
            #endif
        }
		#else
		public void Save()
		{
			throw new NotImplementedException("There is no implementation for saving files on webplayer. ");
		}
		#endif

        public static implicit operator System.Version(BuildVersion pVersion)
        {
            return new System.Version(pVersion.major, pVersion.minor, pVersion.build, pVersion.revision);
        }
        public override string ToString()
        {
            return ((System.Version)this).ToString();
        }
        public string ToLabelString()
        {
            return String.Format("{0}.{1}.{2}{3}{4}", major, minor, build, (type == BuildType.beta) ? "b" : "f", revision);
        }
    }
}