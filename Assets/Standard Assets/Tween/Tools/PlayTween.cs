using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace uTools
{

    public class PlayTween : MonoBehaviour, IPointHandler
    {
        public GameObject tweenTarget;
        public Direction playDirection = Direction.Forward;
        public Trigger trigger = Trigger.OnPointerClick;
        public int tweenGroup = 0;
        public bool inCludeChildren = false;
        public bool ResetToBeginning = false;
        private Tweener[] mTweeners;
        GameObject TweenTarget
        {
            get
            {
                if (tweenTarget == null)
                {
                    tweenTarget = gameObject;
                }
                return tweenTarget;
            }
        }
        Tweener[] Tweeners
        {
            get
            {
                if (mTweeners == null)
                {
                    mTweeners = inCludeChildren ? TweenTarget.GetComponentsInChildren<Tweener>() : TweenTarget.GetComponents<Tweener>();
                }
                return mTweeners;
            }
        }
        void Awake()
        {
            if (tweenTarget == null)
            {
                tweenTarget = gameObject;
            }
            if (mTweeners == null)
            {
                mTweeners = inCludeChildren ? TweenTarget.GetComponentsInChildren<Tweener>() : TweenTarget.GetComponents<Tweener>();
            }
        }
        void Start()
        {
            TriggerPlay(Trigger.OnStart);
        }
        void OnEnable()
        {
            TriggerPlay(Trigger.OnEnable);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            TriggerPlay(Trigger.OnPointerEnter);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            TriggerPlay(Trigger.OnPointerDown);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            TriggerPlay(Trigger.OnPointerClick);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            TriggerPlay(Trigger.OnPointerUp);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TriggerPlay(Trigger.OnPointerExit);
        }

        private void TriggerPlay(Trigger _trigger)
        {
            if (_trigger == trigger)
            {
                Play();
            }
        }

        /// <summary>
        /// Play this instance.
        /// </summary>
        public  void Play()
        {
            if (playDirection == Direction.Toggle)
            {
                foreach (var item in Tweeners)
                {
                    if (item.tweenGroup == tweenGroup)
                    {
                        item.Toggle();
                    }
                }
            }
            else
            {
                foreach (var item in Tweeners)
                {
                    if (item.tweenGroup == tweenGroup)
                    {
                        if (ResetToBeginning)
                        {
                            item.ResetToBeginning();
                        }

                        item.Play(playDirection == Direction.Forward);
                    }
                }
            }
        }

        public void Reset()
        {
            foreach (var item in Tweeners)
            {
                if (item.tweenGroup == tweenGroup)
                {
                    item.ResetToBeginning();
                    
                }
            }
        }
    }
}