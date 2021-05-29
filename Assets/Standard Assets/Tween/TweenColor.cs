using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace uTools {
	
	public class TweenColor : Tween<Color> {

        public bool includeChildren = false;
        public bool ignoreAlpha = false;

        Graphic[] mGraphics;
        Color mColor = Color.white;
        public override Color value
        {
            get
            {
                return mColor;
            }
            set
            {
                SetColor(transform, value);
                mColor = value;
            }
        }

        Graphic[] Graphics
        {
            get
            {
                if (mGraphics == null)
                {
                    mGraphics = includeChildren ? gameObject.GetComponentsInChildren<Graphic>() : gameObject.GetComponents<Graphic>();
                }
                return mGraphics;
            }
        }
        protected override void Start()
        {
            base.Start();
        }

        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = Color.Lerp(from, to, factor);
        }

        void SetColor(Transform _transform, Color _color)
        {
            foreach (var item in Graphics)
            {
                if (ignoreAlpha)
                {
                    _color.a = item.color.a;
                    item.color = _color;
                }
                else
                {
                    item.color = _color;
                }
                
            }
        }

        public static TweenColor Begin(GameObject go, Color from, Color to, float duration, float delay)
        {
            TweenColor comp = Tweener.Begin<TweenColor>(go, duration);
            comp.value = from;
            comp.from = from;
            comp.to = to;
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
