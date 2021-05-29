using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;


/// <summary>
/// 处理单个权限类型的许可问题
/// </summary>
public class PermissionAuthorizationManager:Singleton<PermissionAuthorizationManager>,IManagerBase
{
    public enum Permission
    {
        Denied = 0,
        Granted,
        ShouldAsk,
    }
#if UNITY_IOS && !UNITY_EDITOR
    // from ThirdPartyPlugins\ARBridge
	[DllImport("__Internal")]
	private static extern void _openAppSetting();
#endif
    public bool HasMicrophone
    {
        get
        {
//#if UNITY_IOS
            return Application.HasUserAuthorization(UserAuthorization.Microphone);
//#elif UNITY_ANDROID
//            return AndroidRuntimePermissions.CheckPermission("android.permission.RECORD_AUDIO") 
//                   == AndroidRuntimePermissions.Permission.Granted;
//#endif
        }
    }

    public void Initial()
    {
    }

    public void Release()
    {
    }

    

    public AsyncOperation RequestPermission(string key)
    {
        var type = (UserAuthorization)Enum.Parse(typeof(UserAuthorization),key);
        switch (type)
        {
            case UserAuthorization.Microphone:

                if (!HasMicrophone)
                {
//#if UNITY_IOS
                    return Application.RequestUserAuthorization(UserAuthorization.Microphone);
//#elif UNITY_ANDROID
//                    AndroidRuntimePermissions.RequestPermission("android.permission.RECORD_AUDIO");
//#endif
                }
                break;
            case UserAuthorization.WebCam:
            default:
                break;
        }
        return null;
    }

    public void OpenSetting()
    {
#if !UNITY_EDITOR
#if UNITY_IOS
        _openAppSetting();
#elif UNITY_ANDROID
        //AndroidRuntimePermissions.OpenSettings();
#endif
#endif
    }

    public bool IsContainsPermission(string key)
    {
        foreach (int v in Enum.GetValues(typeof(UserAuthorization)))
        {
            string strName = Enum.GetName(typeof(UserAuthorization), v);
            if (key.Contains(strName))
            {
                return true;
            }
        }
        return false;
    }
}
