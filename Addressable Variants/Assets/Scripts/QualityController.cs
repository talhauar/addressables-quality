using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class QualityController : MonoBehaviour
{
    [SerializeField] private string key;
    [SerializeField] private Material mat;
    [SerializeField] private List<string> meshKeys = new List<string>();
    [SerializeField] private List<MeshFilter> meshFilters = new List<MeshFilter>();
    [SerializeField] private List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
    [SerializeField] private List<Mesh> temporaryMeshes = new List<Mesh>();
    
    private AsyncOperationHandle m_PrefabHandle;
    private Material m_Instance;
    
    private void Awake()
    {
        StartCoroutine(LoadFbx());
        LoadMaterial(key, ConvertEnumToAddressableLabel(QualityManager.Instance.CurrentQuality));
    }
    
    private IEnumerator LoadFbx()
    {
        for (int i = 0; i < meshKeys.Count; i++)
        {
            StartCoroutine(FetchMeshAndAssign(meshKeys[i], meshFilters[i]));
        }

        yield return null;
    }

    private IEnumerator FetchMeshAndAssign(string key ,MeshFilter filter)
    {
        AsyncOperationHandle<IList<IResourceLocation>> locationHandlec = Addressables.LoadResourceLocationsAsync((IEnumerable)new List<object> { "CruiserTank", "HighRes" }, 
            Addressables.MergeMode.Intersection);
        yield return locationHandlec;
        
        AsyncOperationHandle<IList<IResourceLocation>> locationHandle = Addressables.LoadResourceLocationsAsync((IEnumerable)new List<object> { "CruiserTank", "LowRes" }, 
            Addressables.MergeMode.Intersection);
        yield return locationHandle;
        
        Debug.Log($"Locatin Handle:{locationHandle.Result[0]} -- High: {locationHandlec.Result[0]}");
        AsyncOperationHandle<IList<IResourceLocation>> locationHandles = Addressables.LoadResourceLocationsAsync(locationHandle.Result[0]);
        yield return locationHandles;

       foreach (var rl in locationHandle.Result)
       {
           Debug.Log($"Path: {rl.ResourceType}");
       }
       foreach (var rl in locationHandles.Result)
       {
           Debug.Log($"Path Handles: {rl.ResourceType}");
       }
        Debug.Log($"Handles :{locationHandles.Result.Count} --- {locationHandles.Result[0]} --- {key}");
        
        var resourceLocation = locationHandles.Result[0];
        
        AsyncOperationHandle<Mesh> meshHandle = Addressables.LoadAssetAsync<Mesh>(resourceLocation);
        yield return meshHandle;
        Debug.Log($"{meshHandle.Result}");
        if (meshHandle.Status == AsyncOperationStatus.Succeeded)
        {
            filter.mesh = meshHandle.Result;
        }

        //Use this only when the objects are no longer needed
        Addressables.Release(locationHandle);
        Addressables.Release(meshHandle);
    }
    
    private void GetChildMeshRecursive(GameObject obj){
        if (null == obj)
            return;

        foreach (Transform child in obj.transform){
            if (null == child)
                continue;
            if (child.TryGetComponent<MeshFilter>(out var meshFilter))
            {
                meshFilters.Add(meshFilter);
                temporaryMeshes.Add(meshFilter.sharedMesh);
                meshKeys.Add($"{gameObject.name}[{child.name}]");
            }
            if(child.TryGetComponent<MeshRenderer>(out var meshRenderer)) meshRenderers.Add(meshRenderer);
            GetChildMeshRecursive(child.gameObject);
        }
    }

    private void Clear()
    {
        if (m_Instance != null)
        {
            Destroy(m_Instance);
            m_Instance = null;
        }

        if (m_PrefabHandle.IsValid())
            Addressables.Release(m_PrefabHandle);
    }
    
    private void LoadMaterial(string materialKey, string label)
    {
        Addressables.LoadAssetsAsync<Material>((IEnumerable)new List<object> { materialKey, label }, null,
            Addressables.MergeMode.Intersection).Completed += MaterialLoaded;
    }
    
    private void MaterialLoaded(AsyncOperationHandle<IList<Material>> obj)
    {
        Clear();
        AttachMaterial(obj);
    }

    private void AttachMaterial(AsyncOperationHandle<IList<Material>> obj)
    {
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.material = obj.Result[0];
        }
    }
    private string ConvertEnumToAddressableLabel(QualityType type)
    {
        switch (type)
        {
            case QualityType.Low:
                return "LowRes";
            case QualityType.Medium:
                return "MediumRes";
            case QualityType.High:
                return "HighRes";
            default:
                return "LowRes";
        }
    }

    [ContextMenu("Set Keys")]
    private void SetKeys()
    {
        meshFilters.Clear();
        meshKeys.Clear();
        meshRenderers.Clear();
        temporaryMeshes.Clear();

        key = gameObject.name;
        if (gameObject.TryGetComponent<MeshFilter>(out var meshFilter))
        {
            meshFilters.Add(meshFilter);
            temporaryMeshes.Add(meshFilter.sharedMesh);
            meshKeys.Add($"{gameObject.name}[{gameObject.name}]");
        }
        if(gameObject.TryGetComponent<MeshRenderer>(out var meshRenderer)) meshRenderers.Add(meshRenderer);
        GetChildMeshRecursive(gameObject);
    }
    
    [ContextMenu("Load Temporary Meshes")]
    public void LoadTemporaryMeshes()
    {
        for (int i = 0; i < meshFilters.Count; i++)
        {
            meshFilters[i].mesh = temporaryMeshes[i];
        }
    }
    
    [ContextMenu("Set All Meshes None")]
    public void SetAllMeshNone()
    {
        foreach (var meshFilter in meshFilters)
        {
            meshFilter.sharedMesh = null;
        }
    }
}
