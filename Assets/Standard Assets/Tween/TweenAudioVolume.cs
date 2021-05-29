using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace uTools
{

    public class TweenAudioVolume : Tween<float>
    {

        public bool includeChildren = false;

        float mVolume = 0f;

        protected override void Start()
        {
            base.Start();
            // edit by fox
            // 放到awake去
            //if (CacheCanvasGroup != null)
            //{
            //    isCanvasGroup = true;
            //}
        }
        AudioSource[] mAudioSource;
        AudioSource[] CachedAudioSources
        {
            get
            {
                if (mAudioSource == null)
                {
                    mAudioSource = includeChildren ? gameObject.GetComponentsInChildren<AudioSource>() : gameObject.GetComponents<AudioSource>();
                }
                return mAudioSource;
            }
        }
        public override float value
        {
            get
            {
                return mVolume;
            }
            set
            {
                mVolume = value;
                SetVolume(value);
            }
        }

        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = from + factor * (to - from);
        }

        void SetVolume(float _volume)
        {

            foreach (var item in CachedAudioSources)
            {
                item.volume = _volume;
            }

        }

        public static TweenAudioVolume Begin(GameObject go, float from, float to, float duration = 1f, float delay = 0f)
        {
            TweenAudioVolume comp = Begin<TweenAudioVolume>(go, duration);
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