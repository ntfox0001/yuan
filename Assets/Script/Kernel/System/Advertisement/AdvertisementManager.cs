using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class AdvertisementManager : Singleton<AdvertisementManager>, IManagerBase, IAdvertiserCallback
{
    public enum ShowAdCallbackState
    {
        Open = 1,
        Click = 2,
        Close = 3,
        Given = 4,
        Abandon = 5,
        Loaded = 6,
        DownloadActive = 7,
        DownloadFailed = 8,
        DownloadFinished = 9,
        Installed = 10,
    }
    public class WaitPlayAd
    {
        public bool PlayAd { get; private set; }
        public WaitUntil Wait { get; private set; }

        public WaitPlayAd(bool playAd, System.Func<bool> predicate)
        {
            PlayAd = playAd;
            if (predicate != null)
            {
                Wait = new WaitUntil(predicate);
            }
            else
            {
                Wait = null;
            }
        }
    }
    [Tooltip("Only in Editor")]
    public string DebugAdSettingKey = "";
    public TextAsset AdConfig;
    Dictionary<string, AdvertisementSetting> mAdvertisementSettingMap;
    IAdvertiser mAdvertiser = null;
    bool mAdInitialied = false;

    System.Action<ShowAdCallbackState, string, string> mCurrentCallback;
    long mCurrentStartTime = 0;
    bool mCurrentADHasOpen = false;

    bool mIsAdPlaying = false;
    System.Func<WaitPlayAd> mPlayAdFunc;
    public void Initial()
    {
        // do nothing
    }
    string GetAdSettingKey(IAdvertiser ad)
    {
        string key = ad.AdName;
#if UNITY_ANDROID
        key += "android";
#elif UNITY_IOS
        key += "ios";
#else
#error "NoDefine"
#endif

        return key;
    }
    AdvertisementSetting GetAdSetting(IAdvertiser ad)
    {
        AdvertisementSetting adSetting = null;
        string key = GetAdSettingKey(ad);
        mAdvertisementSettingMap.TryGetValue(key, out adSetting);

        return adSetting;
    }
    /// <summary>
    /// 初始化广告系统
    /// </summary>
    /// <param name="jd"></param>
    /// <param name="playAdFunc">返回值小于等于=时，播放广告，大于0时，为等待一段时间后，跳过广告</param>
    /// <returns></returns>
    public CustomYieldInstruction InitialAd(System.Func<WaitPlayAd> playAdFunc)
    {
        if (mAdInitialied)
        {
            return null;
        }
        mAdInitialied = true;
        mPlayAdFunc = playAdFunc;

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(AdConfig.text);
        mAdvertisementSettingMap = AdvertisementSetting.GetAdSetting(doc);

        var adList = GetComponents<IAdvertiser>();

        for (int i = 0; i < adList.Length; i++)
        {
            mAdvertiser = adList[i];
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(DebugAdSettingKey) && GetAdSettingKey(mAdvertiser) != DebugAdSettingKey)
            {
                continue;
            }
#endif
            var adSetting = GetAdSetting(mAdvertiser);
            if (adSetting != null && adSetting.Enable)
            {
                if (mAdvertiser.Initial(AuthManager.GetSingleton().Auth.GetId(), this, adSetting))
                {
                    Debug.Log("AdSystem select: " + mAdvertiser.AdName);
                    return new WaitUntil(() => { return mAdInitialied; });
                }
            }
        }
        Debug.LogWarning("There isn't Advertiser.");

        return null;
    }
    public bool ShowAD(int type, System.Action<ShowAdCallbackState, string, string> cb)
    {
        if (mAdvertiser == null) return false;
        if (mIsAdPlaying)
        {
            return false;
        }

        if (!IsReady(type))
        {
            return false;
        }

        mIsAdPlaying = true;
        SoundManager.GetSingleton().Mute = true;
        mCurrentCallback = cb;
        mCurrentStartTime = DateTimeUtil.GetTimeStamp();
        mCurrentADHasOpen = false;
        
        mAdvertiser.ShowAD(type);
        
        return true;
    }

    public bool ShowADByAdTicked(int type, System.Action<ShowAdCallbackState, string, string> cb)
    {
        var result = mPlayAdFunc();
        if (result.PlayAd)
        {
            return ShowAD(type, cb);
        }
        StartCoroutine(DelayAdCallback(result, cb));
        return true;
    }
    IEnumerator DelayAdCallback(WaitPlayAd waitter, System.Action<ShowAdCallbackState, string, string> cb)
    {   
        yield return waitter.Wait;
        cb(ShowAdCallbackState.Open, "", "");
        cb(ShowAdCallbackState.Given, "", "");
        cb(ShowAdCallbackState.Close, "", "");
    }
    /// <summary>
    /// 广告是否准备好了
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool IsReady(int type)
    {
        bool isReady = mAdvertiser.IsReady(type);
        if (!isReady)
        {
            // GameFollow.GetSingleton().OnAdNoReady(type);
        }
        return isReady;
    }
    public void Release()
    {
        if (mAdvertiser == null) return;
        mAdvertiser.Release();
    }

    public void OnInitial(bool result, string msg)
    {
        mAdInitialied = true;
        if (!result)
        {
            Debug.LogError("Failed to advertisement initial.");
        }
    }

    public void OnShowAd(ShowAdCallbackState state, string placeId, string msg)
    {
        if (mCurrentCallback != null)
        {
            mCurrentCallback(state, placeId, msg);
            if (state == ShowAdCallbackState.Open)
            {
                mCurrentADHasOpen = true;
            }
            else if (state == ShowAdCallbackState.Close)
            {
                OnEndCurrentAd();
            }
        }
    }
    void OnEndCurrentAd()
    {
        SoundManager.GetSingleton().Mute = false;
        // 这里不能设置为null，有些广告平台的given事件不是在close之前调用
        //mCurrentCallback = null;
        mIsAdPlaying = false;
    }
}
