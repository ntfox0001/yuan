using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuthManager : Singleton<AuthManager>, IManagerBase
{
#if UNITY_ANDROID
    public const string PayKey = "GooglePayId";
#elif UNITY_IOS
    public const string PayKey = "ApplePayId";
#else
#error "PayError"
#endif

    public const string LoginType = "__LoginType__";
    public const string WeiXinLogin = "Wechat";
    public const string AppleLogin = "Apple";
    public const string FacebookLogin = "Facebook";
    public const string GoogleLogin = "Google";
    public const string GuestLogin = "Guest";
    public const string EMailLogin = "EMail";
    public const string NoneLogin = "None";
    public const string UnknownLogin = "Unknown";

    public const long Month = 31L * 24L * 60L * 60L * 1000L;
    public IAuth Auth { get; private set; }
    public void Initial()
    {
        //if (HttpManager.GetSingleton().InternetReachable())
        //{
        //    SetOfflineAuth();
        //}
        //else
        //{
        Auth = AuthHelper.GetAuth();
        //}
    }
    public void SetOfflineAuth()
    {
        Auth = new OfflineAuth();
    }
    public void Release()
    {
        
    }
    private void Update()
    {
        if (Auth != null)
        {
            Auth.Update();
        }
    }

    public string GetProductId(string payId)
    {
        var setting = TableManager.GetSingleton().GetSheet("Table", "Pay");
        foreach (var i in setting.Keys)
        {
            if (setting[i][PayKey].GetString() == payId)
            {
                return setting[i]["ProductId"].GetString();
            }
        }

        return "";
    }
    public LitJson.JsonData GetProductJsonData(string productId)
    {
        var setting = TableManager.GetSingleton().GetSheet("Table", "Pay");
        foreach (var i in setting.Keys)
        {
            if (setting[i]["ProductId"].GetString() == productId)
            {
                return setting[i];
            }
        }

        return null;
    }
    public LitJson.JsonData GetProduct(string payId)
    {
        var setting = TableManager.GetSingleton().GetSheet("Table", "Pay");
        foreach (var i in setting.Keys)
        {
            if (setting[i][PayKey].GetString() == payId)
            {
                return setting[i];
            }
        }

        return null;
    }

    public string GetPayId(string productId)
    {
        var setting = TableManager.GetSingleton().GetSheet("Table", "Pay");
        foreach (var i in setting.Keys)
        {
            if (setting[i]["ProductId"].GetString() == productId)
            {
                return setting[i][PayKey].GetString();
            }
        }

        return "";
    }
    public uint GetPrice(string productId)
    {
        var setting = TableManager.GetSingleton().GetSheet("Table", "Pay");
        foreach (var i in setting.Keys)
        {
            if (setting[i]["ProductId"].GetString() == productId)
            {
                return uint.Parse(setting[i]["Price"].GetString());
            }
        }

        return 0;
    }
}
