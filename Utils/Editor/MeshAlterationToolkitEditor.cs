using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(MeshAlterationToolkit), true), CanEditMultipleObjects]
public class MeshAlterationToolkitEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement container = new VisualElement();
        InspectorElement.FillDefaultInspector(container, serializedObject, this);
        PropertyField scriptPropertyField = container.Q<PropertyField>("PropertyField:m_Script");
        scriptPropertyField.SetEnabled(true);

        MeshAlterationToolkit meshAlterationToolkit = this.target as MeshAlterationToolkit;
        
        Button bakeScaleButton = new Button(() => meshAlterationToolkit.BakeScale());
        bakeScaleButton.text = "BakeScale";
        container.Add(bakeScaleButton);

        Button bakeRotationButton = new Button(() => meshAlterationToolkit.BakeRotation());
        bakeRotationButton.text = "BakeRotation";
        container.Add(bakeRotationButton);

        Button changeOriginButton = new Button(() => meshAlterationToolkit.ChangeOriginPos());
        changeOriginButton.text = "ChangeOrigin";
        container.Add(changeOriginButton);

        Button invertMeshButton = new Button(() => meshAlterationToolkit.InvertMesh());
        invertMeshButton.text = "InvertMesh";
        container.Add(invertMeshButton);

        Button saveMeshToAssetButton = new Button(() => meshAlterationToolkit.SaveMeshToAsset());
        saveMeshToAssetButton.text = "SaveMeshToAsset";
        container.Add(saveMeshToAssetButton);

        return container;
    }
}