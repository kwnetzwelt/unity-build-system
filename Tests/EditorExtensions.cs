using NUnit.Framework;
using UnityEditor;

namespace UBS.Tests
{
    public class EditorExtensions : TestBase
    {
        [Test]
        public void CreateCollectionMenu()
        {
            UBSAssetInspector.CreateBuildCollectionMenuCommand();
            BuildCollection collection = AssetDatabase.LoadAssetAtPath<BuildCollection>("Assets"+ UBSAssetInspector.DefaultBuildCollectionAssetName);
            Assert.That(collection, Is.Not.Null);
        }
    }
}