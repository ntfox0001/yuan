using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DeviceOrientation : Singleton<DeviceOrientation>
{
    public bool RunInPad;
    public bool RunInPhone;
    // This event will only be called when an orientation changed (i.e. won't be call at lanch)
    event UnityAction<ScreenOrientation> OrientationChangedEvent;
    

    private ScreenOrientation _orientation;
    /// <summary>
    /// 游戏默认方向，不同游戏要修改
    /// </summary>
    static public ScreenOrientation DefaultOrientation { get { return ScreenOrientation.Portrait; } }

    /// <summary>
    /// 默认设备是手机
    /// </summary>
    static public bool DefaultDeviceIsPad { get { return false; } }
#if UNITY_EDITOR

    static ScreenOrientation sCurrentOrientation = DefaultOrientation;
    static public ScreenOrientation ScreenOrientation
    {
        get
        {
            return sCurrentOrientation;
        }
    }
#else
    static public ScreenOrientation ScreenOrientation
    {
        get
        {
            return Screen.orientation;
        }
    }
#endif
    void Awake()
    {
        _orientation = ScreenOrientation;
    }
    /// <summary>
    /// 加入设备旋转监听，要在初始化完成后调用，否则可能会导致设备旋转调用和初始化冲突
    /// </summary>
    /// <param name="handler"></param>
    public void Attach(IDeviceOrientationHandler handler)
    {
        if (RunInPad && UIUtility.IsPad || RunInPhone && !UIUtility.IsPad)
        {
            OrientationChangedEvent += handler.OnDeviceOrientation;
            // 如果当前环境和设置不一样，那么触发通知
            if (ScreenOrientation != DefaultOrientation && DefaultDeviceIsPad != UIUtility.IsPad)
            {
                handler.OnDeviceOrientation(ScreenOrientation);
            }
        }
    }
    public void Deattach(IDeviceOrientationHandler handler)
    {
        OrientationChangedEvent -= handler.OnDeviceOrientation;
    }
    private void OnOrientationChanged(ScreenOrientation orientation)
    {
        if (OrientationChangedEvent != null)
            OrientationChangedEvent(orientation);
    }

    private void Update()
    {
        if (RunInPad && UIUtility.IsPad || RunInPhone && !UIUtility.IsPad)
        {
            if (_orientation != ScreenOrientation)
            {
                _orientation = ScreenOrientation;
                OnOrientationChanged(_orientation);
            }
        }
    }



#if UNITY_EDITOR
    [UnityEditor.MenuItem("Debug/Orientation/Simulate Landscape")]
    private static void DoSetLandscapeLeft()
    {
        sCurrentOrientation = ScreenOrientation.LandscapeLeft;
    }

    [UnityEditor.MenuItem("Debug/Orientation/Simulate Portrait")]
    private static void DoSetPortrait()
    {
        sCurrentOrientation = ScreenOrientation.Portrait;
    }

#endif

}