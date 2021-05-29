using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClickEffectEx : MonoBehaviour
{
    public uTools.TweenScale mTweenScale;
    
    private void Start()
    {
        if (mTweenScale == null)
        {
            mTweenScale = GetComponent<uTools.TweenScale>();
        }

        mTweenScale.enabled = false;
        
        DynamicEventGameObject.Get(gameObject).MouseDown += ButtonClickEffect_MouseDown;
        DynamicEventGameObject.Get(gameObject).MouseUp += ButtonClickEffect_MouseUp;
    }

    private void ButtonClickEffect_MouseUp(UnityEngine.EventSystems.PointerEventData eventData)
    {
        mTweenScale.PlayReverse();
    }

    private void ButtonClickEffect_MouseDown(UnityEngine.EventSystems.PointerEventData eventData)
    {
        mTweenScale.PlayForward();
    }
}
