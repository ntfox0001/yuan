using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class GameServer : Singleton<GameServer>, IManagerBase
{
    [Tooltip("Alway false in mobile.")]
    public bool Offline;
    public bool Compression;
    public string Url;
    public string GlobalUrl;
    public string Router_Login;
    public string Router_UpdatePlayerData;
    public string Router_LoadPlayerData;
    public string Router_LoadRank;
    public string Router_GetRankRefreshInfo;
    public string Router_GetWeekRankAward;
    public string Router_GetSvrTime;
    public string Router_Mail;
    public string Router_Like;
    public string Router_Pay;
    public int Timeout = 4;

    public long SvrTime { get; private set; }
    /// <summary>
    /// 表示最后一次更新服务器时间的时间
    /// </summary>
    public long SvrTimeLastUpdate { get; private set; }
    /// <summary>
    /// 本地时间，减去这个值，就是矫正后的时间（服务器时间），这个时间可以矫正用户手动调整本地时间，但是不能检测到时钟齿轮
    /// </summary>
    public long LocalTimeDiff { get; private set; }

    bool mHasRefreshConfigValue = false;
    string mUrl = "";
    Dictionary<string, string> mCookie = new Dictionary<string, string>();
    string InternalUrl
    {
        get
        {
            if (mUrl == "")
            {
                string url = GlobalUrl;

                mUrl = url;

                Debug.Log("GameServer url: " + mUrl);
            }
            return mUrl;
        }
    }

    public bool IsLogin { get; private set; }
    public void Initial()
    {

    }

    public void Release()
    {

    }
    /// <summary>
    /// 刷新Config状态，只执行一次
    /// </summary>
    void RefreshConfigValue()
    {
        if (!mHasRefreshConfigValue && ConfigManager.GetSingleton().LoadSuccess)
        {
            mHasRefreshConfigValue = true;

            Compression = true;
            Debug.Log("(Config)Compression is " + Compression);

        }
    }
    public LitJson.JsonData MakeMsg(string msgId)
    {
        var jd = new LitJson.JsonData();
        jd["msgId"] = msgId;
        return jd;
    }
    byte[] ToByte(LitJson.JsonData jd)
    {
        string js = jd.ToJson();
        if (Compression)
        {
            byte[] buf;
            CompressManager.CompressString(js, out buf, CompressManager.Compresszlib);
            float cp = ((float)buf.Length / js.Length) * 100.0f;
            Debug.Log("compress:" + cp.ToString("F2") + "% res: " + js.Length + " compress: " + buf.Length);
            return buf;
        }
        else
        {
            byte[] buf = System.Text.Encoding.UTF8.GetBytes(js);
            Debug.Log("msg len: " + buf.Length);
            return buf;
        }

    }
    public void Logout()
    {
        IsLogin = false;
        mCookie.Remove("Cookie");
    }

    public GameSvrCallback Login()
    {
        RefreshConfigValue();
        var cb = NewGameSvrCallback();
        var msg = MakeMsg("LoginReq");
        msg["sessionKey"] = AuthManager.GetSingleton().Auth.GetKey();

        var logincb = new ProgressCallback<UnityWebRequest>();
        logincb.OnFinish = (UnityWebRequest req, object userData) =>
        {
            string cookie = req.GetResponseHeader("Set-Cookie");
            if (!string.IsNullOrEmpty(cookie))
            {
                IsLogin = true;
                mCookie["Cookie"] = cookie;
            }
            else
            {
                Debug.LogError("Failed to login." + req.downloadHandler.text);
                IsLogin = false;
                mCookie.Clear();
            }

        };
        logincb.OnError = (Exception e, object userData) =>
        {
            IsLogin = false;
        };
        cb.AttachProgressCallback(logincb);
        HttpManager.GetSingleton().HttpPost(GetUrl(InternalUrl + Router_Login), HttpManager.ContentType_UrlEncoded, ToByte(msg), null, logincb, Timeout);
        return cb;
    }
    string GetUrl(string url)
    {
        if (Compression)
        {
            url += "2";
        }
        return url;
    }
    GameSvrCallback NewGameSvrCallback()
    {
        GameSvrCallback cb = null;
        if (!Compression)
        {
            cb = new GameSvrCallback();
        }
        else
        {
            cb = new GameSvrCompressionCallback();
        }

        return cb;
    }
    void OnCheckErrorId(LitJson.JsonData msg, object userData)
    {
        string errorId = msg["errorId"].GetString();

        switch (errorId)
        {
            case "NoLogin": // 客户端还没登陆
            case "InvalidSessionId": // 当前服务器没有这个sessionid，那么重新登陆
                {
                    IsLogin = false;
                    // 自动重新登陆
                    Login();
                    break;
                }
            case "BeKick":  // 被另一个客户端踢下线
                {
                    //var req = userData as UnityWebRequest;
                    //string cookie = req.GetResponseHeader("Set-Cookie");
                    //if (cookie != mCookie["Cookie"])
                    //{
                    // cookie不等，说明已经重新登陆，这个消息有可能是上一次登陆时发送的
                    //    return;
                    //}
                    //Logout();
                    //您的账号已在另一台设备上登陆
                    //MsgBox.Show(1139, () => {
                    //    //GameProcedureHelper.Get().Logout();
                    //    LoadingManager.GetSingleton().SwitchToLogout();
                    //});

                    break;
                }
            case "0":
                {
                    // success
                    Debug.Log(msg["msgId"].GetString() + " successed.");

                    SvrTime = msg["svrTime"].GetLong();
                    SvrTimeLastUpdate = DateTimeUtil.GetTimeStamp();
                    LocalTimeDiff = SvrTimeLastUpdate - SvrTime;

                    // 消息正确，直接退出------------------------------------return
                    return;
                }
            default:
                {
                    Debug.LogError("Msg " + msg["msgId"].GetString() + " ErrorId:" + errorId);
                    break;
                }
        }

        // error id == 0时，直接return，所以只有错误才能走到这里
        LitJson.JsonData svrErr = new LitJson.JsonData();
        svrErr["err"] = errorId;
        AuthManager.GetSingleton().Auth.TraceEvent("ServerErr", svrErr.ToJson());
    }
    void OnNetError(System.Exception e)
    {
        LitJson.JsonData netErr = new LitJson.JsonData();
        netErr["err"] = e.Message;
        AuthManager.GetSingleton().Auth.TraceEvent("NetErr", netErr.ToJson());
    }
    public long GetServerTime()
    {
        if (SvrTime == 0) return DateTimeUtil.GetTimeStamp();
        if (DateTimeUtil.GetTimeStamp() - SvrTimeLastUpdate > 60 * 10)
        {
            return DateTimeUtil.GetTimeStamp();
        }
        return DateTimeUtil.GetTimeStamp() - LocalTimeDiff;
    }
    public GameSvrCallback LoadPlayerData()
    {
        Debug.Log("LoadPlayerData is sent.");
        var msg = MakeMsg("LoadPlayerDataReq");

        return SendMsg(Router_LoadPlayerData, msg);
    }

    public GameSvrCallback UpdatePlayerData(LitJson.JsonData dataRoot)
    {
        Debug.Log("UpdatePlayerData is sent.");

        var msg = MakeMsg("UpdatePlayerDataReq");
        msg["dataRoot"] = dataRoot;

        return SendMsg(Router_UpdatePlayerData, msg);
    }

    public GameSvrCallback LoadRank(string type)
    {
        var msg = MakeMsg("LoadRankReq");
        msg["rankType"] = type;

        return SendMsg(Router_LoadRank, msg);
    }
    public GameSvrCallback GetRankRefreshInfo()
    {
        var msg = MakeMsg("GetRankRefreshInfoReq");
        return SendMsg(Router_GetRankRefreshInfo, msg);
    }
    public GameSvrCallback GetWeekRankAward(string type)
    {
        var msg = MakeMsg("GetWeekRankAwardReq");
        msg["rankType"] = type;

        return SendMsg(Router_GetWeekRankAward, msg);
    }
    public GameSvrCallback GetSvrTime()
    {
        var msg = MakeMsg("GetSvrTimeReq");
        return SendMsg(Router_GetSvrTime, msg);
    }
    public GameSvrCallback GetMailState()
    {
        var msg = MakeMsg("GetMailStateReq");
        return SendMsg(Router_Mail, msg);
    }
    public GameSvrCallback GetMailInfo()
    {
        var msg = MakeMsg("GetMailInfoReq");
        return SendMsg(Router_Mail, msg);
    }
    public GameSvrCallback ReceiveMail(uint id)
    {
        var msg = MakeMsg("ReceiveMailReq");
        msg["id"] = id;
        return SendMsg(Router_Mail, msg);
    }
    /// <summary>
    /// 点赞场景
    /// 返回值：
    /// errorId: FailedOperate 操作错误
    /// like: 999 返回点赞后，这个场景的赞数
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    public GameSvrCallback LikeScene(string sceneName)
    {
        var msg = MakeMsg("LikeSceneReq");
        msg["sceneName"] = sceneName;
        return SendMsg(Router_Like, msg);
    }
    /// <summary>
    /// 获取指定场景列表的点赞数
    /// errorId : FailedOperate 操作错误
    /// likeScene : 字典格式，key：场景名，value：点赞数
    /// playerLike : 字典格式, key: 场景名， value: bool是否已经点赞
    /// </summary>
    /// <param name="sceneNameList"></param>
    /// <returns></returns>
    public GameSvrCallback GetLikeCount(string[] sceneNameList)
    {
        var msg = MakeMsg("GetLikeCountReq");
        LitJson.JsonData jd = new LitJson.JsonData();
        for (int i = 0; i < sceneNameList.Length; i++)
        {
            jd.Add(sceneNameList[i]);
        }
        msg["sceneNameList"] = jd;
        return SendMsg(Router_Like, msg);
    }
    /// <summary>
    /// 指定获取订单物品
    /// </summary>
    /// <param name="orderId">订单id</param>
    /// <returns></returns>
    public GameSvrCallback ConfirmPay(params string[] orderIds)
    {
        var msg = MakeMsg("ConfirmPayReq");
        LitJson.JsonData orderIdsJd = new LitJson.JsonData();
        for (int i = 0; i < orderIds.Length; i++)
        {
            orderIdsJd.Add(orderIds[i]);
        }
        msg["orderIds"] = orderIdsJd;
        return SendMsg(Router_Pay, msg);
    }
    /// <summary>
    /// 查询支付订单
    /// </summary>
    /// <returns></returns>
    public GameSvrCallback QueryPay()
    {
        var msg = MakeMsg("QueryPayReq");
        return SendMsg(Router_Pay, msg);
    }
    public GameSvrCallback SendMsg(string router, LitJson.JsonData msg)
    {
        RefreshConfigValue();
        var cb = NewGameSvrCallback();
        cb.SetFinish(OnCheckErrorId);
        HttpManager.GetSingleton().HttpPost(GetUrl(InternalUrl + router), HttpManager.ContentType_UrlEncoded, ToByte(msg), mCookie, cb.NewProgressCallback(), Timeout);
        return cb;
    }
}