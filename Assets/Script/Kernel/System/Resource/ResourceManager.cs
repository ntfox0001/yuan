using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[ExecuteInEditMode]
public class ResourceManager : Singleton<ResourceManager>, IManagerBase, IResourceLoader
{
    public bool UseAssetBundle;

    AssetBundleLoadHelper mAssetBundleLoadHelper = new AssetBundleLoadHelper();
    public AssetBundleLoadHelper AssetBundleLoadHelper
    {
        get { return mAssetBundleLoadHelper; }
    }

    IResourceLoader mResourceLoader;
    public void Initial()
    {

#if !UNITY_EDITOR
        UseAssetBundle = true;
#endif

        if (UseAssetBundle)
        {
            mResourceLoader = new AssetBundleLoader();
        }
#if UNITY_EDITOR
        else
        {
            mResourceLoader = new AssetLoader();
        }
#endif

    }
    public void Release()
    {

    }
    /// <summary>
    /// 资源组文件是否存在
    /// </summary>
    /// <param name="groupName"></param>
    /// <returns></returns>
    public bool HasAssetBundle(string groupName)
    {
        return mResourceLoader.HasAssetBundle(groupName);
    }
    /// <summary>
    /// 资源组是否已经加载
    /// </summary>
    /// <param name="groupName"></param>
    /// <returns></returns>
    public bool AsssetBundleLoaded(string groupName)
    {
        return mResourceLoader.AsssetBundleLoaded(groupName);
    }
    /// <summary>
    /// 创建资源，阻塞调用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="resName">资源名</param>
    /// <param name="groupName">资源组名</param>
    /// <returns></returns>
    public T CreateResource<T>(string resName, string groupName) where T : Object
    {
        return mResourceLoader.CreateResource<T>(resName, groupName);
    }
    /// <summary>
    /// 异步创建资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="resName">资源名</param>
    /// <param name="groupName">资源组名</param>
    /// <param name="cb">回调</param>
    /// <returns></returns>
    public bool CreateResourceAsync<T>(string resName, string groupName, ProgressCallback<T> cb) where T : Object
    {
        return mResourceLoader.CreateResourceAsync<T>(resName, groupName, cb);
    }
    /// <summary>
    /// 获取资源组内所有资源名字
    /// </summary>
    /// <param name="groupName">资源组名</param>
    /// <returns></returns>
    public List<string> GetAllAssetNames(string groupName)
    {
        return mResourceLoader.GetAllAssetNames(groupName);
    }

    /// <summary>
    /// 阻塞加载资源组
    /// </summary>
    /// <param name="groupName">资源组名</param>
    /// <returns></returns>
    public bool LoadAssetBundle(string groupName)
    {
        return mResourceLoader.LoadAssetBundle(groupName);
    }
    /// <summary>
    /// 异步加载资源组
    /// </summary>
    /// <param name="groupName">资源组名</param>
    /// <param name="cb"></param>
    public void LoadAssetBundleAsync(string groupName, ProgressCallback<string> cb)
    {
//         var dl = GetResourceBackendDownload(groupName);
// 
//         if (dl == null)
//         {
        mResourceLoader.LoadAssetBundleAsync(groupName, cb);
//         }
//         else
//         {
//             switch (dl.State)
//             {
//                 case ResourceBackendDownload.DownloadState.Finished:
//                     {
//                         mResourceLoader.LoadAssetBundleAsync(groupName, cb);
//                         break;
//                     }
//                 case ResourceBackendDownload.DownloadState.WaitStart:
//                     {
//                         dl.ProgressCallback.OnFinish += (string filename, object userData) =>
//                         {
//                             Debug.Log("LoadAssetBundleAsync is finished: " + groupName);
//                             mResourceLoader.LoadAssetBundleAsync(groupName, cb);
//                         };
//                         dl.Start();
//                         break;
//                     }
//                 case ResourceBackendDownload.DownloadState.Downloading:
//                     {
//                         Debug.Log("LoadAssetBundleAsync is downloading: " + groupName);
//                         dl.ProgressCallback.OnFinish += (string filename, object userData) =>
//                         {
//                             Debug.Log("LoadAssetBundleAsync is finished: " + groupName);
//                             mResourceLoader.LoadAssetBundleAsync(groupName, cb);
//                         };
//                         break;
//                     }
//                 case ResourceBackendDownload.DownloadState.Error:
//                     {
//                         cb.OnError(dl.Error, null);
//                         break;
//                     }
//                     
//             }
//         }
           
    }
    /// <summary>
    /// 释放资源组，立刻删除资源组对应的所有资源，包括正在使用的
    /// </summary>
    /// <param name="groupName">资源组名</param>
    public void ReleaseAssetBundle(string groupName)
    {
        mResourceLoader.ReleaseAssetBundle(groupName);
    }
}
