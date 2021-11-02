using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoutTarget : ISwitchTarget
{
    GameObject mWindow;
    bool mLogoutFinished = false;

    public TargetType Type
    {
        get
        {
            return TargetType.None;
        }
    }

    public LogoutTarget()
    {
        
    }
    public void SetParam(params object[] param)
    {

    }
    public WaitForMultiObjects.WaitReturn Preload()
    {
        mLogoutFinished = false;
        WindowManager.GetSingleton().CreateWindowStack();

        LoadingManager.GetSingleton().StartCoroutine(StartLogout());

        return WaitForMultiObjects.WaitReturn.Continue;
    }
    public WaitForMultiObjects.WaitReturn Postload()
    {
        if (mLogoutFinished)
        {
            mWindow = WindowManager.GetSingleton().ActiveWindowStack.CreateWindow(GameConstValues.OpeningWindowName, "opening", true, true);
            return WaitForMultiObjects.WaitReturn.Continue;
        }
        else
        {
            return WaitForMultiObjects.WaitReturn.Wait;
        }
    }
    public void Release()
    {
        Object.Destroy(mWindow);
    }
    IEnumerator StartLogout()
    {
        yield return AuthManager.GetSingleton().Auth.Logout();
        // sdk退出登录失败，游戏还是要退出
        if (AuthManager.GetSingleton().Auth.IsLogoutSuccessed())
        {
            Debug.Log("logout success.");
        }
        else
        {
            Debug.LogError("logout error.");
        }

        Myself.GetSingleton().Online = false;
        mLogoutFinished = true;
        PlayerPrefs.DeleteKey(AuthManager.LoginType);

    }

}
