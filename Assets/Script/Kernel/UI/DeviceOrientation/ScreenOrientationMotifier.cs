using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenOrientationMotifier : MonoBehaviour, IDeviceOrientationHandler
{
    public bool OverrideManager = false;
    public bool RunInPad = false;
    public bool RunInPhone = false;
    private ScreenOrientation _orientation;

    [System.Serializable]
    public class RectTransformRaw
    {
        public Vector2 anchoredPosition;
        public Vector2 sizeDelta;
        public Vector2 anchorMin;
        public Vector2 anchorMax;
        public Vector2 pivot;
        public Quaternion rotation;
        public RectTransformRaw(RectTransform rect)
        {
            anchoredPosition = rect.anchoredPosition;
            anchorMin = rect.anchorMin;
            anchorMax = rect.anchorMax;
            sizeDelta = rect.sizeDelta;
            pivot = rect.pivot;
            rotation = rect.localRotation;
        }
        public void Apply(RectTransform rect)
        {
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.localRotation = rotation;
        }
    }

    [SerializeField]
    public RectTransformRaw Portrait;
    [SerializeField]
    public RectTransformRaw Landscape;

    RectTransform mRect;
    // Use this for initialization
    void Awake()
    {
        
        mRect = GetComponent<RectTransform>();

        if (mRect == null)
        {
            Debug.LogError("There is not RectTransform." + UIUtility.GetPath(transform));
        }

        if (OverrideManager)
        {
            _orientation = DeviceOrientation.ScreenOrientation;
            if (RunInPad && UIUtility.IsPad || RunInPhone && !UIUtility.IsPad)
            {
                if (_orientation != DeviceOrientation.DefaultOrientation || DeviceOrientation.DefaultDeviceIsPad != UIUtility.IsPad)
                {
                    OnDeviceOrientation(_orientation);
                }
            }
        }
        else
        {
            DeviceOrientation.GetSingleton().Attach(this);
        }
    }
	
    public void OnDeviceOrientation(ScreenOrientation ori)
    {
        if (ori == ScreenOrientation.Portrait)
        {
            SetRectTransformRaw(Portrait);
        }
        else
        {
            SetRectTransformRaw(Landscape);
        }
    }

    public void SetRectTransformRaw(RectTransformRaw raw)
    {
        raw.Apply(mRect);
    }
    private void OnDestroy()
    {
        if (!OverrideManager)
        {
            DeviceOrientation.GetSingleton().Deattach(this);
        }
    }

    private void Update()
    {
        if (OverrideManager)
        {
            if (RunInPad && UIUtility.IsPad || RunInPhone && !UIUtility.IsPad)
            {
                if (_orientation != DeviceOrientation.ScreenOrientation)
                {
                    _orientation = DeviceOrientation.ScreenOrientation;
                    OnDeviceOrientation(_orientation);
                }
            }
        }
    }


}

