using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager : MonoBehaviour
{
    private static ResourceManager _instance;
    public static ResourceManager Instance { get; private set; }

    // Cache AsyncOperationHandles required for releasing assets (Addressables.Release)
    private readonly Dictionary<string, AsyncOperationHandle> _handles = new();

    /// <summary>
    /// Automatically runs before the first scene loads to instantiate and register with DontDestroyOnLoad.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (_instance != null) return;

        var go = new GameObject("[ResourceManager]");
        _instance = go.AddComponent<ResourceManager>();
        DontDestroyOnLoad(go);
    }

    private void Awake()
    {
        // Guard against duplicate instances created manually
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Asynchronously loads an asset (Addressables address or key).
    /// </summary>
    /// <typeparam name="T">Type inheriting from UnityEngine.Object</typeparam>
    /// <param name="address">Addressables address or key</param>
    public async Task<T> LoadAsync<T>(string address) where T : Object
    {
        // 1. Check cache
        if (_handles.TryGetValue(address, out var existingHandle))
        {
            if (existingHandle.IsValid())
            {
                var resultAsset = await existingHandle.Task;
                if (resultAsset is T typedAsset)
                {
                    return typedAsset;
                }

                Logger.LogWarning($"[ResourceManager] Cached type mismatch: {address}");
                return null;
            }

            _handles.Remove(address);
        }

        // 2. Start asynchronous load via Addressables
        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(address);
        _handles[address] = handle;

        try
        {
            T asset = await handle.Task;
            return asset;
        }
        catch (System.Exception ex)
        {
            Logger.LogError($"[ResourceManager] Failed to load asset: {address}\n{ex.Message}");
            _handles.Remove(address);
            return null;
        }
    }

    /// <summary>
    /// Loads a prefab and instantiates it asynchronously.
    /// </summary>
    public async Task<GameObject> InstantiateAsync(string address, Transform parent = null, bool instantiateInWorldSpace = false)
    {
        try
        {
            var handle = Addressables.InstantiateAsync(address, parent, instantiateInWorldSpace);
            return await handle.Task;
        }
        catch (System.Exception ex)
        {
            Logger.LogError($"[ResourceManager] Failed to instantiate object: {address}\n{ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Destroys a GameObject instantiated via InstantiateAsync.
    /// </summary>
    public bool ReleaseInstance(GameObject instance)
    {
        if (instance == null)
        {
            return false;
        }

        return Addressables.ReleaseInstance(instance);
    }

    /// <summary>
    /// Unloads a specific cached asset from memory.
    /// </summary>
    public void Unload(string address)
    {
        if (_handles.TryGetValue(address, out var handle))
        {
            _handles.Remove(address);

            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }
    }

    /// <summary>
    /// Releases all cached assets.
    /// </summary>
    public void UnloadAll()
    {
        foreach (var handle in _handles.Values)
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }

        _handles.Clear();
    }

    private void OnDestroy()
    {
        // Automatically release all loaded handles when this manager is destroyed
        if (_instance == this)
        {
            UnloadAll();
            _instance = null;
        }
    }
}