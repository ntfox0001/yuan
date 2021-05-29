using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class FileLogHandler : ILog
{
    private FileStream mFileStream;
    private StreamWriter mStreamWriter;
    private ILogHandler mDefaultLogHandler = Debug.unityLogger.logHandler;

    public FileLogHandler()
    {
        string yesterday = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        // 删除前天的数据
        string[] files = Directory.GetFiles(Application.persistentDataPath + "/", "*.log");
        for (int i = 0; i < files.Length; i++)
        {
            string fn = Path.GetFileName(files[i]);
            if (fn == yesterday || fn == today)
            {
                continue;
            }
            File.Delete(files[i]);
        }
        // 打开今天的数据
        string filePath = Application.persistentDataPath + "/" + "." + today + ".log";

        mFileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        mStreamWriter = new StreamWriter(mFileStream);

        // Replace the default debug log handler
        //Debug.unityLogger.logHandler = this;
    }
    public void Release()
    {
        //Debug.unityLogger.logHandler = mDefaultLogHandler;
        mStreamWriter.Close();
        mFileStream.Close();
    }

    public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
        mStreamWriter.WriteLine(string.Format(format, args));
        mStreamWriter.Flush();
    }

    public void LogException(Exception exception, UnityEngine.Object context)
    {
        mStreamWriter.WriteLine(string.Format("{0}\n{1}", exception.Message, exception.StackTrace));
        mStreamWriter.Flush();
    }
}