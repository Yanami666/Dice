using UnityEngine;
using UnityEditor;


public class AddCollidersEditor
{
    [MenuItem("Tools/给台面加零摩擦材质")]
    static void ApplyMaterial()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null) { Debug.LogError("请先选中pinnlend"); return; }

        // 检查有多少MeshFilter
        MeshFilter[] meshFilters = selected.GetComponentsInChildren<MeshFilter>(true);
        Debug.Log($"找到 {meshFilters.Length} 个MeshFilter");

        // 检查有多少Collider
        Collider[] existingCols = selected.GetComponentsInChildren<Collider>(true);
        Debug.Log($"已有 {existingCols.Length} 个Collider");

        PhysicsMaterial mat = new PhysicsMaterial("Surface");
        mat.dynamicFriction = 0f;
        mat.staticFriction = 0f;
        mat.bounciness = 0.3f;
        mat.frictionCombine = PhysicsMaterialCombine.Minimum;
        mat.bounceCombine = PhysicsMaterialCombine.Average;
        AssetDatabase.CreateAsset(mat, "Assets/Material/SurfaceAuto.physicsMaterial");

        int added = 0;
        foreach (MeshFilter mf in meshFilters)
        {
            if (mf.sharedMesh == null) continue;
            MeshCollider mc = mf.GetComponent<MeshCollider>();
            if (mc == null)
            {
                mc = Undo.AddComponent<MeshCollider>(mf.gameObject);
                mc.sharedMesh = mf.sharedMesh;
                added++;
            }
            mc.sharedMaterial = mat;
            EditorUtility.SetDirty(mf.gameObject);
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"新增了 {added} 个MeshCollider");
    }
}