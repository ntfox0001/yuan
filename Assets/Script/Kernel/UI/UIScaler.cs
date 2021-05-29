using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScaler : MonoBehaviour
{
    public float ScaleBaseWidth = 1080.0f;
    public float ScaleBaseHeight = 1920.0f;

    Vector3 mOriScale;
	// Use this for initialization
	protected virtual void Start () {
        mOriScale = transform.localScale;
        Scale();
	}
	
    protected virtual void Scale()
    {
        float scale = (UIUtility.UIReferenceWidth + UIUtility.UIReferenceHeight) / (ScaleBaseWidth + ScaleBaseHeight);
        transform.localScale = mOriScale * scale;
    }

}
