using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using System.Linq;
#endif

[AddComponentMenu("Mesh/Invert Mesh Normals (CameraBlockerVRC)")]
[DisallowMultipleComponent]
public class InvertNormals : MonoBehaviour
{
#if UNITY_EDITOR
    private void OnEnable()
    {
        ApplyInversionIfNeeded();
    }

    public void ApplyInversionIfNeeded()
    {
        string assetDir = "Assets/InvertedMeshes";
        if (!AssetDatabase.IsValidFolder(assetDir))
            AssetDatabase.CreateFolder("Assets", "InvertedMeshes");

        foreach (MeshFilter mf in GetComponentsInChildren<MeshFilter>(true))
        {
            Mesh shared = mf.sharedMesh;
            if (shared == null) continue;

            string meshName = shared.name;
            if (meshName.EndsWith("_Inverted")) continue;

            Mesh newMesh = Instantiate(shared);
            InvertMesh(newMesh);
            newMesh.name = meshName + "_Inverted";

            string path = Path.Combine(assetDir, newMesh.name + ".asset");

            if (!File.Exists(path))
                AssetDatabase.CreateAsset(newMesh, path);
            else
                Debug.Log($"Skipped: {path} already exists.", mf);

            AssetDatabase.SaveAssets();
            mf.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            Debug.Log($"Inverted and assigned mesh: {path}", mf);
        }
    }

    private static bool hookInstalled = false;

    [InitializeOnLoadMethod]
    private static void RunAfterScriptsReload()
    {
        if (hookInstalled) return;
        hookInstalled = true;

        EditorApplication.delayCall += () =>
        {
            foreach (var obj in Object.FindObjectsOfType<InvertNormals>(true))
            {
                obj.ApplyInversionIfNeeded();
            }
        };
    }

    private static void InvertMesh(Mesh mesh)
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
