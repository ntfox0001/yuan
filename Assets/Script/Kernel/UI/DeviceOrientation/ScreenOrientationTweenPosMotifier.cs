using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenOrientationTweenPosMotifier : MonoBehaviour, IDeviceOrientationHandler
{
    [System.Serializable]
    public class TweenPosValue
    {
        public Vector3 from;
        public Vector3 to;
    }

    public TweenPosValue Portrait;
    public TweenPosValue Landscape;
	
	void Awake ()
    {
        DeviceOrientation.GetSingleton().Attach(this);
	}
    private void Start()
    {
        
    }
    public void OnDeviceOrientation(ScreenOrientation ori)
    {
        var tp = GetComponent<uTools.TweenPosition>();
        if (ori == ScreenOrientation.Portrait)
        {
            tp.from = Portrait.from;
            tp.to = Portrait.to;
        }
        else
        {
            tp.from = Landscape.from;
            tp.to = Landscape.to;
        }
    }
    private void OnDestroy()
    {
        DeviceOrientation.GetSingleton().Deattach(this);
    }
}
