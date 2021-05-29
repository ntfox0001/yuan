using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

public class EditorAdvertiser : AdUnitManager, IAdvertiser, IAdInteractionListener
{
    IAdvertiserCallback mCallback;
    static public bool sAdReady = true;

    public override string AdName { get { return "editor"; } }

    public override bool Initial(string userId, IAdvertiserCallback cb, AdvertisementSetting adSetting)
    {
        if (!base.Initial(userId, cb, adSetting))
        {
            return false;
        }

        // 初始化广告unit
        AdvertisementManager.GetSingleton().StartCoroutine(DelayInit());
        return true;
    }
    IEnumerator DelayInit()
    {
        yield return null;
        mAdCallback.OnInitial(true, "");
        yield return null;
        BeginUpdateAdUnit();

    }

    public override bool SupportPlatform()
    {
#if UNITY_EDITOR
        return true;
#else
        return false;
#endif
    }
    protected override AdUnitBase NewAdUnit(int type, int id)
    {
        string codeId = GetCodeId(type, id);
        if (codeId == "")
        {
            return new NoReadyAdUnit();
        }

        Debug.Log("new ad, type:" + type + " id:" + id);
        var ad = new EditorAdUnit();
        ad.Initial(this, mUserId, codeId, type, id);
        return ad;
    }
}
