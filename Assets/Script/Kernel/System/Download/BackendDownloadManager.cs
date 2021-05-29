using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class BackendDownloadManager : Singleton<BackendDownloadManager>, IManagerBase
{
    public bool Simulation = false;
    public bool EnableDownload = false;
    
    public const string PackageFilename = "packageinfo.json";
    public const string BackendDownloadQuest = "BackendDownloadQuest";
    public enum DownloadState
    {
        /// <summary>
        /// 未开始
        /// </summary>
        None,
        /// <summary>
        /// 正在下载
        /// </summary>
        Downloading,
        /// <summary>
        /// 下载完并验证完成
        /// </summary>
        Finished,
    }

    /// <summary>
    /// 本地下载路径
    /// </summary>
    public string LocalDownloadPath { get; private set; }
    /// <summary>
    /// 本地下载packagefile路径
    /// </summary>
    public string LocalPackageFilename { get; private set; }
    
    List<string> mDownloadQueue = new List<string>();
    /// <summary>
    /// 资源信息，来自源packagefile
    /// </summary>
    public class ResourceInfo
    {
        public string ResName { get; private set; }
        public long Len { get; private set; }
        public string MD5 { get; private set; }
        /// <summary>
        ///  已经检查过本地文件，并且文件正常
        /// </summary>
        public DownloadState State { get; private set; } 
        /// <summary>
        /// 
        /// </summary>
        public bool IsAborted { get; private set; }
        /// <summary>
        /// 获得下载对象
        /// </summary>
        public WaitForHttpProgressCallback<UnityWebRequest> DownloadCallback { get; private set; }

        public ResourceInfo(string resName, LitJson.JsonData info)
        {
            ResName = resName;
            Len = info["len"].GetLong();
            MD5 = info["md5"].GetString();

            string fullfilename = Path.Combine(GetSingleton().LocalDownloadPath, ResName +".info");
            if (File.Exists(fullfilename))
            {
                string infojs = File.ReadAllText(fullfilename);
                LitJson.JsonData infojd = LitJson.JsonMapper.ToObject(infojs);
                if (infojd["md5"].GetString() == MD5)
                {
                    State = DownloadState.Finished;
                    return;
                }
            }
            State = DownloadState.None; // 未开始
        }
        public WaitForHttpProgressCallback<UnityWebRequest> Download(string targetUrl)
        {
            State = DownloadState.Downloading;
            IsAborted = false;
            DownloadCallback = new WaitForHttpProgressCallback<UnityWebRequest>();

            DownloadCallback.ProgressCallback.OnError += (System.Exception e, object userData) =>
            {
                State = DownloadState.None;
            };
            HttpManager.GetSingleton().Download(targetUrl, GetSingleton().LocalDownloadPath + ResName, DownloadCallback.ProgressCallback);
            return DownloadCallback;
        }
        public DownloadState CheckFile()
        {
            string targetFilename = GetSingleton().LocalDownloadPath + ResName;
            if (File.Exists(targetFilename))
            {
                if (MD5Utility.GetFastMD5HashFromFile(targetFilename) == MD5)
                {
                    LitJson.JsonData infojd = new LitJson.JsonData();
                    infojd["md5"] = MD5;
                    File.WriteAllText(GetSingleton().LocalDownloadPath + ResName + ".info", infojd.ToJson());
                    State = DownloadState.Finished;
                    return State;
                }
                else
                {
                    Delete();
                }
            }
            State = DownloadState.None;
            return State;
        }
        public bool Delete()
        {
            string targetFilename = GetSingleton().LocalDownloadPath + ResName;
            if (File.Exists(targetFilename))
            {
                File.Delete(targetFilename);
                File.Delete(targetFilename + ".info");

                Debug.LogWarning("Delete res: " + ResName);
                return true;
            }
            return false;
        }
        public void Abort()
        {
            if (DownloadCallback != null && DownloadCallback.Request != null)
            {
                DownloadCallback.Request.Abort();
                IsAborted = true;
            }
        }
    }

    Dictionary<string, ResourceInfo> mResourceInfoMap = new Dictionary<string, ResourceInfo>();
    public void Initial()
    {
        LocalDownloadPath = Path.Combine(Application.persistentDataPath, "packages/");
        LocalPackageFilename = Path.Combine(LocalDownloadPath, PackageFilename);
    }

    public void Release()
    {
        
    }
    /// <summary>
    /// 加载资源下载列表
    /// </summary>
    /// <param name="packageUrl">cdn路径,应该以"/"结尾</param>
    public void LoadPackageFile(string packageUrl)
    {
        Debug.Log("Backend download: " + EnableDownload.ToString());
        if (!EnableDownload)
        {
            return;
        }
#if UNITY_EDITOR
        if (!Simulation)
        {
            return;
        }
#endif

        StartCoroutine(DelayLoadPackageFile(packageUrl));
    }
    IEnumerator DelayLoadPackageFile(string packageUrl)
    {
        string targetUrl = packageUrl + PackageFilename;
        string localDownloadFilename = LocalDownloadPath + PackageFilename;
        if (File.Exists(LocalPackageFilename))
        {
            // 如果文件存在，那么删除他，重新下载
            File.Delete(LocalPackageFilename);
        }
        int retryCount = 0;
        while (true)
        {
            var cb = new WaitForHttpProgressCallback<UnityWebRequest>();
            
            HttpManager.GetSingleton().Download(targetUrl, localDownloadFilename, cb.ProgressCallback);

            yield return cb;

            if (cb.Error != null)
            {
                Debug.LogError("package file load error: " + cb.Error.Message);
                // 如果重试次数超过3次，那么改为10秒重试一次
                if (retryCount >= 3)
                {
                    yield return new WaitForSeconds(10.0f);
                }
                else
                {
                    yield return new WaitForSeconds(1.0f);
                }
                retryCount++;
                continue;
            }
            
            if (!File.Exists(LocalPackageFilename))
            {
                Debug.LogError("packagefile not exist.");
                yield return new WaitForSeconds(1.0f);
                continue;
            }
            break;
        }
        // 读取源资源列表文件
        string packageFilejs = File.ReadAllText(LocalPackageFilename);
        LitJson.JsonData packageFilejd = LitJson.JsonMapper.ToObject(packageFilejs);

        if (packageFilejd.ContainsKey("filelist"))
        {
            LitJson.JsonData filelistjd = packageFilejd["filelist"];
            foreach (var i in filelistjd.Keys)
            {
                // 创建ResourceInfo
                ResourceInfo info = new ResourceInfo(i, filelistjd[i]);
                mResourceInfoMap.Add(i, info);
            }
        }

        // 开始下载
        while (true)
        {
            yield return new WaitUntil(() => { return mDownloadQueue.Count != 0; });

            var info = GetResourceInfo(mDownloadQueue[0]);
            if (info == null)
            {
                Debug.LogError("ResourceInfo not exist: " + mDownloadQueue[0]);
                mDownloadQueue.RemoveAt(0);
                yield return null;
                continue;
            }
            if (info.State != DownloadState.None)
            {
                Debug.Log("ResourceInfo has done: " + mDownloadQueue[0]);
                mDownloadQueue.RemoveAt(0);
                yield return null;
                continue;
            }

            targetUrl = packageUrl + info.ResName;

            yield return info.Download(targetUrl);

            if (info.DownloadCallback.Error != null)
            {
                Debug.LogError("Failed to download: " + info.DownloadCallback.Error.Message);
            }
            else
            {
                if (info.CheckFile() == DownloadState.Finished)
                {
                    if (info.IsAborted)
                    {
                        // 看看有没有下好了，还被ablort的info
                        Debug.LogWarning("Abort a complete download: " + info.ResName);
                    }

                    mDownloadQueue.RemoveAt(0);
                    Debug.Log("Success to CheckFile: " + info.ResName);
                    continue;
                }
                else
                {
                    Debug.LogError("Failed to CheckFile: " + info.ResName);
                }
            }

            yield return new WaitForSeconds(5.0f);

            // 如果上一个下载是用户取消的
            if (info.IsAborted)
            {
                // 如果这个错误是由于用户取消造成的，那么放最后面去
                string resName = mDownloadQueue[0];
                mDownloadQueue.RemoveAt(0);
                mDownloadQueue.Add(resName);
            }
            continue;
            
        }
    }
    /// <summary>
    /// 下载指定的资源，如果资源不存在，那么返回空
    /// 如果资源已经存在，那么不会加入
    /// </summary>
    /// <param name="resName"></param>
    /// <returns></returns>
    public ResourceInfo AddDownloadQueue(string resName)
    {
        var info = GetResourceInfo(resName);
        if (info == null)
        {
            Debug.LogError("res not exist: " + resName);
            return null;
        }

        if (!mDownloadQueue.Exists((string s) => { return s == resName; }))
        {
            Debug.Log("Download Queue added: " + resName);
            mDownloadQueue.Add(resName);
        }

        return info;
    }
    /// <summary>
    /// 插入下载到队列最前面，如果当前有正在下载，那么放到下一个
    /// 如果资源已经存在，那么删除，并重新插入
    /// </summary>
    /// <param name="resName"></param>
    /// <returns></returns>
    public ResourceInfo InsertDownloadToNext(string resName)
    {
        var info = GetResourceInfo(resName);
        if (info == null)
        {
            Debug.LogError("res not exist: " + resName);
            return null;
        }
        if (info.State != DownloadState.None)
        {
            Debug.LogWarning("res state :" + info.State.ToString());
            return info;
        }
        // 获取正在下载的资源
        info = GetDownloadingResInfo();
        if (info == null)
        {
            // 队列是空的，那么直接加入
            Debug.Log("Download Queue added: " + resName);
            mDownloadQueue.Add(resName);
        }
        else
        {
            // 删除队列后面（除第一个）已经存在的resName
            for (int i = mDownloadQueue.Count - 1; i > 0; i--)
            {
                if (mDownloadQueue[i] == resName)
                {
                    mDownloadQueue.RemoveAt(i);
                }
            }

            if (info.State == DownloadState.None)
            {
                // 如果第一个还没开始下载，那么放到第一个
                Debug.Log("Download Queue insert first: " + resName);
                mDownloadQueue.Insert(0, resName);
            }
            else
            {
                // 如果正在下载或者已经完成，那么放到第二个
                if (mDownloadQueue.Count > 1)
                {
                    Debug.Log("Download Queue insert second: " + resName);
                    mDownloadQueue.Insert(1, resName);
                }
                else
                {
                    Debug.Log("Download Queue added: " + resName);
                    mDownloadQueue.Add(resName);
                }
            }
        }
        return info;
    }
    /// <summary>
    /// 获得正在下载或即将下载的ResourceInfo
    /// 返回空说明队列是空的
    /// </summary>
    /// <returns></returns>
    public ResourceInfo GetDownloadingResInfo()
    {
        if (mDownloadQueue.Count == 0)
        {
            return null;
        }

        return GetResourceInfo(mDownloadQueue[0]);
    }
    public void AbortDownloading()
    {
        var info = GetDownloadingResInfo();
        if (info != null && info.State == DownloadState.Downloading)
        {
            info.Abort();
        }
    }
    public ResourceInfo GetResourceInfo(string name)
    {
        ResourceInfo info;
        if (mResourceInfoMap.TryGetValue(name, out info))
        {
            return info;
        }
        return null;
    }
    public bool RemoveResource(string resName)
    {
        var info = GetResourceInfo(resName);
        if (info == null)
        {
            return false;
        }

        info.Delete();
        info.CheckFile();
        return true;
    }
}
