using UnityEditor;

namespace UBS
{
    public static class SerializedPropertyExtensions
    {
        public static SerializedProperty FindPropertyByAutoPropertyName(this SerializedObject obj, string propName)
        {
            return obj.FindProperty($"<{propName}>k__BackingField");
        }
    }
}