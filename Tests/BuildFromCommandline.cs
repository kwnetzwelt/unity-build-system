using Editor.UBS.Commandline;
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

        [Test]
        public void CommandLineParser_WithEmptyArgument_DoesNotThrow()
        {
            var arguments = new[] { "-collection", "Assets/Test.asset", "", "-batchmode" };
            Assert.DoesNotThrow(() =>
            {
                var parser = new CommandLineArgsParser(arguments);
                Assert.IsTrue(parser.Collection.HasArgument("collection"));
                Assert.AreEqual("Assets/Test.asset", parser.Collection.GetValue("collection"));
                Assert.IsTrue(parser.Collection.HasArgument("batchmode"));
            });
        }
    }
}