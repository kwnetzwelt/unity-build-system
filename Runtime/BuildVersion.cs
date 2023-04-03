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
        const string FileName = "Assets/Resources/buildVersion.txt";
        const string PlainFileName = "Assets/Resources/buildVersionPlain.txt";
        public int major = 1;
        public int minor = 0;
        public int build = 0;
        public int revision = 0;
        public BuildType type = BuildType.beta;

        public string commitID = "";
        public string tagName = "";
        public string buildTimestamp = "";

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
			
            if(!File.Exists(FileName))
            {
                var bv = new BuildVersion();
                bv.Save();
            }
            content = File.ReadAllText(FileName);
			
            return Serializer.Load<BuildVersion>(content);
        }

        public void Save()
        {
            // Save XML serialization
            var content = Serializer.Save(this);
            FileInfo fi = new FileInfo(FileName);
            if(!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            File.WriteAllText(FileName, content);
            #if UNITY_EDITOR
            AssetDatabase.ImportAsset(FileName);
            #endif

            // While we're at it, write version as a string to a plain text file.
            // Useful for continuous integration tools etc.
            File.WriteAllText(PlainFileName, ToLabelString());
            #if UNITY_EDITOR
            AssetDatabase.ImportAsset(PlainFileName);
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
            return ((System.Version) this).ToString();
        }
        public string ToShortString()
        {
            return String.Format("{0}.{1}.{2}", major, minor, build);
        }
        public string ToLabelString()
        {
            return String.Format("{0}.{1}.{2}{3}{4}", major, minor, build, (type == BuildType.beta) ? "b" : "f", revision);
        }

        public void ParseFromBundleVersion(string bundleVersion)
        {
            try
            {
                var segments = bundleVersion.Split(".");
                if (segments.Length >= 3)
                {
                    major = int.Parse(segments[0]);
                    minor = int.Parse(segments[1]);
                    build = int.Parse(segments[2]);
                    if (segments.Length > 3)
                        revision = int.Parse(segments[3]);
                }
            }
            catch (Exception e)
            {
                throw new FormatException("Input string was not in the expected format {0}.{1}.{2}",e);
            }
                
        }
    }
}