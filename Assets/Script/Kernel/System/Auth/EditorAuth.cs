using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR && !DREAM_OFFLINE
public class AuthHelper
{
    public static void InitialComponent(GameObject parent)
    {
    }
    public static IAuth GetAuth()
    {
        return new EditorAuth();
    }
}
public class EditorAuth : AuthBase
{
    protected static float[] sPayRetryTimes = new float[] { 1.0f, 1.0f, 2.0f, 5.0f, 10.0f, 10.0f, 15.0f };
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
        return SystemInfo.deviceUniqueIdentifier;
    }
    public override string GetKey()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }
    public override string GetDeviceId()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }
    public override string GetBindInfo()
    {
        return mLoginType;
    }
    public override CustomYieldInstruction InitialPay(System.Action<string> successFunc, System.Action<AuthError> failFunc)
    {
        var setting = TableManager.GetSingleton().GetSheet("Table", "Pay");
        LitJson.JsonData productList = new LitJson.JsonData();
        foreach (var i in setting.Keys)
        {
            productList.Add(setting[i]["ProductId"].GetString());

            PayProductInfo info = new PayProductInfo();
            info.Price = long.Parse(setting[i]["Price"].GetString());
            info.FormattedPrice = "RMB￥" + info.Price;
            info.PayId = setting[i][AuthManager.PayKey].GetString();
            info.PriceCurrencyCode = "RMB";
            mProductList.Add(info);
        }
        string js = productList.ToJson();
        Debug.Log(js);

        AuthManager.GetSingleton().StartCoroutine(DelayPayInit());
        return base.InitialPay(successFunc, failFunc);
    }
    IEnumerator DelayPayInit()
    {
        yield return new WaitForSeconds(DelayTime);
        mPayInitial.SetFinish(true);
    }
    public override void Buy(string productId, System.Action<string> successFunc, System.Action<AuthError> failFunc)
    {
        string payId = AuthManager.GetSingleton().GetPayId(productId);
        AuthManager.GetSingleton().StartCoroutine(DelayPay(payId, successFunc, failFunc));

    }
    IEnumerator DelayPay(string payId, System.Action<string> successFunc, System.Action<AuthError> failFunc)
    {
        yield return new WaitForSeconds(DelayTime);
        string productId = AuthManager.GetSingleton().GetProductId(payId);
        mPaySuccessFunc(productId);

        if (successFunc != null)
        {
            successFunc(productId);
        }
    }
    public override void CheckRestore(Action<List<string>> successFunc, Action<AuthError> failFunc, string throughgo)
    {
        AuthManager.GetSingleton().StartCoroutine(_CheckRestoreFree(successFunc, failFunc));
    }
    IEnumerator _CheckRestoreFree(Action<List<string>> successFunc, Action<AuthError> failFunc)
    {
        yield return new WaitForSeconds(2.0f);
        successFunc(new List<string>());
    }

    IEnumerator _CheckRestore(Action<List<string>> successFunc, Action<AuthError> failFunc)
    {
        // 查询服务器是否有未完成订单，然后确认订单完成它
        // 所有非订阅和包月制永远都是未完成状态
        int i = 0;
        List<string> orderIds = new List<string>();
        while (true)
        {
            Debug.Log("QueryPay");
            var qp = GameServer.GetSingleton().QueryPay();
            yield return qp;

            if (qp.LogicSuccess)
            {
                Debug.Log("QueryPay success: " + qp.Msg.ToJson());
                var purchasesJd = qp.Msg["purchases"];

                for (int k = 0; k < purchasesJd.Count; k++)
                {
                    string orderId = purchasesJd[k]["orderId"].GetString();
                    orderIds.Add(orderId);
                }
                break;
            }

            i = i >= sPayRetryTimes.Length ? sPayRetryTimes.Length - 1 : i;
            Debug.LogError("QueryPay Error: " + qp.NetError + " ErrorId: " + qp.ErrorId);
            yield return new WaitForSeconds(sPayRetryTimes[i]);
            i++;
        }

        List<string> prodIds = new List<string>();

        // 如果没有订单，那么直接返回
        if (orderIds.Count > 0)
        {
            i = 0;
            // 通知服务器这些支付
            while (true)
            {
                Debug.Log("ConfirmPay");
                var cp = GameServer.GetSingleton().ConfirmPay(orderIds.ToArray());
                yield return cp;

                if (cp.LogicSuccess)
                {
                    Debug.Log("ConfirmPay: " + cp.Msg.ToJson());
                    var prodIdsJd = cp.Msg["prodIds"];
                    for (int k = 0; k < prodIdsJd.Count; k++)
                    {
                        string prodId = prodIdsJd[k].GetString();
                        mPaySuccessFunc(prodId);
                        prodIds.Add(prodId);
                    }

                    break;
                }

                i = i >= sPayRetryTimes.Length ? sPayRetryTimes.Length - 1 : i;
                Debug.LogError("QueryPay Error: " + cp.NetError + " ErrorId: " + cp.ErrorId);
                yield return new WaitForSeconds(sPayRetryTimes[i]);
                i++;

            }
        }

        if (successFunc != null) successFunc(prodIds);
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
        mLogout.SetFinish(true);
    }
}

#endif