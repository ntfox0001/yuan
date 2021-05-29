using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class AuthBase : IAuth
{
    protected List<PayProductInfo> mProductList = new List<PayProductInfo>();
    const float Timeout = 10.0f;

    protected InvokeEnumOpt mInitial = new InvokeEnumOpt();

    // 登陆
    protected InvokeEnumOpt mLogin = new InvokeEnumOpt();

    // 退出登录
    protected InvokeEnumOpt mLogout = new InvokeEnumOpt();

    // 初始化支付
    protected InvokeEnumOpt mPayInitial = new InvokeEnumOpt();

    // 绑定微信
    protected InvokeEnumOpt mBindAccount = new InvokeEnumOpt();

    protected System.Action<string> mPaySuccessFunc;
    protected System.Action<AuthError> mPayFailFunc;

    protected System.Action mShareSuccessFunc = null;
    protected System.Action<AuthError> mShareFailFunc = null;

    protected bool IsInitialFinished() { return mInitial.Finished; }
    protected bool IsLoginFinished() { return mLogin.Finished; }
    protected bool IsPayFinished() { return mPayInitial.Finished; }
    protected bool IsLogoutFinished() { return mLogout.Finished; }
    protected bool IsBindAccountFinished() { return mBindAccount.Finished; }
    protected string mCheckRestoreThroughGo;
    public AuthError Error { get; protected set; }
    public virtual CustomYieldInstruction Initial()
    {
        return new WaitForEnumOpt(mInitial);
    }
    public virtual CustomYieldInstruction Login()
    {
        return new WaitForEnumOpt(mLogin);
    }
    public virtual CustomYieldInstruction InitialPay(System.Action<string> successFunc, System.Action<AuthError> failFunc)
    {
        mPaySuccessFunc = successFunc;
        mPayFailFunc = failFunc;
        return new WaitForEnumOpt(mPayInitial);
    }
    public virtual CustomYieldInstruction Logout()
    {
        mLogin.Clear(); // 重置登陆成功状态
        return new WaitForEnumOpt(mLogout);
    }
    public virtual CustomYieldInstruction BindAccount(string type)
    {
        return new WaitForEnumOpt(mBindAccount);
    }
    
    public virtual bool IsInitialSuccessed() { return mInitial.Successed; }
    public virtual bool IsLoginSuccessed() { return mLogin.Successed; }
    public virtual bool IsPayInitSuccessed() { return mPayInitial.Successed; }
    public virtual bool IsLogoutSuccessed() { return mLogout.Successed; }
    public virtual bool IsBindAccountSuccessed() { return mBindAccount.Successed; }
    public abstract void SetLoginType(string type);
    public abstract string GetLoginType();
    public abstract string GetId();
    public abstract string GetKey();
    public abstract string GetDeviceId();
    public abstract string GetBindInfo();

    public virtual void Buy(string productId, System.Action<string> successFunc, System.Action<AuthError> failFunc)
    {
        
    }
    public virtual void CheckRestore(System.Action<List<string>> successFunc, System.Action<AuthError> failFunc, string throughgo)
    {
        mCheckRestoreThroughGo = throughgo;
    }
    public virtual void TraceEvent(string eventName, string properties)
    {

    }
    public virtual void AdjustTraceEvent(string eventName)
    {

    }
    public virtual void SharePic(string picPath, SocialShareType type, System.Action successFunc, System.Action<AuthError> failFunc)
    {
        mShareSuccessFunc = successFunc;
        mShareFailFunc = failFunc;
    }
    public virtual bool LoginAvailable(string loginType)
    {
        return true;
    }

    public virtual void Exit()
    {
        
    }
    public virtual void Update()
    {

    }
    public virtual void UpdatePlayerInfo()
    {

    }
    public virtual List<PayProductInfo> GetProductInfoList()
    {
        return mProductList;
    }
    public virtual int GetAge()
    {
        return -1;
    }
    public virtual CustomYieldInstruction GuestTimeLimit()
    {
        return null;
    }
}
