using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResourceLoader
{
    // 资源是否存在
    bool HasAssetBundle(string groupName);
    // 资源是否已经加载
    bool AsssetBundleLoaded(string groupName);
    // 阻塞加载
    bool LoadAssetBundle(string groupName);

    // 异步加载
    void LoadAssetBundleAsync(string groupName, ProgressCallback<string> cb);
    List<string> GetAllAssetNames(string groupName);

    // 阻塞创建
    T CreateResource<T>(string resName, string groupName) where T : UnityEngine.Object;

    // 异步创建资源
    bool CreateResourceAsync<T>(string resName, string groupName, ProgressCallback<T> cb) where T : UnityEngine.Object;

    // 释放资源
    void ReleaseAssetBundle(string groupName);
}
