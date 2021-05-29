using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class PermissionManager : Singleton<PermissionManager>, IManagerBase
{
#if UNITY_IOS && !UNITY_EDITOR
    // from ThirdPartyPlugins\ARBridge
	[DllImport("__Internal")]
	private static extern void _openAppSetting();
#endif
    public void Initial()
    {

    }

    public void Release()
    {

    }

    public void OpenSetting()
    {
#if !UNITY_EDITOR
#if UNITY_IOS
        _openAppSetting();
#elif UNITY_ANDROID
        AndroidOpenSetting();
#endif
#endif

    }

    void AndroidOpenSetting()
    {
        try
        {
            using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                string packageName = currentActivityObject.Call<string>("getPackageName");

                using (var uriClass = new AndroidJavaClass("android.net.Uri"))
                using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null))
                using (var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.APPLICATION_DETAILS_SETTINGS", uriObject))
                {
                    intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
                    intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
                    currentActivityObject.Call("startActivity", intentObject);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public IEnumerator RequestPermission(UserAuthorization authorization, Action granted, Action denied, Action deniedAndDontAskAgain)
    {
        Debug.Log("PermissionManager.RequestPermission: " + authorization.ToString());
#if UNITY_ANDROID && !UNITY_EDITOR
        return AndroidRequestPermission(authorization, granted, denied, deniedAndDontAskAgain);
#elif UNITY_IOS && !UNITY_EDITOR
        return iOSRequestPermission(authorization, granted, denied;
#else
        return null;
#endif

    }
    IEnumerator AndroidRequestPermission(UserAuthorization authorization, Action granted, Action denied, Action deniedAndDontAskAgain)
    {
        string permissions = "";
        switch (authorization)
        {
            case UserAuthorization.Microphone:
                permissions = "android.permission.RECORD_AUDIO";
                break;
            case UserAuthorization.WebCam:
                permissions = "android.permission.CAMERA";
                break;
            default:
                return null;
        }

        Debug.Log("AndroidRequestPermission: " + permissions);

        bool requestEnd = false;
        AndroidPermissionsManager.RequestPermission(permissions, new AndroidPermissionCallback(grantedPermission =>
            {
                // The permission was successfully granted, restart the change avatar routine
                granted();
                requestEnd = true;
                Debug.Log("AndroidPermissionsManager.RequestPermission, Granted");
            },
            deniedPermission =>
            {
                // The permission was denied
                denied();
                requestEnd = true;
                Debug.Log("AndroidPermissionsManager.RequestPermission, Denied");
            },
            deniedPermissionAndDontAskAgain =>
            {
                // The permission was denied, and the user has selected "Don't ask again"
                // Show in-game pop-up message stating that the user can change permissions in Android Application Settings
                // if he changes his mind (also required by Google Featuring program)
                deniedAndDontAskAgain();
                requestEnd = true;
                Debug.Log("AndroidPermissionsManager.RequestPermission, deniedAndDontAskAgain");
            }));

        return new WaitUntil(() =>
        {
            return requestEnd;
        });
    }

    IEnumerator iOSRequestPermission(UserAuthorization authorization, Action grantedPermission, Action deniedPermission)
    {
        var ao = Application.RequestUserAuthorization(authorization);
        return new WaitUntil(() =>
        {
            if (Application.HasUserAuthorization(authorization))
            {
                grantedPermission();
            }
            else
            {
                deniedPermission();
            }

            return ao.isDone;
        });
    }
}
