using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
public class UIUtility
{
    public static float DeviceHVRate = 0.6f;
    static CanvasScaler sUIRootCanvasScaler = null;
    static float sPixelDragThreshold = -1.0f;
    static float sSqrPixelDragThreshold = -1.0f;
    static Vector3 sScaleZero = new Vector3(0, 0, 1);

    static public Vector3 ScaleZero
    {
        get { return sScaleZero; }
    }
    /// <summary>
    /// 这个真的能节省不少
    /// </summary>
    static public float SqrPixelDragThreshold
    {
        get
        {
            if (sSqrPixelDragThreshold < 0)
            {
                sSqrPixelDragThreshold = PixelDragThreshold * PixelDragThreshold;
            }
            return sSqrPixelDragThreshold;
        }
    }
    /// <summary>
    /// 减少.调用，传说中大量调用能节省一点
    /// </summary>
    static public float PixelDragThreshold
    {
        get
        {
            if (sPixelDragThreshold < 0)
            {
                sPixelDragThreshold = GlobalObjects.GetSingleton().EventySystem.pixelDragThreshold;
            }
            return sPixelDragThreshold;
        }
    }
    static public void SetZero(GameObject go)
    {
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
    }
    static public void SetPosZero(GameObject go)
    {
        go.transform.localPosition = Vector3.zero;

    }
    static public GameObject NewGameObject(string name)
    {
        GameObject go = new GameObject(name);
        SetZero(go);
        return go;
    }
    static public GameObject NewRectGameObject(string name)
    {
        GameObject go = NewGameObject(name);
        go.AddComponent<RectTransform>();
        return go;
    }
    static public void SetFullRectTransform(RectTransform trans)
    {
        trans.anchorMin = Vector2.zero;
        trans.anchorMax = Vector2.one;
        trans.pivot = new Vector2(0.5f, 0.5f);
        trans.offsetMax = new Vector2(0, 0);
        trans.offsetMin = new Vector2(0, 0);
        trans.localScale = Vector3.one;
    }
    static public bool IsPad
    {
        get
        {
            // 只计算横屏比
            float y = Screen.width > Screen.height ? Screen.width : Screen.height;
            float x = Screen.width < Screen.height ? Screen.width : Screen.height;
            return (x / y > DeviceHVRate);
        }
    }


    static public CanvasScaler UIRootCanvasScaler
    {
        get
        {
            if (sUIRootCanvasScaler == null)
            {
                sUIRootCanvasScaler = GlobalObjects.GetSingleton().UIRoot.GetComponent<CanvasScaler>();
            }
            return sUIRootCanvasScaler;
        }
    }
    static public float UIReferenceWidth
    {
        get
        {
            if (UIRootCanvasScaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
            {
                return UIRootCanvasScaler.referenceResolution.x;
            }
            else
            {
                return Screen.width / UIRootCanvasScaler.scaleFactor;
            }
        }
    }
    static public float UIReferenceHeight
    {
        get
        {
            if (UIRootCanvasScaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
            {
                return UIRootCanvasScaler.referenceResolution.y;
            }
            else
            {
                return Screen.height / UIRootCanvasScaler.scaleFactor;
            }
        }
    }
    static public string GetPath(Transform trans)
    {
        string path = trans.name;
        if (trans.parent != null)
        {
            path = GetPath(trans.parent) + "/" + path;
        }
        return path;
    }
    static public bool ScreenProtrait
    {
        get
        {
            return (DeviceOrientation.ScreenOrientation == ScreenOrientation.Portrait);
        }
    }

    static public bool GetAndCreateChild<T>(GameObject parent, string name) where T : Component
    {
        Transform trans = parent.transform.Find(name);
        if (trans == null)
        {
            GameObject go = new GameObject(name);
            go.AddComponent<T>();
            go.transform.SetParent(parent.transform);
            SetZero(go);

            return go;
        }
        return trans.gameObject;
    }

    /// <summary>
    /// 创建一个资源组中的gameObject资源，资源组必须已经加载，否则返回失败
    /// </summary>
    /// <param name="res"></param>
    /// <param name="group">不需要ui前缀</param>
    /// <returns></returns>
    static public GameObject CreateGameObject(string res, string group)
    {
        return UnityEngine.Object.Instantiate(ResourceManager.GetSingleton().CreateResource<GameObject>(res, WindowManager.WindowGroupPrefix + group));
    }
    /// <summary>
    /// 创建sprite
    /// </summary>
    /// <param name="res"></param>
    /// <param name="group">必须是ui组下的，不需要ui前缀</param>
    /// <returns></returns>
    static public Sprite CreateSprite(string res, string group)
    {
        return CreateSpriteByRawGroup(res, WindowManager.WindowGroupPrefix + group);
    }
    /// <summary>
    /// 创建sprite
    /// </summary>
    /// <param name="res"></param>
    /// <param name="group">需要前缀</param>
    /// <returns></returns>
    static public Sprite CreateSpriteByRawGroup(string res, string group)
    {
        return ResourceManager.GetSingleton().CreateResource<Sprite>(res, group);
    }
    static public GameObject BlockInput(Transform parent)
    {
        var go = new GameObject("BlockInput", typeof(RectTransform), typeof(ClickArea), typeof(DynamicEventGameObject));
        go.transform.SetParent(parent);
        SetFullRectTransform(go.transform as RectTransform);
        return go;
    }
    static public void FadeMaskUI(FadeMaskUI.Fade fade, Transform parent, System.Action willFadeOut, System.Action finished)
    {
        var fadeui = CreateGameObject("FadeMask", "global");
        fadeui.transform.SetParent(parent);
        SetFullRectTransform(fadeui.transform as RectTransform);
        fadeui.GetComponent<FadeMaskUI>().Play(fade, willFadeOut, finished);
    }
    static public void FadeMaskUIOnTop(FadeMaskUI.Fade fade, System.Action willFadeOut, System.Action finished)
    {
        var emptyWin = WindowManager.GetSingleton().TopWindowStack.CreateWindow("EmptyWindow", "global");
        System.Action func = () =>
        {
            GameObject.Destroy(emptyWin);
        };
        func += finished;
        FadeMaskUI(fade, emptyWin.transform, willFadeOut, func);
    }
    static public void SetBgFullScreen(RectTransform rt)
    {
        if (rt != null)
        {
            float dim = UIReferenceHeight > UIReferenceWidth ? UIReferenceHeight : UIReferenceWidth;
            Vector2 size = new Vector2(dim, dim);
            rt.sizeDelta = size;
        }
    }
}
