using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

namespace uTools
{

    public class TweenFactor : Tween<float>
    {
        float mValue;
        public UnityEventFloat onFactorUpdate = null;
        public override float value
        {
            get
            {
                return mValue;
            }
            set
            {
                mValue = value;
                onFactorUpdate.Invoke(mValue);
            }
        }

        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = from + factor * (to - from);
        }


        public static TweenAlpha Begin(GameObject go, float from, float to, float duration = 1f, float delay = 0f)
        {
            TweenAlpha comp = Begin<TweenAlpha>(go, duration);
            comp.value = from;
            comp.from = from;
            comp.to = to;
            comp.duration = duration;
            comp.delay = delay;
            if (duration <= 0)
            {
                comp.Sample(1, true);
                comp.enabled = false;
            }
            return comp;
        }
    }
}