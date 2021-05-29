using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
public class GameSvrCallback : CustomYieldInstruction
{
    protected Action<LitJson.JsonData, object> mFinishFunc;
    protected Action<float, object> mProgressFunc;
    protected Action<Exception, object> mErrorFunc;
    protected object mUserData;
    protected bool mHasAttach = false;
    public float Progress { get; protected set; }
    public LitJson.JsonData Msg { get; protected set; }
    public string ErrorId { get; protected set; }
    public bool IsDone { get; protected set; }
    public string NetError { get; protected set; }
    public bool Success { get { return (string.IsNullOrEmpty(NetError) && ErrorId == "0") ; } }
    public GameSvrCallback(Action<LitJson.JsonData, object> finishFunc, Action<float, object> progressFunc, Action<Exception, object> errFunc, object userData)
    {
        mFinishFunc = finishFunc;
        mProgressFunc = progressFunc;
        mErrorFunc = errFunc;
    }
    public GameSvrCallback()
    {

    }
    public GameSvrCallback SetFinish(Action<LitJson.JsonData, object> finishFunc)
    {
        mFinishFunc += finishFunc;
        return this;
    }
    public GameSvrCallback SetProgress(Action<float, object> progressFunc)
    {
        mProgressFunc += progressFunc;
        return this;
    }
    public GameSvrCallback SetError(Action<Exception, object> errorFunc)
    {
        mErrorFunc += errorFunc;
        return this;
    }
    public void AttachProgressCallback(ProgressCallback<UnityWebRequest> cb)
    {
        if (mHasAttach)
        {
            // 每个对象不可重复使用
            throw new Exception("GameSvrCallback has attached.");
        }
        mHasAttach = true;
        cb.OnFinish += FinishFunc;
        cb.OnProgress += ProgressFunc;
        cb.OnError += ErrorFunc;
        cb.UserData = mUserData;
    }
    public ProgressCallback<UnityWebRequest> NewProgressCallback()
    {
        var cb = new ProgressCallback<UnityWebRequest>();
        AttachProgressCallback(cb);
        return cb;
    }
    protected virtual void FinishFunc(UnityWebRequest req, object userData)
    {
        IsDone = true;
        LitJson.JsonData msg = LitJson.JsonMapper.ToObject(req.downloadHandler.text);
        Msg = msg;
        if (msg.ContainsKey("errorId"))
        {
            ErrorId = Msg["errorId"].GetString();
        }
        else
        {
            Debug.LogError("NeedErrorId:" + msg.ToJson() + " url: " + req.url);
        }
        

        if (mFinishFunc != null)
        {
            mFinishFunc(msg, userData);
        }
    }
    protected virtual void ProgressFunc(float progress, object userData)
    {
        Progress = progress;
        if (mProgressFunc != null)
        {
            mProgressFunc(progress, userData);
        }
        
    }
    protected virtual void ErrorFunc(Exception e, object userData)
    {
        IsDone = true;
        NetError = e.Message;

        if (mErrorFunc != null)
        {
            mErrorFunc(e, userData);
        }
        
    }

    public override bool keepWaiting
    {
        get
        {
            return !IsDone;
        }
    }
    /// <summary>
    /// 服务器返回OK
    /// </summary>
    /// <returns></returns>
    public bool LogicSuccess
    {
        get
        {
            return string.IsNullOrEmpty(NetError) && ErrorId == "0";
        }
    }
}