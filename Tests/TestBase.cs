using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace UBS.Tests
{
    public abstract class TestBase
    {
        
        [SetUp]
        public virtual void Setup()
        {
            AssetDatabase.DeleteAsset("Assets" + UBSAssetInspector.DefaultBuildCollectionAssetName);
        }

        [TearDown]
        public virtual void TearDown()
        {
            AssetDatabase.DeleteAsset("Assets" + UBSAssetInspector.DefaultBuildCollectionAssetName);
            AssetDatabase.DeleteAsset(TestCollectionLocation);
        }

        protected const string TestCollectionName = "TestCollection";
        protected const string TestCollectionLocation = "Assets/TestCollection.asset";
        
        protected static void SaveBuildCollection(BuildCollection collection)
        {
            AssetDatabase.SaveAssetIfDirty(collection);
        }
        
        protected static BuildCollection CreateTestBuildCollection()
        {
            var collection = ScriptableObject.CreateInstance<BuildCollection>();
            collection.name = TestCollectionName;
            AssetDatabase.CreateAsset(collection, TestCollectionLocation);
            return collection;
        }

        protected static BuildProcess CreateBuildProcess(BuildCollection collection = null)
        {
            var testProcess = new BuildProcess();
            testProcess.Pretend = true;
            if(collection is not null)
                collection.Processes.Add(testProcess);
            return testProcess;
        }
    }
}