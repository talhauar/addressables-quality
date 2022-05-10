#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class QualityPrebuild : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    public int callbackOrder { get; }
    public void OnPostprocessBuild(BuildReport report)
    {
        Debug.Log($"Build Ended.(Quality Prebuild)");
        var paths = AssetDatabase.FindAssets("t:prefab", new string[] {"Assets/Prefabs/Cards"});

        foreach (var path in paths)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(path));
            if (prefab.TryGetComponent<QualityController>(out var qc))
            {
                qc.LoadTemporaryMeshes();
            }
        }
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log($"Build Started.(Quality Prebuild)");
        var paths = AssetDatabase.FindAssets("t:prefab", new string[] {"Assets/Prefabs/Cards"});

        foreach (var path in paths)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(path));
            if (prefab.TryGetComponent<QualityController>(out var qc))
            {
                qc.SetAllMeshNone();
            }
        }
    }
}

#endif
