using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net;

public class GlobalObjects : Singleton<GlobalObjects>
{
    public enum GameArea
    {
        Global, Mainland, Steam,
    }
    public GameArea Area
    {
        get
        {
            return GameArea.Global;
        }
    }
    /// <summary>
    /// 游戏版本，每个游戏版本都会有自己对应的设置文件
    /// 游戏版本应该是只包含字母和数字的字符串
    /// </summary>
    public string GameVersion;
    public bool UseLocalConfig; // 是否使用本地配置文件
    public TextAsset LocalConfig;  // 本地配置文件路径
    public string WebConfigPath;  // 网络配置文件路径
    public string GlobalWebConfigPath; // 全球服网络配置文件路径
    public string ScriptClassName;  // 脚本启动类名
    public string MajorVersion; // 主版本
    public UnityEngine.EventSystems.EventSystem EventySystem;
    public Canvas UIRoot;
    public Canvas SceneRoot;
    public Canvas ShadeRoot;
    public Camera MainCamera;
    public string PersistName;
    /// <summary>
    /// 对各个模块进行初始化，下载资源等
    /// </summary>
    public ProcedureBase StartupProcedure;
    /// <summary>
    /// 游戏逻辑中预先加载的数据
    /// </summary>
    public ProcedureBase GameProcedure;
    IManagerBase[] mManagers;
    // 全局事件
    public EventHandler EventHandler { get; private set; }
    // 平台相关调用
    public IPlatformUtil PlatformUtil { get; private set; }

    IPersistent mPersistent;
    public IPersistent Persistent { get { return mPersistent; } }
    public IPersistent LocalPersistent { get; private set; }
    List<IApplicationEvent> mApplicationEventList = new List<IApplicationEvent>();


    private void Awake()
    {
        AuthHelper.InitialComponent(gameObject);
        StartCoroutine(Initial());
    }
    public string GetPlatformVersionFilename()
    {
        string fn = "";
#if UNITY_IOS
        fn = "iosVersion.json";
#elif UNITY_ANDROID
        fn = "androidVersion.json";
#else
        fn = "version.json";
#endif

        return fn;
    }
    /// <summary>
    /// 返回对应区域的web config path
    /// </summary>
    /// <returns></returns>
    public string GetWebConfigPath()
    {
        return GlobalWebConfigPath;
    }
    
    public string GetIndieNiNiName()
    {
        string p1 = "abcdefghijklimnopqrstuvwxyzABCDEFGHIJKLIMNOPQRSTUVWXYZ!@#$%^&*()_+-={}|[]\\<>?,./~`";
        string p = "#";
        string p2 = "@";
        string p3 = "^";
        string p4 = "*";
        string p5 = "f";
        string p6 = "T";
        string p7 = "z";
        string p8 = "m";
        string p9 = "+";
        string p10 = "-";
        string p11 = "G";
        string p12 = "e";
        string p13 = "k";
        string p14 = "B";
        string p15 = "c";
        string p16 = "P";
        string p17 = "z";
        string p18 = "?";
        string p19 = "q";

        //return p + p4 + p2 + p13 + p11 + p3 + p6 + p9 + p11 + p12 + p14 + p19 + p17 + p2 + p18 + p10 + p5 + p6 + p7 + p8 + p15 + p16;
        char[] ca = p1.ToCharArray();
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        int s = 15;
        for (int i = 0; i < 32; i++)
        {
            int a = s + i * 5 - 2;
            int r = a % ca.Length;
            sb.Append(ca[r]);
        }
        return sb.ToString();
    }
    public string GetIndieNiNiNewName(string id)
    {
        string p1 = "ABCDEFGHIJKLIMNOPQRSTUVWXYZ!@#$%^&*()_+-={}|[]\\<>?,./~`abcdefghijklimnopqrstuvwxyz";
        char[] ca = p1.ToCharArray();
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        int s = 15;
        for (int i = 0; i < 32; i++)
        {
            int a = s + i * 5 - 2;
            int r = a % ca.Length;
            sb.Append(ca[r]);
        }
        // todo： 应该用id转为int在取余
        int pos = id.Length % 13;
        sb.Insert(pos, id);

        return sb.ToString();
    }
    IEnumerator Initial()
    {
        
        Application.targetFrameRate = 60;
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        EventHandler = new EventHandler();

        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
        mManagers = GetComponentsInChildren<IManagerBase>();

        yield return null;


#if UNITY_IOS
        mPersistent = new XmlPersistent(PersistName);
#elif UNITY_ANDROID
        //mPersistent = new XmlPersistent(PersistName);
        mPersistent = new XmlPersistentEncrypt(PersistName, GetIndieNiNiName());
#endif


        PlatformUtil = gameObject.AddComponent<PlatformUtil>();

        foreach (IManagerBase mb in mManagers)
        {
            mb.Initial();
        }

        if (StartupProcedure != null)
        {
            StartupProcedure.Initial();
        }
        

        yield return null;

        Debug.Log("DeviceName: " + SystemInfo.deviceName);
        if (StartupProcedure != null)
        {
            StartupProcedure.Do();
        }

    }

    private void OnDestroy()
    {
        foreach (IManagerBase mb in mManagers)
        {
            mb.Release();
        }
    }

    public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        bool isOk = true;
        // If there are errors in the certificate chain,
        // look at each error to determine the cause.
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            for (int i = 0; i < chain.ChainStatus.Length; i++)
            {
                if (chain.ChainStatus[i].Status == X509ChainStatusFlags.RevocationStatusUnknown)
                {
                    continue;
                }
                chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                bool chainIsValid = chain.Build((X509Certificate2)certificate);
                if (!chainIsValid)
                {
                    isOk = false;
                    Debug.LogError("Failed to SSL certificate validation.");
                    break;
                }
            }
        }
        return isOk;
    }

    public void RegisterApplicationEvent(IApplicationEvent appEvent)
    {
        mApplicationEventList.Add(appEvent);
    }
    public void UnregisterApplicationEvent(IApplicationEvent appEvent)
    {
        mApplicationEventList.Remove(appEvent);
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            for (int i = 0; i < mApplicationEventList.Count; i++)
            {
                mApplicationEventList[i].ApplicationPause();
            }
            Debug.Log("Application pause.");
        }
        else
        {
            for (int i = 0; i < mApplicationEventList.Count; i++)
            {
                mApplicationEventList[i].ApplicationResume();
            }
            Debug.Log("Application resume.");
        }
    }
    //private void OnApplicationQuit()
    //{
    //    if (AppPause != null)
    //    {
    //        AppPause();
    //    }
    //}
    public void SaveJson(string key, LitJson.JsonData jd)
    {
        Persistent.SaveData(key, jd.ToJson());
    }
    public LitJson.JsonData LoadJson(string key)
    {
        string js = Persistent.LoadDataString(key);
        LitJson.JsonData jd = LitJson.JsonMapper.ToObject(js);
        return jd;
    }
}