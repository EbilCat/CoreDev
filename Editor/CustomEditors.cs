using CoreDev.Observable;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


namespace CoreDev.CustomEditors
{
/*
    This class makes all MonoBehaviour Inspectors render using UI Toolkit
    and will be removed in future when Unity inspectors use UI Toolkit for 
    inspectors by default
*/
    [CustomEditor(typeof(MonoBehaviour), true), CanEditMultipleObjects]
    public class MonoBehaviourEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement container = new VisualElement();
            InspectorElement.FillDefaultInspector(container, serializedObject, this);
            return container;
        }
    }


    [CustomPropertyDrawer(typeof(IObservableVar), true)]
    public class ObservableVarPropertyDrawer : PropertyDrawer
    {
        IObservableVar observableVar;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = new VisualElement();

            SerializedProperty serializedProperty = property.FindPropertyRelative("currentValue");
            string propertyPath = serializedProperty.propertyPath;
            string propertyName = propertyPath.Substring(0, propertyPath.IndexOf('.'));
            PropertyField propertyField = new PropertyField(serializedProperty, char.ToUpper(propertyName[0]) + propertyName.Substring(1));
            propertyField.SetEnabled(EditorApplication.isPlaying == false);
            container.Add(propertyField);

            return container;
        }
    }
}