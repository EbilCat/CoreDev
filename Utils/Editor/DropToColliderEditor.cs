using CoreDev.Utils;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(DropToCollider), true), CanEditMultipleObjects]
public class DropToColliderEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement container = new VisualElement();
        InspectorElement.FillDefaultInspector(container, serializedObject, this);
        PropertyField scriptPropertyField = container.Q<PropertyField>("PropertyField:m_Script");
        scriptPropertyField.SetEnabled(true);

        DropToCollider dropToCollider = this.target as DropToCollider;
        Button dropToColliderButton = new Button(() => dropToCollider.Drop());
        dropToColliderButton.text = "Drop to Collider";
        container.Add(dropToColliderButton);

        return container;
    }
}