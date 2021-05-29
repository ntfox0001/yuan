using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetLogHandler : ILog
{
    public int MaxLog = 100;
    public int Timeout = 4;
    //private ILogHandler mDefaultLogHandler = Debug.unityLogger.logHandler;
    LitJson.JsonData mLogCache = new LitJson.JsonData();
    string mNetLogAddress;
    bool mLogging = true;
    bool mOnLogProc = false;
    LitJson.JsonData mLogData = null;
    public NetLogHandler()
    {
        //Debug.unityLogger.logHandler = this;
        mLogCache.SetJsonType(LitJson.JsonType.Array);
        LogManager.GetSingleton().StartCoroutine(UpdateLog());

        if (mNetLogAddress != "")
        {
            mNetLogAddress = LogManager.GetSingleton().NetLogAddress;
        }
    }
    IEnumerator UpdateLog()
    {
        int errLogCount = 0;

        while (mLogging)
        {
            yield return new WaitForSecondsRealtime(LogManager.GetSingleton().NetLogRefreshTime);
            mOnLogProc = true;
            if (AuthManager.GetSingleton().Auth.IsLoginSuccessed())
            {
                if (mLogData == null)
                {
                    mLogData = new LitJson.JsonData();
                    mLogData["id"] = AuthManager.GetSingleton().Auth.GetId();
                    mLogData["log"] = mLogCache;
                }
                if (mLogCache.Count > 0)
                {
                    string js = mLogData.ToJson(true);

                    ProgressCallback<UnityWebRequest> cb = new ProgressCallback<UnityWebRequest>();
                    cb.OnError = (Exception e, object userData) =>
                    {
                        errLogCount++;


                    };
                    cb.OnFinish = (UnityWebRequest req, object userData) =>
                    {

                        errLogCount = 0;
                    };
                    HttpManager.GetSingleton().HttpPost(mNetLogAddress,
                        HttpManager.ContentType_TextPlain, System.Text.Encoding.UTF8.GetBytes(js), null, cb, Timeout);


                    // 立刻清除
                    mLogCache.Clear();

                    if (errLogCount != 0)
                    {
                        float t = errLogCount > 10 ? 10 : errLogCount;
                        yield return new WaitForSeconds(t);

                    }
                }
            }
            else
            {
                if (MaxLog <= mLogCache.Count)
                {
                    mLogCache.Clear();
                }
            }
            mOnLogProc = false;
        }
        Debug.Log("++++++++++++++++++++++++++++++++++++++++++++++++++++");
    }
    public void LogException(Exception exception, UnityEngine.Object context)
    {
        if (mLogging && !mOnLogProc)
        {
            mLogCache.Add(string.Format("{0}\n{1}", exception.Message, exception.StackTrace));
        }

        //mDefaultLogHandler.LogException(exception, context);
    }

    public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
        if (mLogging && !mOnLogProc)
        {
            mLogCache.Add(string.Format(format, args));
        }

        //mDefaultLogHandler.LogFormat(logType, context, format, args);
    }
    public void Release()
    {
        //Debug.unityLogger.logHandler = mDefaultLogHandler;
    }
}
