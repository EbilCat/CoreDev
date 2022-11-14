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
            PropertyField scriptPropertyField = container.Q<PropertyField>("PropertyField:m_Script");
            scriptPropertyField.SetEnabled(true);
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

            SerializedProperty serializedCurrentValue = property.FindPropertyRelative("currentValue");

            if (serializedCurrentValue != null)
            {
                string propertyPath = serializedCurrentValue.propertyPath;
                PropertyField propertyField = new PropertyField(serializedCurrentValue, property.displayName);
                propertyField.SetEnabled(EditorApplication.isPlaying == false);
                container.Add(propertyField);
            }
            else
            {
                Label label = new Label(property.displayName +" (Non-Serializable)");
                label.style.marginLeft = 3.0f;
                container.Add(label);
            }

            return container;
        }
    }
}