using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAuth
{
    /// <summary>
    /// 初始化sdk
    /// </summary>
    /// <returns></returns>
    CustomYieldInstruction Initial();
    bool IsInitialSuccessed();
    /// <summary>
    /// 登陆
    /// </summary>
    /// <returns></returns>
    CustomYieldInstruction Login();
    bool IsLoginSuccessed();
    /// <summary>
    /// 设置登陆方式 Guest,Wechat
    /// </summary>
    /// <param name="type"></param>
    void SetLoginType(string type);
    string GetLoginType();
    /// <summary>
    /// 返回userId
    /// </summary>
    /// <returns></returns>
    string GetId();
    string GetKey();
    string GetDeviceId();
    string GetBindInfo();
    /// <summary>
    /// 支付初始化
    /// </summary>
    /// <param name="successFunc">补单成功回调，参数是productId</param>
    /// <param name="failFunc">补单失败回调，参数是err msg</param>
    /// <returns></returns>
    CustomYieldInstruction InitialPay(System.Action<string> successFunc, System.Action<AuthError> failFunc);
    bool IsPayInitSuccessed();
    /// <summary>
    /// 购买
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="successFunc">当前支付成功，参数是productId</param>
    /// <param name="failFunc">当前支付失败，参数是err msg</param>
    void Buy(string productId, System.Action<string> successFunc, System.Action<AuthError> failFunc);

    /// <summary>
    /// 恢复之前的非消耗购买
    /// </summary>
    /// <param name="successFunc"></param>
    /// <param name="failFunc"></param>
    /// <param name="throughgo">all, subscribe, nonConsumption</param>
    void CheckRestore(System.Action<List<string>> successFunc, System.Action<AuthError> failFunc, string throughgo);
    /// <summary>
    /// 发送bi事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="properties"></param>
    void TraceEvent(string eventName, string properties);
    /// <summary>
    /// 发送adjust事件
    /// </summary>
    /// <param name="eventName"></param>
    void AdjustTraceEvent(string eventName);

    void SharePic(string picPath, SocialShareType type, System.Action successFunc, System.Action<AuthError> failFunc);
    bool LoginAvailable(string loginType);
    /// <summary>
    /// 退出
    /// </summary>
    /// <returns></returns>
    void Exit();
    /// <summary>
    /// 更新
    /// </summary>
    void Update();
    /// <summary>
    /// 退出登陆
    /// </summary>
    CustomYieldInstruction Logout();
    bool IsLogoutSuccessed();
    CustomYieldInstruction BindAccount(string type);
    bool IsBindAccountSuccessed();
    /// <summary>
    /// 更新玩家信息，每个sdk接入类自己实现
    /// </summary>
    void UpdatePlayerInfo();

    List<PayProductInfo> GetProductInfoList();
    AuthError Error { get; }

    /// <summary>
    /// 返回用户年龄
    /// </summary>
    /// <returns></returns>
    int GetAge();
    /// <summary>
    /// 开始一个游客限时过程
    /// </summary>
    /// <returns></returns>
    CustomYieldInstruction GuestTimeLimit();
}
