using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 用来接收HttpManager的回调，可以在协程中使用
/// 取消泛型定义，能少写就少写
/// </summary>
public class WaitForHttp : CustomYieldInstruction
{
    ProgressCallback<UnityWebRequest> mProgressCb = new ProgressCallback<UnityWebRequest>();
    bool mIsFinished;
    UnityWebRequest mRequest;

    public byte[] Data { get; private set; }
    public string Text { get; private set; }
    public System.Exception Error { get; private set; }
    
    public UnityWebRequest Target { get; private set; }
    public float Progress { get; private set; }
    public ProgressCallback<UnityWebRequest> ProgressCallback
    {
        get { return mProgressCb; }
    }
    public object UserData
    {
        get { return mProgressCb.UserData; }
        set { mProgressCb.UserData = value; }
    }
    public WaitForHttp()
    {
        Progress = 0;
        mRequest = null;
        Data = null;
        Text = null;
        mProgressCb.UserData = null;
        mProgressCb.OnBegin += OnBegin;
        mProgressCb.OnFinish += OnFinish;
        mProgressCb.OnError += OnError;
        mProgressCb.OnProgress += OnProgress;
    }
    
    void OnBegin(object obj)
    {
        mRequest = obj as UnityWebRequest;
    }
    void OnFinish(UnityWebRequest d, object userData)
    {
        Data = mRequest.downloadHandler.data;
        Text = mRequest.downloadHandler.text;

        Target = d;
        mIsFinished = true;
    }
    void OnError(System.Exception e, object userData)
    {
        Debug.LogError("Failed to WaitForHttpProgressCallback:" + e.Message);
        Error = e;
        mIsFinished = true;
    }
    void OnProgress(float p, object userData)
    {
        Progress = p;
    }
    public override bool keepWaiting
    {
        get
        {
            return !mIsFinished;
        }
    }
}
