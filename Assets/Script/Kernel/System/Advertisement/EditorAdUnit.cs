using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorAdUnit : AdUnitBase {
    public override void Initial(IAdInteractionListener listener, string userId, string codeId, int type, int id, params object[] param)
    {
        base.Initial(listener, userId, codeId, type, id, param);

        HasError = false;
        IsReady = true;
        mListener.OnAdLoad(this);
    }
    public override void Release()
    {
        
    }

    public override bool Show()
    {
        AdvertisementManager.GetSingleton().StartCoroutine(DelayAD());
        return true;
    }

    IEnumerator DelayAD()
    {
        yield return null;
        
        var adwin = WindowManager.GetSingleton().ActiveWindowStack.CreateWindow("EditorAdvertiserWindow", "global");
        mListener.OnAdShow(this);
        yield return new WaitForSecondsRealtime(2.0f);
        mListener.OnVideoComplete(this);
        mListener.OnAdClose(this);
        GameObject.Destroy(adwin);
    }
}
