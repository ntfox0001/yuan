using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System;
#if UNITY_IOS
using UnityEngine.iOS;
#endif
public class HttpManager : Singleton<HttpManager>, IManagerBase
{
    public const string ContentType_TextPlain = "text/plain";
    public const string ContentType_UrlEncoded = "application/x-www-form-urlencoded";
    public const string ContentType_FormData = "multipart/form-data";
    public const string ContentType_Json = "application/json";
    public const string ContentType_OctetStream = "application/octet-stream";

    public void HttpGet(string url, Dictionary<string,string> header, ProgressCallback<UnityWebRequest> cb, int timeout = 0)
    {
        if (InternetReachable())
        {
            StartCoroutine(WebGet(url, header, cb, timeout));
        }
        else
        {
            StartCoroutine(DelayError(cb));
        }
    }
    private IEnumerator WebGet(string url, Dictionary<string, string> header, ProgressCallback<UnityWebRequest> cb, int timeout)
    {
        UnityWebRequest req = UnityWebRequest.Get(url);
        req.timeout = timeout;
        if (header != null)
        {
            foreach (var i in header)
            {
                req.SetRequestHeader(i.Key, i.Value);
            }
        }
        cb.TouchBegin(req);
        req.SendWebRequest();
        while (!req.isDone)
        {
            yield return null;
            cb.TouchProgress(req.downloadProgress);
        }

        if (req.isNetworkError || req.isHttpError)
        {
            string errStr = "WebRequest get error:" + req.error + " url:" + url;
            cb.TouchError(new System.Exception(errStr));
            Debug.LogError(errStr);
        }
        else
        {
            cb.TouchFinish(req);
            req.Dispose();
            req = null;
        }
    }
    public void HttpPost(string url, string contentType, byte[] data, Dictionary<string, string> header, ProgressCallback<UnityWebRequest> cb, int timeout = 0)
    {
        if (InternetReachable())
        {
            StartCoroutine(WebPost(url, contentType, data, header, cb, timeout));
        }
        else
        {
            StartCoroutine(DelayError(cb));
        }

    }
    private IEnumerator WebPost(string url, string contentType, byte[] data, Dictionary<string, string> header, ProgressCallback<UnityWebRequest> cb, int timeout)
    {
        UnityWebRequest req = new UnityWebRequest();
        req.timeout = timeout;
        req.method = "POST";
        req.uri = new Uri(url);
        req.SetRequestHeader("Content-Type", contentType);
        if (header != null)
        {
            foreach (var i in header)
            {
                req.SetRequestHeader(i.Key, i.Value);
            }
        }

        req.uploadHandler = new UploadHandlerRaw(data);
        req.downloadHandler = new DownloadHandlerBuffer();

        cb.TouchBegin(req);
        req.SendWebRequest();

        while (!req.isDone)
        {
            yield return null;
            cb.TouchProgress(req.downloadProgress);
        }

        if (req.isNetworkError || req.isHttpError)
        {
            string errStr = "WebRequest get error:" + req.error + " url:" + url;
            cb.TouchError(new System.Exception(errStr));
            Debug.LogError(errStr);
        }
        else
        {
            cb.TouchFinish(req);
            req.Dispose();
            req = null;
        }
    }

    IEnumerator DelayError<T>(ProgressCallback<T> cb)
    {
        yield return null;
        
        cb.TouchError(new Exception("NoInternet"));
        
    }
    // --------------------------------------------------------------------------------------------------------------------
    // download
    public void Download(string url, string path, ProgressCallback<UnityWebRequest> cb)
    {
        if (InternetReachable())
        {
            StartCoroutine(WebDL(url, path, cb));
        }
        else
        {
            StartCoroutine(DelayError(cb));
        }
    }

    private IEnumerator WebDL(string url, string path, ProgressCallback<UnityWebRequest> cb)
    {
        UnityWebRequest req = new UnityWebRequest();
        req.method = "GET";
        req.uri = new Uri(url);
        req.downloadHandler = new DownloadHandlerFile(path);

        cb.OnBegin(req);
        req.SendWebRequest();

        while (!req.isDone)
        {
            yield return null;
            cb.TouchProgress(req.downloadProgress);
        }

        if (req.isNetworkError || req.isHttpError)
        {
            string errStr = "WebRequest get error:" + req.error + " url:" + url;
            cb.TouchError(new Exception(errStr));
            Debug.LogError(errStr);
        }
        else
        {
            cb.TouchFinish(req);
            req.Dispose();
            req = null;
        }
    }
    /// <summary>
    /// 返回网络是否打开（只说明设备是否打开了，并不能保证网络是否畅通）
    /// </summary>
    /// <returns></returns>
    public bool InternetReachable()
    {
#if UNITY_EDITOR
        return true;
#else
        return Application.internetReachability != NetworkReachability.NotReachable;
#endif
    }
    public void Initial()
    {
        
    }

    public void Release()
    {
        
    }
}
