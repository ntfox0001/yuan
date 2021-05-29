using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
public interface ILog : ILogHandler
{
    void Release();
}
public class LogManager :Singleton<LogManager>, IManagerBase
{
    public bool UseNetLog = false;
    public bool UseFileLog = false;

    public string NetLogAddress = "";
    public float NetLogRefreshTime = 0.5f;
    // 文件日志
    private ILog mNetLogHandler = null;
    private ILog mFileLogHandler = null;
    private ILogHandler mLogPrefix;

    public void Initial()
    {
        // 强制ios下不使用文件log（访问不到）
#if (UNITY_IOS) && !UNITY_EDITOR
        UseFileLog = false;
#endif
        mLogPrefix = new PrefixLogModifer("<DreamDetective> ");
        if (UseFileLog)
        {
            mFileLogHandler = new FileLogHandler();
        }
        if (UseNetLog)
        {
            mNetLogHandler = new NetLogHandler();
        }

        Application.logMessageReceived += HandleLog;
    }

    private void HandleLog(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {
            if (mFileLogHandler != null)
            {
                mFileLogHandler.LogFormat(type, null, "<{0}>:{1}\n{2}", type.ToString(), condition, stackTrace);
            }
            if (mNetLogHandler != null)
            {
                mNetLogHandler.LogFormat(type, null, "<{0}>:{1}\n{2}", type.ToString(), condition, stackTrace);
            }

        }
        else
        {
            if (mFileLogHandler != null)
            {
                mFileLogHandler.LogFormat(type, null, "<{0}>:{1}", type.ToString(), condition);
            }
            if (mNetLogHandler != null)
            {
                mNetLogHandler.LogFormat(type, null, "<{0}>:{1}", type.ToString(), condition);
            }

        }
    }


    public void Release()
    {
        Application.logMessageReceived -= HandleLog;
        if (mFileLogHandler != null)
        {
            mFileLogHandler.Release();
        }
        if (mNetLogHandler != null)
        {
            mNetLogHandler.Release();
        }
    }

}
