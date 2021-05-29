using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_IOS && !UNITY_EDITOR
using UnityEngine.iOS;
#endif
public class AssetBundleLoader : IResourceLoader
{
    public const string StreamingListFilename = "streamingfiles.json";

    LitJson.JsonData mStreamingFiles = null;
    Dictionary<string, AssetBundle> mAssetGroupMap = new Dictionary<string, AssetBundle>();
    Dictionary<string, Dictionary<string, Object>> mAssetCache = new Dictionary<string, Dictionary<string, Object>>();
    string GetFilenameByGroup(string groupName)
    {
        string fpath = Path.Combine(Application.persistentDataPath, "packages/") + groupName;
        if (File.Exists(fpath))
        {
            Debug.Log("assetbundle in persistent: " + fpath);
            return fpath;
        }
        else
        {
            fpath = Path.Combine(Application.streamingAssetsPath, groupName);
            Debug.Log("assetbundle in streaming: " + fpath);
            return fpath;
        }
    }
    /// <summary>
    /// 在安卓下，无法返回assetbundle是否存在
    /// </summary>
    /// <param name="groupName"></param>
    /// <returns></returns>
    public bool HasAssetBundle(string groupName)
    {
#if UNITY_ANDROID
        if (mStreamingFiles == null)
        {
            string streamingfiles = Path.Combine(Application.streamingAssetsPath, StreamingListFilename);
            WWW www = new WWW(streamingfiles);
            while (!www.isDone)
            {

            }
            if (www.error == null)
            {
                mStreamingFiles = LitJson.JsonMapper.ToObject(www.text);
            }
        }

        if (mStreamingFiles != null)
        {
            bool rt = mStreamingFiles.ContainsKey(groupName);
            if (rt)
            {
                return true;
            }
        }
#endif
        return File.Exists(GetFilenameByGroup(groupName));


    }

    public bool AsssetBundleLoaded(string groupName)
    {
        return mAssetGroupMap.ContainsKey(groupName);
    }

    // 阻塞加载
    public bool LoadAssetBundle(string groupName)
    {
        string filename = GetFilenameByGroup(groupName);
        if (mAssetGroupMap.ContainsKey(groupName))
        {
            Debug.LogWarning("assetbundle already loaded:" + groupName);
            return false;
        }
        // 不存在那么就去加载
        AssetBundle ab = AssetBundle.LoadFromFile(filename);
        mAssetGroupMap.Add(groupName, ab);

        return true;
    }

    // 异步加载
    public void LoadAssetBundleAsync(string groupName, ProgressCallback<string> cb)
    {
        string filename = GetFilenameByGroup(groupName);
        if (mAssetGroupMap.ContainsKey(groupName))
        {
            Debug.LogWarning("assetbundle already loaded:" + groupName);
            //StartCoroutine(LoadAssetBundleAsync_CoroutineFinish(groupName, cb));
        }
        else
        {
            ResourceManager.GetSingleton().StartCoroutine(LoadAssetBundleAsync_Coroutine(filename, groupName, cb));
        }
    }

    public List<string> GetAllAssetNames(string groupName)
    {
        List<string> allnames = new List<string>();
        AssetBundle ab;
        if (mAssetGroupMap.TryGetValue(groupName, out ab))
        {
            allnames.AddRange(ab.GetAllAssetNames());
        }
        return allnames;
    }
    //IEnumerator LoadAssetBundleAsync_CoroutineFinish(string groupName, ProgressCallback<string> cb)
    //{
    //    yield return null;
    //    cb.TouchFinish(groupName);
    //}
    IEnumerator LoadAssetBundleAsync_Coroutine(string filename, string groupName, ProgressCallback<string> cb)
    {
        AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(filename);
        while (!req.isDone)
        {
            yield return null;
            cb.TouchProgress(req.progress);
        }
        yield return req;
        if (req.assetBundle == null)
        {
            cb.TouchError(new System.Exception("No exist to assetBundle: " + filename + ", groupName:" + groupName));
            Debug.LogError("No exist to assetBundle: " + filename + ", groupName:" + groupName);
        }
        else
        {
            mAssetGroupMap[groupName] = req.assetBundle;
            cb.TouchFinish(groupName);
        }
    }
    
    // 阻塞创建
    public T CreateResource<T>(string resName, string groupName) where T : UnityEngine.Object
    {
        // 先查找缓存
        T cacheRes = GetResource<T>(resName, groupName);
        if (cacheRes != null)
        {
            return cacheRes;
        }

        if (mAssetGroupMap.ContainsKey(groupName))
        {
            AssetBundle ab = mAssetGroupMap[groupName];
            T res = ab.LoadAsset<T>(resName);
            AddResource(resName, groupName, res);
            return res;
        }
        else
        {
            Debug.LogWarning("Res not found. resName:" + resName + " groupName:" + groupName);
            return null;
        }
    }
    T GetResource<T>(string resName, string groupName) where T : Object
    {
        if (mAssetCache.ContainsKey(groupName))
        {
            Dictionary<string, Object> v = mAssetCache[groupName];
            if (v != null)
            {
                if (v.ContainsKey(resName))
                {
                    T rt = v[resName] as T;
                    if (rt == null)
                    {
                        Debug.LogWarning("GetResource type error. " + v[resName].GetType());
                        return null;
                    }
                    else
                    {
                        return rt;
                    }
                }
            }
        }
        return null;
    }
    void AddResource(string resName, string groupName, Object d)
    {
        if (!mAssetCache.ContainsKey(groupName))
        {
            mAssetCache[groupName] = new Dictionary<string, Object>();
        }

        if (!mAssetCache[groupName].ContainsKey(resName))
        {
            mAssetCache[groupName][resName] = d;
        }
        else
        {
            Debug.LogWarning("resource already exist. resName:" + resName + " groupName:" + groupName);
        }
    }

    // 异步创建资源
    public bool CreateResourceAsync<T>(string resName, string groupName, ProgressCallback<T> cb) where T : UnityEngine.Object
    {
        T cacheRes = GetResource<T>(resName, groupName);
        if (cacheRes != null)
        {
            ResourceManager.GetSingleton().StartCoroutine(CreateresourceAsync_CoroutineFinish<T>(cacheRes, cb));
        }
        if (mAssetGroupMap.ContainsKey(groupName))
        {
            AssetBundle ab = mAssetGroupMap[groupName];
            if (ab == null)
            {
                Debug.LogError("CreateResourceAsync error: " + groupName + " is null.");
                return false;
            }
            else
            {
                ResourceManager.GetSingleton().StartCoroutine(CreateResourceAsync_Coroutine<T>(resName, groupName, ab, cb));
            }
            
            return true;
        }
        else
        {
            Debug.LogWarning("Res not found. resName:" + resName + " groupName:" + groupName);
            return false;
        }
    }
    IEnumerator CreateresourceAsync_CoroutineFinish<T>(T res, ProgressCallback<T> cb) where T : UnityEngine.Object
    {
        yield return null;
        cb.TouchFinish(res);
    }
    IEnumerator CreateResourceAsync_Coroutine<T>(string resName, string groupName, AssetBundle ab, ProgressCallback<T> cb) where T : UnityEngine.Object
    {
        AssetBundleRequest req = ab.LoadAssetAsync<T>(resName);
        while (!req.isDone)
        {
            yield return null;
            cb.TouchProgress(req.progress);
        }
        yield return req;
        AddResource(resName, groupName, req.asset);
        cb.TouchFinish(req.asset as T);

    }

    // 释放资源
    public void ReleaseAssetBundle(string groupName)
    {
        if (mAssetGroupMap.ContainsKey(groupName))
        {
            AssetBundle ab = mAssetGroupMap[groupName];
            mAssetGroupMap.Remove(groupName);
            ab.Unload(true);
            mAssetCache.Remove(groupName);
        }
        else
        {
            Debug.LogWarning("assetbundle does not exist:" + groupName);
        }
    }

}
