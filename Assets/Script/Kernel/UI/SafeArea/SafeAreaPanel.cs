using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 设计安全区域面板（适配iPhone X）
/// Jeff 2017-12-1
/// 文件名 SafeAreaPanel.cs
/// </summary>
public class SafeAreaPanel : MonoBehaviour, IDeviceOrientationHandler
{
    public bool Left = true;
    public bool Right = true;
    public bool Top = true;
    public bool Bottom = true;
    private RectTransform target;
    Vector2 oriTargetMin;
    Vector2 oriTargetMax;
    bool ignoreRectChange = false;
    bool mInitialized = false;
#if UNITY_EDITOR
    [SerializeField]
    private bool Simulate_X = false;
#endif


    void Awake()
    {
        // 如果这个节点是window，那么等待window初始化完成，调用InitialByWindow
        if (GetComponent<WindowBase>() != null) return;

        Initial();
    }
    void Initial()
    {
        if (mInitialized) return;
        target = GetComponent<RectTransform>();
        oriTargetMin = target.anchorMin;
        oriTargetMax = target.anchorMax;

        mInitialized = true;

        ApplySafeArea();
        DeviceOrientation.GetSingleton().Attach(this);
    }
    /// <summary>
    /// 通过window调用的初始化
    /// </summary>
    void InitialByWindow()
    {
        Initial();
    }
    void ApplySafeArea()
    {
        if (!mInitialized || ignoreRectChange) return;

        ignoreRectChange = true;
        var area = Screen.safeArea;// SafeAreaUtils.Get();

#if UNITY_EDITOR

        /*
        iPhone X 横持手机方向:
        iPhone X 分辨率
        2436 x 1125 px

        safe area
        2172 x 1062 px

        左右边距分别
        132px

        底边距 (有Home条)
        63px

        顶边距
        0px
        */

        if (Simulate_X)
        {
            area = new Rect(60, 60, Screen.width - 120, Screen.height - 120);
        }
#endif

        Vector2 targetMin = oriTargetMin, targetMax = oriTargetMax;
        
        var anchorMin = area.position;
        var anchorMax = area.position + area.size;
        // 只有锚点在0和1的位置上才设置
        
        anchorMin.x = Left ? anchorMin.x / Screen.width : targetMin.x;
        anchorMin.y = Bottom ? anchorMin.y / Screen.height : targetMin.y;
        anchorMax.x = Right ? anchorMax.x / Screen.width : targetMax.x;
        anchorMax.y = Top ? anchorMax.y / Screen.height : targetMax.y;
        float minx = UpdateAnchor(anchorMin.x, anchorMax.x, targetMin.x);
        float maxy = UpdateAnchor(anchorMin.y, anchorMax.y, targetMax.y);

        // 窗口左上向右下移动
        Vector2 minx_maxy = new Vector2(minx, maxy);
        targetMin += minx_maxy;
        targetMax += minx_maxy;

        float maxx = UpdateAnchor(anchorMin.x, anchorMax.x, targetMax.x);
        float miny = UpdateAnchor(anchorMin.y, anchorMax.y, targetMin.y);
        targetMin += new Vector2(0, miny);
        targetMax += new Vector2(maxx, 0);

        target.anchorMin = targetMin;
        target.anchorMax = targetMax;

        ignoreRectChange = false;
    }

    float UpdateAnchor(float min, float max, float v)
    {
        if (v < min)
        {
            return min - v;
        }
        else if (v > max)
        {
            return max - v;
        }else
        {
            return 0;
        }
    }

    public void OnDeviceOrientation(ScreenOrientation ori)
    {
        ApplySafeArea();
    }
    private void OnDestroy()
    {
        DeviceOrientation.GetSingleton().Deattach(this);
    }
}