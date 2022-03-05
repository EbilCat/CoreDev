using UnityEditor;
using UnityEngine;

public class MeshAlterationToolkit : MonoBehaviour
{
    private Vector3 objectWorldPos;
    private Vector3 objectEuler;


    [ContextMenu("BakeScale")]
    public void BakeScale()
    {
        this.SaveObjectPosAndRotation();
        this.transform.position = Vector3.zero;
        this.transform.eulerAngles = Vector3.zero;

        //Get MeshFilter
        MeshFilter meshFilter = this.GetComponent<MeshFilter>();
        if (meshFilter == null) { Debug.LogWarning("Baking of scale can only be used on an object which has a mesh"); return; }

        //Extract Mesh
        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3 currentScale = this.transform.localScale;

        //Apply scale to vertices of Mesh
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];
            vertex = new Vector3(vertex.x * currentScale.x, vertex.y * currentScale.y, vertex.z * currentScale.z);
            vertices[i] = vertex;
        }
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        this.RestoreObjectPosAndRotation();
        this.transform.localScale = Vector3.one;
    }


    [ContextMenu("BakeRotation")]
    public void BakeRotation()
    {
        Vector3 preBakePos = this.transform.position;
        Quaternion preBakeRot = this.transform.rotation;

        //Get MeshFilter
        MeshFilter meshFilter = this.GetComponent<MeshFilter>();
        if (meshFilter == null) { Debug.LogWarning("Baking of scale can only be used on an object which has a mesh"); return; }

        //Extract Mesh
        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3 currentScale = this.transform.localScale;

        //Apply scale to vertices of Mesh
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];
            vertex = preBakeRot * new Vector3(vertex.x * currentScale.x, vertex.y * currentScale.y, vertex.z * currentScale.z);
            vertices[i] = vertex;
        }
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        this.transform.position = preBakePos;
        this.transform.rotation = Quaternion.identity;
        this.transform.localScale = Vector3.one;
    }


    [ContextMenu("ChangeOriginPos")]
    public void ChangeOriginPos()
    {
        this.SaveObjectPosAndRotation();

        //Get MeshFilter
        MeshFilter meshFilter = this.GetComponent<MeshFilter>();
        if (meshFilter == null) { Debug.LogWarning("Changing of origin can only be used on an object which has a mesh"); return; }

        //Extract Mesh
        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;

        //Shift vertices
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];
            vertex = vertex + this.transform.position;
            vertices[i] = vertex;
        }
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        this.transform.position = Vector3.zero;
    }


    [ContextMenu("InvertMesh")]
    public void InvertMesh()
    {
        //Get MeshFilter
        MeshFilter meshFilter = this.GetComponent<MeshFilter>();
        if (meshFilter == null) { Debug.LogWarning("Inverting can only be used on an object which has a mesh"); return; }

        //Extract Mesh
        Mesh mesh = meshFilter.mesh;
        int[] triangles = mesh.triangles;
        int triangleCount = triangles.Length / 3;
        Vector3[] normals = mesh.normals;

        //Invert Triangles
        for (int i = 0; i < triangleCount; i++)
        {
            int firstVertIndex = i * 3;
            int firstVert = triangles[firstVertIndex];
            int thirdVertIndex = (i * 3) + 2;
            int thirdVert = triangles[thirdVertIndex];

            triangles[firstVertIndex] = thirdVert;
            triangles[thirdVertIndex] = firstVert;
        }

        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = -normals[i];
        }

        mesh.triangles = triangles;
        mesh.normals = normals;
    }


#if UNITY_EDITOR
    [ContextMenu("SaveMeshToAsset")]
    public void SaveMeshToAsset()
    {
        Mesh mesh;
        SkinnedMeshRenderer skinnedMeshRenderer = this.GetComponent<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer != null)
        {
            mesh = Instantiate(skinnedMeshRenderer.sharedMesh);
            if (skinnedMeshRenderer == null) { Debug.LogWarning("No mesh to save"); return; }
        }
        else
        {
            MeshFilter meshFilter = this.GetComponent<MeshFilter>();
            mesh = meshFilter.sharedMesh;
        }
        //Get MeshFilter

        string path = EditorUtility.SaveFilePanel("Save Mesh Asset", "Assets/", name, "asset");
        if (string.IsNullOrEmpty(path)) return;

        path = FileUtil.GetProjectRelativePath(path);

        AssetDatabase.CreateAsset(mesh, path);
        AssetDatabase.SaveAssets();
    }
#endif

    private void SaveObjectPosAndRotation()
    {
        this.objectWorldPos = this.transform.position;
        this.objectEuler = this.transform.eulerAngles;
    }

    private void RestoreObjectPosAndRotation()
    {
        this.transform.position = this.objectWorldPos;
        this.transform.eulerAngles = this.objectEuler;
    }
}
