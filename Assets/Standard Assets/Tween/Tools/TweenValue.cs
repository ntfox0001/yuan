using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uTools;

[System.Serializable]
public class TweenValueBase
{
    public Tweener.Style Style = Tweener.Style.Once;
    public float Duration = 1;
    public AnimationCurve AnimationCurve;
    public EaseType Method = EaseType.linear;
    public float Delay = 0;
    public bool IgnoreTimeScale = true;

    public void Apply(Tweener t)
    {
        t.style = Style;
        t.duration = Duration;
        t.animationCurve = AnimationCurve;
        t.method = Method;
        t.delay = Delay;
        t.ignoreTimeScale = IgnoreTimeScale;
    }

}

[System.Serializable]
public class TweenValue<T> : TweenValueBase
{
    public T From;
    public T To;
    public TweenValue() { }
    public TweenValue(T from, T to, Tweener.Style style, float duration, AnimationCurve curve, EaseType method, float delay, bool ignoreTimeScale)
    {
        From = from;
        To = to;
        Style = style;
        Duration = duration;
        AnimationCurve = curve;
        Method = method;
        Delay = delay;
        IgnoreTimeScale = ignoreTimeScale;
    }
    public void Apply(uTools.Tween<T> t)
    {
        t.from = From;
        t.to = To;
        base.Apply(t);
    }
    public float Length
    {
        get { return Delay + Duration; }
    }
}

[System.Serializable]
public class TweenFloatValue : TweenValue<float>
{
    public TweenFloatValue() { }
    public TweenFloatValue(float from, float to, float duration)
    {
        From = from;
        To = to;
        Duration = duration;
    }
    public TweenFloatValue(float from, float to, Tweener.Style style, float duration, AnimationCurve curve, EaseType method, float delay, bool ignoreTimeScale)
        : base(from, to, style,duration, curve, method, delay, ignoreTimeScale)
    {

    }
}
[System.Serializable]
public class TweenVec3Value : TweenValue<Vector3>
{
    public TweenVec3Value() { }
    public TweenVec3Value(Vector3 from, Vector3 to, Tweener.Style style, float duration, AnimationCurve curve, EaseType method, float delay, bool ignoreTimeScale)
    : base(from, to, style, duration, curve, method, delay, ignoreTimeScale)
    {

    }
}