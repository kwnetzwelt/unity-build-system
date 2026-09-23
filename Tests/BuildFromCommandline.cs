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

        [Test]
        public void CommandLineParser_WithDevelopmentBuild_ParsesCorrectly()
        {
            var arguments = new[] { "-collection", "Assets/Test.asset", "-batchmode", "-developmentBuild" };
            var parser = new CommandLineArgsParser(arguments);
            Assert.IsTrue(parser.Collection.HasArgument("developmentBuild"));
        }

        [Test]
        public void BuildFromCommandLine_WithDevelopmentBuild_SetsBuildOptionsDevelopment()
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
                "-batchmode",
                "-developmentBuild"
            };

            UBSProcess.BuildFromCommandLine(arguments);

            var ubsProcess = UBSProcess.LoadUBSProcess();
            Assert.IsTrue(ubsProcess.IsInBatchMode);
            Assert.IsTrue(ubsProcess.config.DevelopmentBuild);
            Assert.IsTrue((ubsProcess.GetBuildOptions(process) & BuildOptions.Development) != 0);
        }

        [Test]
        public void BuildFromCommandLine_WithoutDevelopmentBuild_DoesNotSetBuildOptionsDevelopment()
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

            var ubsProcess = UBSProcess.LoadUBSProcess();
            Assert.IsTrue(ubsProcess.IsInBatchMode);
            Assert.IsFalse(ubsProcess.config.DevelopmentBuild);
            Assert.IsFalse((ubsProcess.GetBuildOptions(process) & BuildOptions.Development) != 0);
        }

        [Test]
        public void BuildFromCommandLine_WithDevelopmentBuild_NotInBatchMode_DoesNotSetBuildOptionsDevelopment()
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
                "-developmentBuild"
            };

            UBSProcess.BuildFromCommandLine(arguments);

            var ubsProcess = UBSProcess.LoadUBSProcess();
            Assert.IsFalse(ubsProcess.IsInBatchMode);
            Assert.IsTrue(ubsProcess.config.DevelopmentBuild);
            Assert.IsFalse((ubsProcess.GetBuildOptions(process) & BuildOptions.Development) != 0);
        }
    }
}