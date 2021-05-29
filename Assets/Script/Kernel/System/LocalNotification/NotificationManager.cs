using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public interface LocalNotificationInterface
{
    void SendNotification(TimeSpan delay, string title, string message);
    void ClearNotification();
}

public class NotificationManager : MonoBehaviour
{
    static NotificationManager msThis;
    public NotificationManager()
    {
        msThis = this;

#if UNITY_ANDROID && !UNITY_EDITOR
            mNotification = new AndroidLocalNotification();
#endif
#if UNITY_IOS && !UNITY_EDITOR
            mNotification = new iOSLocalNotification();
#endif

    }
    static public NotificationManager GetSingleton()
    {
        return msThis;
    }
    //--------------------------------------------------------------------------------------------------------------------
    public delegate void OnGenerateNotificationHandler();
    public event OnGenerateNotificationHandler GenerateNotification;

    LocalNotificationInterface mNotification;
    class LocalNotificationData
    {
        public DateTime time;
        public string title;
        public string message;
    }
    Dictionary<string, LocalNotificationData> mLocalNotificationDataMap = new Dictionary<string,LocalNotificationData>();

    void Start()
    {
        if (mNotification != null)
        {
            //第一次进入游戏的时候清空，有可能用户自己把游戏冲后台杀死，这里强制清空
            mNotification.ClearNotification();
        }

        //DebugManager.GetSingleton().LogDebug(DateTime.UtcNow.ToString());
    }

    void OnApplicationPause(bool paused)
    {
        //程序进入后台时
        if (paused)
        {
            if (GenerateNotification != null)
            {
                GenerateNotification();
            }
            //SetNotification("我就是测试一下", new TimeSpan(0, 1, 0), "测试1分钟", "这是测试1分钟");
            //SetNotification("我就是测试一下2", new TimeSpan(0, 1, 5), "测试1分钟5秒", "这是测试1分钟5秒");

            if (mNotification != null)
            {
                mNotification.ClearNotification();
                ScheduleNotification();
            }
        }
        else
        {
            if (mNotification != null)
            {
                //程序从后台进入前台时
                mNotification.ClearNotification();
                mLocalNotificationDataMap.Clear();
            }
        }
    }
    void ScheduleNotification()
    {
        List<string> delList = new List<string>();
        foreach (KeyValuePair<string, LocalNotificationData> iter in mLocalNotificationDataMap)
        {
            if (iter.Value.time.Ticks > DateTime.Now.Ticks)
            {
                TimeSpan ts = new TimeSpan(iter.Value.time.Ticks - DateTime.Now.Ticks);
                mNotification.SendNotification(ts, iter.Value.title, iter.Value.message);
            }
            else
            {
                delList.Add(iter.Key);
            }
        }
        
        for (int i = 0; i < delList.Count; i++)
        {
            mLocalNotificationDataMap.Remove(delList[i]);
        }
    }
    public void SetNotification(string tag, TimeSpan delay, string title, string message)
    {
        LocalNotificationData data = new LocalNotificationData();
        data.time = DateTime.Now.Add(delay);
        data.title = title;
        data.message = message;
        mLocalNotificationDataMap.Remove(tag);
        mLocalNotificationDataMap.Add(tag, data);
    }
    public void RemoveNotification(string tag)
    {
        //LocalNotificationData data;
        //if (mLocalNotificationDataMap.TryGetValue(tag, out data))
        //{
            mLocalNotificationDataMap.Remove(tag);
        //}
    }
}