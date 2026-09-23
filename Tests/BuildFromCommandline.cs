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

        [Test]
        public void CommandLineParser_WithKeyValueSeparator_ParsesCorrectly()
        {
            var parserEquals = new CommandLineArgsParser(new[] { "-buildProcessByNames=Android,iOS" });
            Assert.IsTrue(parserEquals.Collection.HasArgument("buildProcessByNames"));
            Assert.AreEqual("Android,iOS", parserEquals.Collection.GetValue("buildProcessByNames"));

            var parserColon = new CommandLineArgsParser(new[] { "-buildProcessByNames:Android,iOS" });
            Assert.IsTrue(parserColon.Collection.HasArgument("buildProcessByNames"));
            Assert.AreEqual("Android,iOS", parserColon.Collection.GetValue("buildProcessByNames"));
        }

        [Test]
        public void CommandLineParser_WithMultipleValues_AppendsCommaSeparated()
        {
            var parser = new CommandLineArgsParser(new[] { "-buildProcessByNames", "Android", "iOS" });
            Assert.IsTrue(parser.Collection.HasArgument("buildProcessByNames"));
            Assert.AreEqual("Android,iOS", parser.Collection.GetValue("buildProcessByNames"));
        }

        [Test]
        public void BuildFromCommandLine_WithBuildProcessByNames_FiltersSelectedProcesses()
        {
            var collection = CreateTestBuildCollection();
            var p1 = CreateBuildProcess(collection);
            p1.Name = "Android_Dev";
            p1.Platform = BuildTarget.Android;
            p1.OutputPath = "builds/android.apk";
            p1.Pretend = true;

            var p2 = CreateBuildProcess(collection);
            p2.Name = "iOS_Release";
            p2.Platform = BuildTarget.iOS;
            p2.OutputPath = "builds/ios";
            p2.Pretend = true;

            var p3 = CreateBuildProcess(collection);
            p3.Name = "Standalone_Mac";
            p3.Platform = BuildTarget.StandaloneOSX;
            p3.OutputPath = "builds/mac.app";
            p3.Pretend = true;

            SaveBuildCollection(collection);

            var arguments = new[]
            {
                "-collection",
                TestCollectionLocation,
                "-buildProcessByNames",
                "Android_Dev, Standalone_Mac",
                "-batchmode"
            };

            UBSProcess.BuildFromCommandLine(arguments);

            var ubsProcess = UBSProcess.LoadUBSProcess();
            Assert.AreEqual(2, ubsProcess.config.SelectedBuildProcesses.Count);
            Assert.IsTrue(ubsProcess.config.SelectedBuildProcesses.Exists(p => p.Name == "Android_Dev"));
            Assert.IsTrue(ubsProcess.config.SelectedBuildProcesses.Exists(p => p.Name == "Standalone_Mac"));
            Assert.IsFalse(ubsProcess.config.SelectedBuildProcesses.Exists(p => p.Name == "iOS_Release"));
        }

        [Test]
        public void BuildFromCommandLine_WithSingleBuildProcessByName_FiltersSingleProcess()
        {
            var collection = CreateTestBuildCollection();
            var p1 = CreateBuildProcess(collection);
            p1.Name = "Android_Dev";
            p1.Platform = BuildTarget.Android;
            p1.OutputPath = "builds/android.apk";
            p1.Pretend = true;

            var p2 = CreateBuildProcess(collection);
            p2.Name = "iOS_Release";
            p2.Platform = BuildTarget.iOS;
            p2.OutputPath = "builds/ios";
            p2.Pretend = true;

            SaveBuildCollection(collection);

            var arguments = new[]
            {
                "-collection",
                TestCollectionLocation,
                "-buildProcessByNames",
                "Android_Dev",
                "-batchmode"
            };

            UBSProcess.BuildFromCommandLine(arguments);

            var ubsProcess = UBSProcess.LoadUBSProcess();
            Assert.AreEqual(1, ubsProcess.config.SelectedBuildProcesses.Count);
            Assert.AreEqual("Android_Dev", ubsProcess.config.SelectedBuildProcesses[0].Name);
        }
    }
}