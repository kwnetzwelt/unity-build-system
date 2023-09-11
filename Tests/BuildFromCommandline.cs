using NUnit.Framework;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

namespace UBS.Tests
{
    public class BuildFromCommandline : TestBase
    {
        
        [Test]
        public void SpecifyBuildCollection()
        {
            var collection = CreateTestBuildCollection();
            var process = CreateBuildProcess(collection);
            process.OutputPath = "test.apk";
            process.Platform = BuildTarget.Android;
            
            SaveBuildCollection(collection);
            var arguments = new []
            {
                "-collection", 
                TestCollectionLocation,
                "-buildAll",
                "-batchmode"
            };

            UBSProcess.BuildFromCommandLine(arguments);
        }

        
    }
}