using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if DREAM_OFFLINE
public class AuthHelper
{
    public static void InitialComponent(GameObject parent)
    {
    }
    public static IAuth GetAuth()
    {
        return new OfflineAuth();
    }
}
#endif
public class OfflineAuth : AuthBase
{
    string mLoginType = AuthManager.NoneLogin;
    const float DelayTime = 0.1f;
    public override CustomYieldInstruction Initial()
    {
        AuthManager.GetSingleton().StartCoroutine(DelayInitial());
        return base.Initial();
    }
    IEnumerator DelayInitial()
    {
        yield return new WaitForSeconds(DelayTime);
        //mIsInitialFinished = true;
        //mIsInitialSuccessed = true;
        mInitial.SetFinish(true);
    }
    public override CustomYieldInstruction Login()
    {
        AuthManager.GetSingleton().StartCoroutine(DelayLogin());
        return base.Login();
    }
    IEnumerator DelayLogin()
    {
        yield return new WaitForSeconds(DelayTime);
        //mIsLoginFinished = true;
        //mIsLoginSuccessed = true;
        mLogin.SetFinish(true);
    }
    public override void SetLoginType(string type)
    {
        mLoginType = type;
    }
    public override string GetLoginType()
    {
        return mLoginType;
    }
    public override string GetId()
    {
        //return SystemInfo.deviceUniqueIdentifier;
        return "fdsakjkgjdksajgfkdlsajgfkldsjalkgfdsag";
    }
    public override string GetKey()
    {
        //return SystemInfo.deviceUniqueIdentifier;
        return "dsajgkldsjagklrejakgljeralkgresahreajetr";
    }
    public override string GetDeviceId()
    {
        //return SystemInfo.deviceUniqueIdentifier;
        return "hko4phk4ophjit4rowhsjt48r9bwpj84rt0pjg9845rwpgj";
    }
    public override string GetBindInfo()
    {
        return AuthManager.GuestLogin;
    }
    public override CustomYieldInstruction InitialPay(System.Action<string> successFunc, System.Action<AuthError> failFunc)
    {
        var setting = TableManager.GetSingleton().GetSheet("Table", "Pay");
        LitJson.JsonData productList = new LitJson.JsonData();
        foreach (var i in setting.Keys)
        {
            productList.Add(setting[i]["ProductId"].GetString());
        }
        string js = productList.ToJson();
        Debug.Log(js);

        AuthManager.GetSingleton().StartCoroutine(DelayPayInit());
        return base.InitialPay(successFunc, failFunc);
    }
    IEnumerator DelayPayInit()
    {
        yield return new WaitForSeconds(DelayTime);
        //mIsPayInitFinished = true;
        //mIsPayInitSuccessed = true;
        mPayInitial.SetFinish(true);
    }
    public override void Buy(string productId, System.Action<string> successFunc, System.Action<AuthError> failFunc)
    {
        string payId = AuthManager.GetSingleton().GetPayId(productId);
        AuthManager.GetSingleton().StartCoroutine(DelayPay(payId, successFunc, failFunc));

    }
    IEnumerator DelayPay(string productId, System.Action<string> successFunc, System.Action<AuthError> failFunc)
    {
        yield return new WaitForSeconds(DelayTime);

        mPaySuccessFunc(productId);

        if (successFunc != null)
        {
            successFunc(productId);
        }
    }
    public override void CheckRestore(Action<List<string>> successFunc, Action<AuthError> failFunc, string throughgo)
    {
        AuthManager.GetSingleton().StartCoroutine(DelayCheckStore(successFunc));
    }
    IEnumerator DelayCheckStore(Action<List<string>> successFunc)
    {
        yield return new WaitForSeconds(2.0f);
        successFunc(new List<string>());
        mPaySuccessFunc("fullstamina10");
    }
    public override void SharePic(string picPath, SocialShareType type, Action successFunc, Action<AuthError> failFunc)
    {
        successFunc();
    }
    public override CustomYieldInstruction BindAccount(string type)
    {
        AuthManager.GetSingleton().StartCoroutine(DelayBindAccount());
        return base.BindAccount(type);
    }
    IEnumerator DelayBindAccount()
    {
        yield return null;
        //mIsBindAccountFinished = true;
        //mIsBindAccountSuccessed = true;
        mBindAccount.SetFinish(true);
    }
    public override CustomYieldInstruction Logout()
    {
        mLoginType = AuthManager.NoneLogin;
        AuthManager.GetSingleton().StartCoroutine(DelayLogout());
        return base.Logout();
    }
    IEnumerator DelayLogout()
    {
        yield return null;
        //mIsLogoutFinished = true;
        //mIsLogoutSuccessed = true;
        mLogout.SetFinish(true);
    }
}