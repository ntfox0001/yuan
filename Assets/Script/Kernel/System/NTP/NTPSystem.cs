using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;

public class NTPSystem : Singleton<NTPSystem>, IManagerBase
{
    public float UpdateInteral = 10.0f;
    public static string[] gNtpServers = new string[]
    {
        "time.windows.com",
        "ntp.ntsc.ac.cn",
        "ntp1.aliyun.com"
    };
    NetTime mNetTime;

    public double TimeRate { get; private set; }

    long mPreNetTime = 0;
    long mPreLocalTime = 0;
    /// <summary>
    /// 返回纠正后的deltaTime
    /// </summary>
    public float DeltaTime { get { return (float)TimeRate * Time.deltaTime; } }
    public long TimeStamp
    {
        get
        {
            long t = DateTimeUtil.GetTimeStamp();
            return (long)(TimeRate * (t - mPreLocalTime) + mPreLocalTime);
        }
    }
    public void Initial()
    {
        mNetTime = new NetTime();

        TimeRate = 1.0f;

        //StartCoroutine(UpdateTime());
    }
    IEnumerator UpdateTime()
    {
        var waitTime = mNetTime.GetTime();
        yield return waitTime;
        mPreNetTime = mNetTime.LastTimeStamp;
        mPreLocalTime = DateTimeUtil.GetTimeStamp();

        yield return new WaitForSecondsRealtime(1.0f);
        waitTime = mNetTime.GetTime();
        yield return waitTime;

        long currNetTime = mNetTime.LastTimeStamp;
        long currLocalTime = DateTimeUtil.GetTimeStamp();

        TimeRate = (float)(currNetTime - mPreNetTime) / (currLocalTime - mPreLocalTime);

        mPreNetTime = currNetTime;
        mPreLocalTime = currLocalTime;

        while (true)
        {
            yield return new WaitForSecondsRealtime(10.0f);

            waitTime = mNetTime.GetTime();
            yield return waitTime;

            currNetTime = mNetTime.LastTimeStamp;
            currLocalTime = DateTimeUtil.GetTimeStamp();

            TimeRate = (double)(currNetTime - mPreNetTime) / (double)(currLocalTime - mPreLocalTime);
            //Debug.Log(string.Format("timeRate:{0}, currNetTime:{1}, preNetTime:{2}, currLocalTime:{3}, preLocalTime:{4}", TimeRate, currNetTime, mPreNetTime, currLocalTime, mPreLocalTime));

            mPreNetTime = currNetTime;
            mPreLocalTime = currLocalTime;
        }
    }
    public void Release()
    {
        
    }

}
