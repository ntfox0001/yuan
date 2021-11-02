using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
/// <summary>
/// 启动流程
/// 1， 判断是否使用本地config
/// 2， 下载config
/// 3， 解析config
/// </summary>
public class StartupProcedure : ProcedureBase
{
    public Action OnDownloadConfigFinished; // 设置文件加载完成

    GameObject mStartupWindow;
    bool mIsFinished = false;
    public override void Do()
    {
        // 创建startup window
        Func<CrossFadeUI.WillFadeOutReturn> willFadeOut = OnWillFadeOut;
        Action startupWindowFinished = OnStartupWindowFinished;
        // 加载shtartup ui
        mStartupWindow = WindowManager.GetSingleton().TopWindowStack.CreateWindow("StartupWindow", "startup", willFadeOut, startupWindowFinished);
        
    }

    public override void Initial()
    {
        // 创建第一个windowStack
        WindowManager.GetSingleton().CreateWindowStack();
#if UNITY_EDITOR || DREAM_OFFLINE
        // 是否使用本地config
        if (GlobalObjects.GetSingleton().UseLocalConfig)
        {
            Debug.Log("Load local config file.");
            ConfigManager.GetSingleton().LoadXml(GlobalObjects.GetSingleton().LocalConfig.text);
            TouchDownloadConfigFinished();
        }
        else
#endif
        {

            // 下载config
            StartCoroutine(StartDownloadConfigCoroutine());    
            
        }
    }
    IEnumerator StartDownloadConfigCoroutine()
    {
        string url = GlobalObjects.GetSingleton().GetWebConfigPath() + "/config/" + GlobalObjects.GetSingleton().GameVersion + ".xml";
        var waitxml = ConfigManager.GetSingleton().LoadXmlFromUrl(url);
        yield return waitxml;
        string fn = Application.persistentDataPath + "/" + GlobalObjects.GetSingleton().GameVersion + ".xml";
        if (!ConfigManager.GetSingleton().LoadSuccess)
        {
            // 下载失败，可能是用户禁止了程序访问网络，或者设备离线
            Debug.Log("Load local config file.");
            
            if (File.Exists(fn))
            {
                // 读取上次的config文件
                ConfigManager.GetSingleton().LoadXmlFile(fn);
            }
            else
            {
                Debug.LogError("Failed to config httpGet.");
            }
            TouchDownloadConfigFinished();
        }
        else
        {
            // 下载完成
            Debug.Log("Load remote config file.");
            // 下载成功
            ConfigManager.GetSingleton().SaveXmlFile(fn);
            TouchDownloadConfigFinished();
        }
    }

    // 触发下载config完成事件
    void TouchDownloadConfigFinished()
    {
        if (OnDownloadConfigFinished != null)
        {
            OnDownloadConfigFinished();
        }
        // 读取后台下载资源路径
        string backendResUrl = ConfigManager.GetSingleton().GetPlatformString(ConfigValues.BackendResPath, "");
        Debug.Log("backend res url: " + backendResUrl);
        if (backendResUrl != "")
        {
            backendResUrl = GlobalObjects.GetSingleton().GetWebConfigPath() + backendResUrl;
            // 有下载资源路径
            BackendDownloadManager.GetSingleton().LoadPackageFile(backendResUrl);
        }
        // todo: 热更界面
        
        // 资源检查结束
        TouchFinished();
    }

    // 触发流程结束事件
    void TouchFinished()
    {
        // 完成
        Debug.Log("StartupProcedure Finished.");
        if (OnInitialFinished != null)
        {
            OnInitialFinished();
        }

        // GameProcedure回调
        GlobalObjects.GetSingleton().GameProcedure.OnInitialFinished += OnGameProcedureInitialFinished;
        GlobalObjects.GetSingleton().GameProcedure.Initial();        
    }
    void OnGameProcedureInitialFinished()
    {
        mIsFinished = true;
    }
    // 控制最后一页是否淡出
    CrossFadeUI.WillFadeOutReturn OnWillFadeOut()
    {
        // 当所有流程都结束了，才能开始淡出
        if (mIsFinished)
        {
            // 开始新流程
            Debug.Log("GameProcedure Do.");
            GlobalObjects.GetSingleton().GameProcedure.Do();
        }
        return mIsFinished ? CrossFadeUI.WillFadeOutReturn.WFOR_Continue : CrossFadeUI.WillFadeOutReturn.WFOR_Wait;
    }

    // startupwindow退出了
    void OnStartupWindowFinished()
    {
        Debug.Log("OnStartupWindowFinished");
        Destroy(mStartupWindow);
    }

}
