﻿using System.Collections.Generic;
using UnityEngine;
using SG;

// [DisallowMultipleComponent]
// [AddComponentMenu("")]

public class ResourceRecycle : MonoBehaviour
{
    //obj pool
    private Dictionary<string, Pool> poolDict = new Dictionary<string, Pool>();

    private static ResourceRecycle mInstance = null;

    public static ResourceRecycle Instance
    {
        get
        {
            if (mInstance == null)
            {
                GameObject go = new GameObject("ResourceRecycle", typeof(ResourceRecycle));
                go.transform.localPosition = new Vector3(9999999, 9999999, 9999999);
                // Kanglai: if we have `GO.hideFlags |= HideFlags.DontSave;`, we will encounter Destroy problem when exit playing
                // However we should keep using this in Play mode only!
                mInstance = go.GetComponent<ResourceRecycle>();

                if (Application.isPlaying)
                {
                    DontDestroyOnLoad(mInstance.gameObject);
                }
                else
                {
                    Debug.LogWarning("[ResourceManager] You'd better ignore ResourceManager in Editor mode");
                }
            }

            return mInstance;
        }
    }
    public void InitPool(string poolName, int size, PoolInflationType type = PoolInflationType.DOUBLE, string GroupName = "")
    {
        if (poolDict.ContainsKey(poolName))
        {
            return;
        }
        else
        {
            GameObject pb = CreateUI(poolName, GroupName);
            if (pb == null)
            {
                Debug.LogError("[ResourceManager] Invalide prefab name for pooling :" + poolName);
                return;
            }
            poolDict[poolName] = new Pool(poolName, pb, gameObject, size, type);
        }
    }

    /// <summary>
    /// Returns an available object from the pool 
    /// OR null in case the pool does not have any object available & can grow size is false.
    /// </summary>
    /// <param name="poolName"></param>
    /// <returns></returns>
    public GameObject GetObjectFromPool(string poolName, bool autoActive = true, int autoCreate = 0)
    {
        GameObject result = null;

        if (!poolDict.ContainsKey(poolName) && autoCreate > 0)
        {
            InitPool(poolName, autoCreate, PoolInflationType.INCREMENT);
        }

        if (poolDict.ContainsKey(poolName))
        {
            Pool pool = poolDict[poolName];
            result = pool.NextAvailableObject(autoActive);
            //scenario when no available object is found in pool
#if UNITY_EDITOR
            if (result == null)
            {
                Debug.LogWarning("[ResourceManager]:No object available in " + poolName);
            }
#endif
        }
#if UNITY_EDITOR
        else
        {
            Debug.LogError("[ResourceManager]:Invalid pool name specified: " + poolName);
        }
#endif
        return result;
    }

    /// <summary>
    /// Return obj to the pool
    /// </summary>
    /// <param name="go"></param>
    public void ReturnObjectToPool(GameObject go)
    {
        PoolObject po = go.GetComponent<PoolObject>();
        if (po == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning("Specified object is not a pooled instance: " + go.name);
#endif
        }
        else
        {
            Pool pool = null;
            if (poolDict.TryGetValue(po.poolName, out pool))
            {
                pool.ReturnObjectToPool(po);
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogWarning("No pool available with name: " + po.poolName);
            }
#endif
        }
    }

    /// <summary>
    /// Return obj to the pool
    /// </summary>
    /// <param name="t"></param>
    public void ReturnTransformToPool(Transform t)
    {
        if (t == null)
        {
#if UNITY_EDITOR
            Debug.LogError("[ResourceManager] try to return a null transform to pool!");
#endif
            return;
        }
        ReturnObjectToPool(t.gameObject);
    }
    public void DeletePool(string poolname)
    {
        if (poolDict.ContainsKey(poolname))
        {
            var pool = poolDict[poolname];
            if (pool != null)
            {
                pool.OnDestroySelf();
                poolDict.Remove(poolname);
            }
        }
       
    }
    public GameObject CreateUI(string uiName, string GroupName)
    {
        GameObject go = GameObject.Instantiate(ResourceManager.GetSingleton().CreateResource<GameObject>(uiName, GroupName), Vector3.zero, Quaternion.identity);
        return go;
    }
}