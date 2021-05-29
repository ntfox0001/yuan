using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

public class ProgressCallback<T>
{
    public object UserData;
    public Action<object> OnBegin;
    public Action<float, object> OnProgress;
    public Action<T, object> OnFinish;
    public Action<Exception, object> OnError;

    /// <summary>
    /// 用来启动的时候传递特殊对象
    /// </summary>
    /// <param name="obj"></param>
    public void TouchBegin(object obj)
    {
        if (OnBegin != null)
            OnBegin(obj);
    }
    public void TouchProgress(float p)
    {
        if (OnProgress != null)
            OnProgress(p, UserData);
    }

    public void TouchFinish(T d)
    {
        if (OnFinish != null)
            OnFinish(d, UserData);
    }
    public void TouchError(Exception e)
    {
        if (OnError != null)
            OnError(e, UserData);
    }
}

/// <summary>
/// 提供一个协程到progress的对应回调
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class WaitForProgressCallback<T> : CustomYieldInstruction
{
    ProgressCallback<T> mProgressCb = new ProgressCallback<T>();
    float mProgress = 0;
    T mTarget;
    bool mIsFinished = false;
    public object UserData { get { return mProgressCb.UserData; } }
    public Exception Error { get; private set; }
    public T Target { get { return mTarget; } }
    public float Progress { get { return mProgress; } }
    public ProgressCallback<T> ProgressCallback
    {
        get { return mProgressCb; }
    }

    public WaitForProgressCallback(object userData = null)
    {
        mProgressCb.UserData = userData;
        mProgressCb.OnFinish += OnFinish;
        mProgressCb.OnError += OnError;
        mProgressCb.OnProgress += OnProgress;

    }
    void OnFinish(T d, object userData)
    {
        mTarget = d;
        mIsFinished = true;
    }
    void OnError(Exception e, object userData)
    {
        Debug.LogError("Failed to WaitForProgressCallback:" + e.Message);
        Error = e;
        mIsFinished = true;
    }
    void OnProgress(float p, object userData)
    {
        mProgress = p;
    }
    public override bool keepWaiting
    {
        get
        {
            return !mIsFinished;
        }
    }
}