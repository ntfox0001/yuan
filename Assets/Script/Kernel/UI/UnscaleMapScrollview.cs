using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnscaleMapScrollview : MonoBehaviour
{
    public MapScrollView ParentMapScrollView;
    public bool Always = true;
	// Use this for initialization
	
    private void OnTransformParentChanged()
    {
        if (ParentMapScrollView == null)
        {
            ParentMapScrollView = GetComponentInParent<MapScrollView>();
        }
        Unscale();
    }
    
    // Update is called once per frame
    void Update () {
		if (Always)
        {
            Unscale();
        }
	}

    void Unscale()
    {
        if (ParentMapScrollView != null && ParentMapScrollView.content != null)
        {
            Vector3 scale = ParentMapScrollView.content.localScale;
            scale.x = 1.0f / scale.x;
            scale.y = 1.0f / scale.y;
            transform.localScale = scale;
        }
    }
}
