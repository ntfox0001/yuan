using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace uTools
{
    [RequireComponent(typeof(Image))]
    public class TweenSprite : Tween<Sprite>
    {
        Image mCacheImage;
        Image CacheImage
        {
            get
            {
                if (mCacheImage == null)
                {
                    mCacheImage = GetComponent<Image>();
                }
                return mCacheImage;
            }
        }
        public override Sprite value
        {
            get
            {
                return CacheImage.sprite;
            }
            set
            {
                CacheImage.sprite = value;
            }
        }

        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = factor > 0.5 ? to : from;
        }

        public static TweenPosition Begin(GameObject go, Vector3 from, Vector3 to, float duration = 1f, float delay = 0f)
        {
            TweenPosition comp = Tweener.Begin<TweenPosition>(go, duration);
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
