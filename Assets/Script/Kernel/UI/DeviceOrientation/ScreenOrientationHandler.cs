using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScreenOrientationHandler : MonoBehaviour, IDeviceOrientationHandler
{
    public UnityEvent OnPortrait;
    public UnityEvent OnLandscape;
    void Awake()
    {
        DeviceOrientation.GetSingleton().Attach(this);
    }
    public void OnDeviceOrientation(ScreenOrientation ori)
    {
        if (ori == ScreenOrientation.Portrait)
        {
            if (OnPortrait != null)
            {
                OnPortrait.Invoke();
            }
        }
        else
        {
            if (OnLandscape != null)
            {
                OnLandscape.Invoke();
            }
        }
    }
    private void OnDestroy()
    {
        DeviceOrientation.GetSingleton().Deattach(this);
    }

}
