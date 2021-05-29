using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


#if UNITY_ANDROID && !UNITY_EDITOR
public class AndroidLocalNotification : LocalNotificationInterface
{
    /// <summary>
    /// Inexact uses `set` method
    /// Exact uses `setExact` method
    /// ExactAndAllowWhileIdle uses `setAndAllowWhileIdle` method
    /// Documentation: https://developer.android.com/intl/ru/reference/android/app/AlarmManager.html
    /// </summary>
    public enum NotificationExecuteMode
    {
        Inexact = 0,
        Exact = 1,
        ExactAndAllowWhileIdle = 2
    }
    int id = 0;

    private string fullClassName = "net.agasper.unitynotification.UnityNotificationManager";
    private string mainActivityClassName = "com.unity3d.player.UnityPlayerNativeActivity";


    public void SendNotification(TimeSpan delay, string title, string message)
    {
        id++;
        SendNotification(id, (int)delay.TotalSeconds, title, message, Color.white, true, true, true, "icon", "icon");
    }
    
    void SendNotification(int id, long delay, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "", string smallIcon = "", NotificationExecuteMode executeMode = NotificationExecuteMode.Inexact)
    {
        AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        if (pluginClass != null)
        {
            pluginClass.CallStatic("SetNotification", id, delay * 1000L, title, message, message, sound ? 1 : 0, vibrate ? 1 : 0, lights ? 1 : 0, bigIcon, smallIcon, bgColor.r * 65536 + bgColor.g * 256 + bgColor.b, (int)executeMode, mainActivityClassName);
        }
    }

    void SendRepeatingNotification(int id, long delay, long timeout, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "", string smallIcon = "")
    {
        AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        if (pluginClass != null)
        {
            pluginClass.CallStatic("SetRepeatingNotification", id, delay * 1000L, title, message, message, timeout * 1000, sound ? 1 : 0, vibrate ? 1 : 0, lights ? 1 : 0, bigIcon, smallIcon, bgColor.r * 65536 + bgColor.g * 256 + bgColor.b, mainActivityClassName);
        }
    }

    public void CancelNotification(int id)
    {
        AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        if (pluginClass != null) {
            pluginClass.CallStatic("CancelNotification", id);
        }
    }

    public void ClearNotification()
    {
        for (int i = 1; i <= id; i++)
        {
            CancelNotification(i);
        }
        id = 0;
        //AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        //if (pluginClass != null)
            //pluginClass.CallStatic("CancelAll");
    }
}
#endif

#if UNITY_IOS && !UNITY_EDITOR

using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;
using LocalNotification = UnityEngine.iOS.LocalNotification;
public class iOSLocalNotification : LocalNotificationInterface
{
    int mNumber = 1;
    public iOSLocalNotification()
    {
        NotificationServices.RegisterForNotifications(NotificationType.Alert | NotificationType.Badge | NotificationType.Sound);
    }
    public void SendNotification(TimeSpan delay, string title, string message)
    {
        LocalNotification localNotification = new LocalNotification();
        localNotification.fireDate = DateTime.Now.Add(delay);
        localNotification.alertBody = message;
        localNotification.applicationIconBadgeNumber = 1;
        localNotification.hasAction = true;
        mNumber++;
        //if (isRepeatDay)
        //{
        //    //是否每天定期循环
        //    localNotification.repeatCalendar = CalendarIdentifier.ChineseCalendar;
        //    localNotification.repeatInterval = CalendarUnit.Day;
        //}
        localNotification.soundName = LocalNotification.defaultSoundName;
        NotificationServices.ScheduleLocalNotification(localNotification);

        //DebugManager.GetSingleton().LogDebug("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-= number:" + mNumber);
    }
    public void ClearNotification()
    {
        //DebugManager.GetSingleton().LogDebug("Notify count: " + NotificationServices.localNotificationCount);
        for (int i = NotificationServices.localNotificationCount - 1; i >= 0 ; i--)
        {
            LocalNotification ln = NotificationServices.localNotifications[i];
            NotificationServices.CancelLocalNotification(ln);
            //DebugManager.GetSingleton().LogDebug("notify text: " + ln.alertBody);
        }
        LocalNotification l = new LocalNotification ();
        l.applicationIconBadgeNumber = -1;
        NotificationServices.PresentLocalNotificationNow (l);

        NotificationServices.CancelAllLocalNotifications();
        NotificationServices.ClearLocalNotifications();
        mNumber = 1;
    }
}

#endif
