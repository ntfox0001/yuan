using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStartEffect : MonoBehaviour
{
    public TweenFloatValue Alpha = new TweenFloatValue(0, 1, uTools.Tweener.Style.Once, 0.2f, null, uTools.EaseType.linear, 0, true);
    public TweenVec3Value Scale = new TweenVec3Value(Vector3.zero, Vector3.one, uTools.Tweener.Style.Once, 0.2f, null, uTools.EaseType.linear, 0, true);

    private void Awake()
    {
        var ta = gameObject.AddComponent<uTools.TweenAlpha>();
        Alpha.Apply(ta);
        ta.PlayForward();

        var ts = gameObject.AddComponent<uTools.TweenScale>();
        Scale.Apply(ts);
        ts.PlayForward();
    }
}
