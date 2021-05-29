using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(uTools.TweenAlpha))]
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(UnityEngine.UI.Image))]
public class FadeMaskUI : MonoBehaviour
{
    [System.Serializable]
    public class Fade
    {
        public float FadeIn;
        public FadeValue FadeInValue = new FadeValue(0, 1);
        public AnimationCurve FadeInCurve;
        public float FadeOut;
        public FadeValue FadeOutValue = new FadeValue(1, 0);
        public AnimationCurve FadeOutCurve;
        public float FadeKeep;
        public Color FadeColor = Color.white;
    }
    [System.Serializable]
    public class FadeValue
    {
        public FadeValue(float from, float to)
        {
            From = from;
            To = to;
        }
        public float From;
        public float To;
    }
    public uTools.TweenAlpha TweenAlpha;
    public UnityEngine.UI.Image Image;
    private void Awake()
    {
        if (TweenAlpha == null)
        {
            TweenAlpha = GetComponent<uTools.TweenAlpha>();
            TweenAlpha.method = uTools.EaseType.none;
        }
    }

    public void Play(Fade fade, System.Action willFadeOut, System.Action finished)
    {
        StartCoroutine(PlayCoroutine(fade, willFadeOut, finished));
    }
    IEnumerator PlayCoroutine(Fade fade, System.Action willFadeOut, System.Action finished)
    {
        Image.color = fade.FadeColor;
        TweenAlpha.ResetToBeginning();
        TweenAlpha.from = fade.FadeInValue.From;
        TweenAlpha.to = fade.FadeInValue.To;
        TweenAlpha.duration = fade.FadeIn;
        TweenAlpha.animationCurve = fade.FadeInCurve;
        TweenAlpha.PlayForward();
        yield return new WaitForSecondsRealtime(fade.FadeIn + fade.FadeKeep);
        // 开始interlude
        if (willFadeOut != null)
        {
            willFadeOut();
        }
        TweenAlpha.ResetToBeginning();
        TweenAlpha.from = fade.FadeOutValue.From;
        TweenAlpha.to = fade.FadeOutValue.To;
        TweenAlpha.duration = fade.FadeOut;
        TweenAlpha.animationCurve = fade.FadeOutCurve;
        TweenAlpha.PlayForward();
        yield return new WaitForSecondsRealtime(fade.FadeOut);
        if (finished != null)
        {
            try
            {
                finished();
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Scene had destroyed!!!: {0}.", e.Message);
            }

        }
        Destroy(gameObject);
    }

}
