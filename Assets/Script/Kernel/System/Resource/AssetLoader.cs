#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;


// 直接读取asset, 只能用于editor
public class AssetLoader : IResourceLoader
{
    Dictionary<string, int> mAssetGroupMap = new Dictionary<string, int>();
    Dictionary<string, Dictionary<string, Object>> mAssetCache = new Dictionary<string, Dictionary<string, Object>>();

    public bool HasAssetBundle(string groupName)
    {
        return Directory.Exists(GetDirectoryByGroup(groupName));
    }
    public bool AsssetBundleLoaded(string groupName)
    {
        return mAssetGroupMap.ContainsKey(groupName);
    }
    public T CreateResource<T>(string resName, string groupName) where T : Object
    {
        // 先查找缓存
        T cacheRes = GetResource<T>(resName, groupName);
        if (cacheRes != null)
        {
            return cacheRes;
        }

        // 模拟加载过程
        if (mAssetGroupMap.ContainsKey(groupName))
        {
            string filename;
            if (FindFile(resName, GetDirectoryByGroup(groupName), out filename))
            {
                Object obj = AssetDatabase.LoadAssetAtPath<T>(filename);
                return (T)obj;
            }
        }
        Debug.LogWarning("resource is not exist:" + resName + ", group:" + groupName);
        return null;
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
    string GetDirectoryByGroup(string groupName)
    {
        string[] sp = groupName.Split(new char[] { '_' });
        if (sp.Length == 1)
        {
            return "Assets/" + sp[0] + "/";
        }
        else
        {
            return "Assets/" + sp[0] + "/" + sp[1] + "/";
        }
    }
    bool FindFile(string resName, string dir, out string fn)
    {
        //Debug.Log("dir:" + dir + "  resName:" + resName);
        string[] files = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
        foreach (string f in files)
        {
            if (Path.GetExtension(f) == ".meta") continue;
            string rn = Path.GetFileNameWithoutExtension(f);
            if (rn == resName)
            {
                //Debug.Log("dir:" + dir + "  resName:" + resName + "   f:" + f);
                fn = f;
                return true;
            }
        }
        fn = "";
        return false;
    }


    public bool CreateResourceAsync<T>(string resName, string groupName, ProgressCallback<T> cb) where T : Object
    {
        if (mAssetGroupMap.ContainsKey(groupName))
        {
            ResourceManager.GetSingleton().StartCoroutine(CreateResourceCoroutine(resName, groupName, cb));
            return true;
        }
        else
        {
            return false;
        }
        
    }
    IEnumerator CreateResourceCoroutine<T>(string resName, string groupName, ProgressCallback<T> cb) where T : Object
    {
        yield return null;
        T obj = CreateResource<T>(resName, groupName);
        if (obj == null)
        {
            cb.OnError(new System.Exception("no found res: " + resName + " in " + groupName), null);
        }
        else
        {
            cb.TouchFinish(obj);
        }
    }
    public List<string> GetAllAssetNames(string groupName)
    {
        List<string> allnames = new List<string>();
        string[] files = Directory.GetFiles(GetDirectoryByGroup(groupName), "*", SearchOption.AllDirectories);
        foreach (string f in files)
        {
            if (Path.GetExtension(f) == ".meta") continue;
            var name = f.Replace("\\", "/");
            allnames.Add(name);

        }
        return allnames;
    }
    public bool LoadAssetBundle(string groupName)
    {
        if (mAssetGroupMap.ContainsKey(groupName))
        {
            Debug.LogWarning("assetbundle already loaded:" + groupName);
            return false;
        }
        mAssetGroupMap.Add(groupName, 1);
        return true;
    }

    public void LoadAssetBundleAsync(string groupName, ProgressCallback<string> cb)
    {
        if (mAssetGroupMap.ContainsKey(groupName))
        {
            Debug.LogWarning("assetbundle already loaded:" + groupName);
        }
        else
        {
            ResourceManager.GetSingleton().StartCoroutine(LoadAssetBundleCoroutine(groupName, cb));
        }
    }
    IEnumerator LoadAssetBundleCoroutine(string groupName, ProgressCallback<string> cb)
    {
        yield return null;
        if (LoadAssetBundle(groupName))
        {
            cb.TouchFinish(groupName);
        }
        else
        {
            cb.TouchError(new System.ArgumentException());
        }
        
    }
    public void ReleaseAssetBundle(string groupName)
    {
        mAssetGroupMap.Remove(groupName);
        mAssetCache.Remove(groupName);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}

#endif