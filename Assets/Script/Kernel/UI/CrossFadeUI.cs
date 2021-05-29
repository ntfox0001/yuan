using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(uTools.TweenAlpha))]
public class CrossFadeUI : MonoBehaviour
{
    public enum WillFadeOutReturn
    {
        WFOR_Wait,
        WFOR_Continue,
    }
    public delegate void FadeFinishedHandler();
    public event FadeFinishedHandler OnFinished;
    public delegate WillFadeOutReturn WillFadeOutHandler();
    public event WillFadeOutHandler OnWillFadeOut;

    public CanvasGroup CanvasGroup;
    public uTools.TweenAlpha TweenAlpha;
    public float WaitingTime = 1.0f;
    public float FadeInTime = 1.0f;
    public float FadeOutTime = 1.0f;
    public bool ClickToSkip = true;

    DynamicEventGameObject mDynamicEventGameObject;

    bool mNeedSkip = false;
	// Use this for initialization
	void Start () {

        CanvasGroup.alpha = 0;
        mDynamicEventGameObject = DynamicEventGameObject.Get(this);
        mDynamicEventGameObject.Click += OnCrossFadeUIClick;

        
    }

    public void PlayForward()
    {
        StartCoroutine(StartFade());
    }

    IEnumerator StartFade()
    {
        TweenAlpha.duration = FadeInTime;
        TweenAlpha.ResetToBeginning();
        TweenAlpha.PlayForward();

        yield return new WaitForSeconds(FadeInTime);

        if (ClickToSkip)
        {
            WaitWhile wffr = new WaitWhile(NeedSkip);
            WaitForSecondsRealtime wfsr = new WaitForSecondsRealtime(WaitingTime);
            yield return new WaitForMultiObjects(WaitForMultiObjects.WaitForType.WaitForAny, wffr, wfsr);
        }
        else
        {
            yield return new WaitForSecondsRealtime(WaitingTime);
        }

        // 等待
        yield return new WaitWhile(WaitForWillFadeOut);

        TweenAlpha.duration = FadeOutTime;
        TweenAlpha.ResetToBeginning();
        TweenAlpha.PlayReverse();

        yield return new WaitForSecondsRealtime(FadeOutTime);
        
        if (OnFinished != null)
        {
            OnFinished();
        }
    }
	
    bool NeedSkip()
    {
        return !mNeedSkip;
    }
    /// <summary>
    /// fadein之后，将要进入fadeout时，调用
    /// </summary>
    /// <returns></returns>
    bool WaitForWillFadeOut()
    {
        if (OnWillFadeOut != null)
        {
            return OnWillFadeOut() == WillFadeOutReturn.WFOR_Wait;
        }
        return false;
    }
    void OnCrossFadeUIClick(UnityEngine.EventSystems.PointerEventData ev){
        mNeedSkip = true;
    }

}
