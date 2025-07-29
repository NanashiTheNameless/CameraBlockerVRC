using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

[ExecuteInEditMode]
public class InvertNormalsComponent : MonoBehaviour
{
    public bool invertNow = false;

#if UNITY_EDITOR
    void Update()
    {
        if (!invertNow)
            return;

        invertNow = false;

        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null)
        {
            Debug.LogWarning("No MeshFilter or mesh found.");
            return;
        }

        Mesh newMesh = Instantiate(mf.sharedMesh);
        InvertMesh(newMesh);

        string assetDir = "Assets/InvertedMeshes";
        if (!AssetDatabase.IsValidFolder(assetDir))
            AssetDatabase.CreateFolder("Assets", "InvertedMeshes");

        string fileName = $"{gameObject.name}_Inverted.asset";
        string path = Path.Combine(assetDir, fileName);

        AssetDatabase.CreateAsset(newMesh, path);
        AssetDatabase.SaveAssets();

        mf.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        Debug.Log($"Inverted mesh saved and applied: {path}");
    }

    void InvertMesh(Mesh mesh)
    {
        Vector3[] normals = mesh.normals;
        for (int i = 0; i < normals.Length; i++)
            normals[i] = -normals[i];
        mesh.normals = normals;

        for (int s = 0; s < mesh.subMeshCount; s++)
        {
            int[] tris = mesh.GetTriangles(s);
            for (int i = 0; i < tris.Length; i += 3)
            {
                int tmp = tris[i];
                tris[i] = tris[i + 1];
                tris[i + 1] = tmp;
            }
            mesh.SetTriangles(tris, s);
        }

        mesh.RecalculateBounds();
    }
#endif
}
