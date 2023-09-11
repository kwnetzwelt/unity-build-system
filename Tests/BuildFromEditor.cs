using NUnit.Framework;
using UnityEditor;

namespace UBS.Tests
{
    public class BuildFromEditor : TestBase
    {
        [Test]
        public void InspectorBuildSelected()
        {
            var collection = CreateTestBuildCollection();
            var process = CreateBuildProcess(collection);
            process.OutputPath = "test.apk";
            process.Platform = BuildTarget.Android;
            
            SaveBuildCollection(collection);
            
            var window = UBSBuildWindow.Create(collection);
            window.Initialize();
            while (!window.IsDone)
            {
                window.OnUpdate();
            }
            window.Close();
        }
    }
}