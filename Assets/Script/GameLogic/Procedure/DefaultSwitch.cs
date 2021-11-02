using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultSwitch : ISwitch
{
    int mCall = 0;
    ISwitchTarget mPreTarget;
    float mMinLoadingTime = 2.0f;
    public DefaultSwitch() { }
    public DefaultSwitch(float minLoadingTime)
    {
        mMinLoadingTime = minLoadingTime;
    }
    public WaitForMultiObjects.WaitReturn OnBeginFadeIn(ISwitchTarget pre, ISwitchTarget target)
    {
        mCall = 0;
        return WaitForMultiObjects.WaitReturn.Continue;
    }
    public WaitForMultiObjects.WaitReturn OnEndFadeIn(ISwitchTarget pre, ISwitchTarget target, System.Action onFadeInFunc)
    {
        if (mCall == 0)
        {
            if (pre != null)
                pre.Release();

            mCall = 1;
        }
        if (onFadeInFunc != null)
        {
            onFadeInFunc();
        }
        return target.Preload();
    }
    public WaitForMultiObjects.WaitReturn OnBeginFadeOut(ISwitchTarget pre, ISwitchTarget target)
    {
        return target.Postload();
    }
    public WaitForMultiObjects.WaitReturn OnEndFadeOut(ISwitchTarget pre, ISwitchTarget target)
    {
        return WaitForMultiObjects.WaitReturn.Continue;
    }

    public float MinLoadingTime()
    {
        return mMinLoadingTime;
    }
}
