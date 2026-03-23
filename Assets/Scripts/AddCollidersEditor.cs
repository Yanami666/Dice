using UnityEngine;
using UnityEditor;


public class AddCollidersEditor
{
    [MenuItem("Tools/给台面加零摩擦材质")]
    static void ApplyMaterial()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null) { Debug.LogError("请先选中GilligansIsland"); return; }

        // 直接在代码里创建零摩擦材质
        PhysicsMaterial mat = new PhysicsMaterial("Surface");
        mat.dynamicFriction = 0f;
        mat.staticFriction = 0f;
        mat.bounciness = 0.3f;
        mat.frictionCombine = PhysicsMaterialCombine.Minimum;
        mat.bounceCombine = PhysicsMaterialCombine.Average;

        // 保存材质到Assets
        AssetDatabase.CreateAsset(mat, "Assets/Material/SurfaceAuto.physicsMaterial");

        // 给所有子物体碰撞体加上
        Collider[] cols = selected.GetComponentsInChildren<Collider>(true);
        foreach (Collider col in cols)
        {
            col.sharedMaterial = mat;
            EditorUtility.SetDirty(col.gameObject);
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"完成！给 {cols.Length} 个碰撞体加了零摩擦材质");
    }
}