using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WaitForHttpProgressCallback<T> : CustomYieldInstruction
{
    ProgressCallback<T> mProgressCb = new ProgressCallback<T>();
    bool mIsFinished;

    public System.Exception Error { get; private set; }
    public UnityWebRequest Request { get; private set; }
    public T Target { get; private set; }
    public float Progress { get; private set; }
    public ProgressCallback<T> ProgressCallback
    {
        get { return mProgressCb; }
    }

    public WaitForHttpProgressCallback()
    {
        Progress = 0;
        Request = null;
        mProgressCb.UserData = null;
        mProgressCb.OnBegin += OnBegin;
        mProgressCb.OnFinish += OnFinish;
        mProgressCb.OnError += OnError;
        mProgressCb.OnProgress += OnProgress;

    }
    void OnBegin(object obj)
    {
        Request = obj as UnityWebRequest;
    }
    void OnFinish(T d, object userData)
    {
        //Request = null;
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