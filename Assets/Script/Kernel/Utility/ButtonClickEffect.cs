using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClickEffect : MonoBehaviour
{
    public float Scale = 0.9f;

    Vector3 mPreScale;
    private void Start()
    {
        DynamicEventGameObject.Get(gameObject).MouseDown += ButtonClickEffect_MouseDown;
        DynamicEventGameObject.Get(gameObject).MouseUp += ButtonClickEffect_MouseUp;
    }

    private void ButtonClickEffect_MouseUp(UnityEngine.EventSystems.PointerEventData eventData)
    {
        transform.localScale = mPreScale;
    }

    private void ButtonClickEffect_MouseDown(UnityEngine.EventSystems.PointerEventData eventData)
    {

        mPreScale = transform.localScale;
        transform.localScale = transform.localScale * Scale;
    }
}
