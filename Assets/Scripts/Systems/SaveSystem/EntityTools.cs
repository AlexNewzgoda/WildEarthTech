using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class EntityTools
{
    public static Entity InstantiateEntity (this MonoBehaviour monoBehaviour, AssetReference reference)
    {
        GameObject obj = reference.InstantiateAsync().WaitForCompletion();

        Entity entity;
        if (obj.TryGetComponent(out entity))
        {
            entity.SetNewGUID();
        }

        return entity;
    }

    public static bool SceneIsFirstLoaded (this MonoBehaviour monoBehaviour)
    {
        return GameSerializer.SceneIsFirstLoaded(monoBehaviour.gameObject.scene.name);
    }

    public static GameObject GetAssetObject (AssetReference reference)
    {
        AsyncOperationHandle<GameObject> handle = reference.LoadAssetAsync<GameObject>();
        handle.WaitForCompletion();

        GameObject go = handle.Result;

        Addressables.Release(handle);

        return go;
    }
}
