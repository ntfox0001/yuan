using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PrefixLogModifer : ILogHandler
{
    string mPrefix;
    ILogHandler mDefaultLogHandler;
    public PrefixLogModifer(string prefix)
    {
        mPrefix = prefix;
        mDefaultLogHandler = Debug.unityLogger.logHandler;
        Debug.unityLogger.logHandler = this;
    }
    public void LogException(Exception exception, UnityEngine.Object context)
    {
        mDefaultLogHandler.LogException(new Exception(mPrefix + exception.Message, exception), context);
    }
    public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
        mDefaultLogHandler.LogFormat(logType, context, mPrefix + format, args);
    }
}
