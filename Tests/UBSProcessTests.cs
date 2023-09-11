using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UBS;
using UnityEditor;

namespace UBS.Tests
{
    
    public class UBSProcessTests
    {
        const string TestCollectionName = "TestCollection";
        
        [SetUp]
        public void Setup()
        {
            //Remove
            AssetDatabase.DeleteAsset(UBSProcess.ProcessPath);
        }
        
        [Test]
        public void CreateInstance()
        {
            var collection = ScriptableObject.CreateInstance<BuildCollection>();
            collection.name = TestCollectionName;
            var config = new UBSProcessConfiguration()
            {
                Collection = collection
            };
            UBSProcess.CreateFromConfig(config);
            var process = UBSProcess.LoadUBSProcess();
            Assert.NotNull(process);
            Assert.That(process.BuildCollection.name, Is.EqualTo(TestCollectionName));
        }
        
        


    }
}