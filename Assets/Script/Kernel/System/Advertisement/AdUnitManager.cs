using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AdUnitManager : MonoBehaviour, IAdInteractionListener, IAdvertiser
{
    public float Expire = 60.0f * 45.0f; // 45 min
    public bool AdDebug = true;
    protected IAdvertiserCallback mAdCallback;
    protected AdvertisementSetting mAdSetting;
    protected string mUserId;

    List<List<AdUnitBase>> mAdUnitList = new List<List<AdUnitBase>>();

    public abstract string AdName { get; }
    public virtual bool Initial(string userId, IAdvertiserCallback cb, AdvertisementSetting adSetting)
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(AdvertisementManager.GetSingleton().DebugAdSettingKey))
        {
            if (!SupportPlatform())
            {
                return false;
            }
        }
#else
        if (!SupportPlatform())
        {
            return false;
        }
#endif
        mAdCallback = cb;
        mUserId = userId;
        mAdSetting = adSetting;

        return true;
    }

    public abstract bool SupportPlatform();
    protected void BeginUpdateAdUnit()
    {
        for (int i = 0; i < 2; i++)
        {
            List<AdUnitBase> units = new List<AdUnitBase>();
            for (int j = 0; j < 2; j++)
            {
                units.Add(NewAdUnit(i, j));
            }
            mAdUnitList.Add(units);
        }

        AdvertisementManager.GetSingleton().StartCoroutine(UpdateAdUnit());

    }
    IEnumerator UpdateAdUnit()
    {

        // init map
        while (true)
        {
            // 检查adunit，是否过期
            for (int i = 0; i < 2; i++)
            {
                var subList = mAdUnitList[i];
                for (int j = 0; j < 2; j++)
                {
                    if (subList[j] == null)
                    {
                        continue;
                    }
                    if (subList[j].HasError || subList[j].HoldTime > Expire)
                    {
                        // 广告没准备好时，holdtime总是0，所以不用再检查一次ready
                        // 过期了，那么干掉
                        Debug.Log("AdUnit " + subList[j].CodeId + " expire: " + subList[j].HoldTime);
                        RenewAdUnit(subList[j]);
                    }
                }
            }
            Debug.Log("Update AdUnit.");
            yield return new WaitForSecondsRealtime(60.0f);// 1min
        }
    }
    protected abstract AdUnitBase NewAdUnit(int type, int id);
    protected void RenewAdUnit(AdUnitBase unit)
    {
        mAdUnitList[unit.Type][unit.Id].Release();
        // 重建一个
        mAdUnitList[unit.Type][unit.Id] = NewAdUnit(unit.Type, unit.Id);
    }
    protected string GetCodeId(int type, int id)
    {
        try
        {
            switch (type)
            {
                case 0:
                    return mAdSetting.RewardedId[id];
                case 1:
                    return mAdSetting.InterstitialId[id];
            }
        }
        catch (System.Exception)
        {

        }
        return "";
    }
    protected AdUnitBase GetAvailableAdUnit(int type)
    {
        // 找到创建时间最长的那个
        AdUnitBase ad = null;
        var subList = mAdUnitList[type];
        for (int i = 0; i < 2; i++)
        {
            if (subList[i]!= null && subList[i].IsReady)
            {
                if (ad == null)
                {
                    ad = subList[i];
                }
                else
                {
                    if (ad.HoldTime < subList[i].HoldTime)
                    {
                        ad = subList[i];
                    }
                }
            }
        }
        return ad;
    }
    public virtual bool IsReady(int type)
    {
        // 无视type，id
        return GetAvailableAdUnit(type) != null;
    }
    public virtual bool ShowAD(int type)
    {
        // 无视type,id
        var ad = GetAvailableAdUnit(type);
        if (ad == null)
        {
            Debug.LogError("AdUnit is null, need check ready before show.");
            return false;
        }

        ad.Show();
        return true;
    }
    public virtual void Release()
    {
        for (int i = 0; i < mAdUnitList.Count; i++)
        {
            var subList = mAdUnitList[i];
            for (int j = 0; j < subList.Count; j++)
            {
                if (subList[j] != null)
                {
                    subList[j].Release();
                    subList[j] = null;
                }
            }
        }
    }
    public virtual void OnError(AdUnitBase self, int code, string message)
    {

    }

    public virtual void OnAdLoad(AdUnitBase self)
    {
        mAdCallback.OnShowAd(AdvertisementManager.ShowAdCallbackState.Loaded, "", "");
    }

    public virtual void OnAdCached(AdUnitBase self)
    {

    }

    public virtual void OnAdShow(AdUnitBase self)
    {
        mAdCallback.OnShowAd(AdvertisementManager.ShowAdCallbackState.Open, "", "");
    }

    public virtual void OnAdVideoBarClick(AdUnitBase self)
    {
        mAdCallback.OnShowAd(AdvertisementManager.ShowAdCallbackState.Click, "", "");
    }

    public virtual void OnAdClose(AdUnitBase self)
    {
        mAdCallback.OnShowAd(AdvertisementManager.ShowAdCallbackState.Close, "", "");
        RenewAdUnit(self);
    }

    public virtual void OnVideoComplete(AdUnitBase self)
    {
        mAdCallback.OnShowAd(AdvertisementManager.ShowAdCallbackState.Given, "", "");
    }

    public virtual void OnVideoError(AdUnitBase self)
    {

    }

    public virtual void OnDownloadActive(string appName, AdUnitBase self)
    {
        mAdCallback.OnShowAd(AdvertisementManager.ShowAdCallbackState.DownloadActive, "", appName);
    }

    public virtual void OnDownloadPaused(string appName, AdUnitBase self)
    {

    }

    public virtual void OnDownloadFailed(string appName, AdUnitBase self)
    {
        mAdCallback.OnShowAd(AdvertisementManager.ShowAdCallbackState.DownloadFailed, "", appName);
    }

    public virtual void OnDownloadFinished(string appName, AdUnitBase self)
    {
        mAdCallback.OnShowAd(AdvertisementManager.ShowAdCallbackState.DownloadFinished, "", appName);
    }

    public virtual void OnInstalled(string appName, AdUnitBase self)
    {
        mAdCallback.OnShowAd(AdvertisementManager.ShowAdCallbackState.Installed, "", appName);
    }

}
