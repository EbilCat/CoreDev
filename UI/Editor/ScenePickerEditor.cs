#if UNITY_EDITOR

using UnityEditor;

namespace CoreDev.UI
{
    [CustomEditor(typeof(ScenePicker), true)]
    public class ScenePickerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ScenePicker picker = target as ScenePicker;
            SceneAsset oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(picker.scenePath);

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            SceneAsset newScene = EditorGUILayout.ObjectField("Scene", oldScene, typeof(SceneAsset), false) as SceneAsset;

            if (EditorGUI.EndChangeCheck())
            {
                string newPath = AssetDatabase.GetAssetPath(newScene);
                SerializedProperty scenePathProperty = serializedObject.FindProperty("scenePath");
                scenePathProperty.stringValue = newPath;
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif