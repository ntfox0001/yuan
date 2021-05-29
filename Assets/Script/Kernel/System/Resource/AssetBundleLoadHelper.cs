using System.Collections;
using System.Collections.Generic;

// 嵌套调用LoadAssetBundle,ReleaseAssetBundle
// 必须成对调用
public class AssetBundleLoadHelper
{
    Dictionary<string, int> mAssetBundleCountMap = new Dictionary<string, int>();
    // 阻塞加载
    public bool LoadAssetBundle(string groupName)
    {
        if (mAssetBundleCountMap.ContainsKey(groupName))
        {
            mAssetBundleCountMap[groupName] = mAssetBundleCountMap[groupName] + 1;
        }
        else
        {
            ResourceManager.GetSingleton().LoadAssetBundle(groupName);
            mAssetBundleCountMap[groupName] = 1;
        }
        
        return true;
    }

    // 异步加载
    public void LoadAssetBundleAsync(string groupName, ProgressCallback<string> cb)
    {
        if (mAssetBundleCountMap.ContainsKey(groupName))
        {
            mAssetBundleCountMap[groupName] = mAssetBundleCountMap[groupName] + 1;
            ResourceManager.GetSingleton().StartCoroutine(LoadAssetBundleAsync_CoroutineFinish(groupName, cb));
        }
        else
        {
            ResourceManager.GetSingleton().LoadAssetBundleAsync(groupName, cb);
            mAssetBundleCountMap[groupName] = 1;
        }
    }
    IEnumerator LoadAssetBundleAsync_CoroutineFinish(string groupName, ProgressCallback<string> cb)
    {
        yield return null;
        cb.TouchFinish(groupName);
    }

    // 释放资源
    public void ReleaseAssetBundle(string groupName)
    {
        if (mAssetBundleCountMap.ContainsKey(groupName))
        {
            if (mAssetBundleCountMap[groupName] == 1)
            {
                ResourceManager.GetSingleton().ReleaseAssetBundle(groupName);
                mAssetBundleCountMap.Remove(groupName);
            }
            else
            {
                mAssetBundleCountMap[groupName] = mAssetBundleCountMap[groupName] - 1;
            }
            
        }
    }
}
