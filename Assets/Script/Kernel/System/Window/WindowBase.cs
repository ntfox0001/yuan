using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public abstract class WindowBase : ResourceController
{
    public delegate void OnDestroyWindowHandler(GameObject obj);
    public event OnDestroyWindowHandler DestroyWindow;

    WindowStack mWindowStack = null;
    public WindowStack WindowStack
    {
        set
        {
            if (mWindowStack == null)
            {
                mWindowStack = value;
            }
        }
        get
        {
            return mWindowStack;
        }
    }
    string mGroupName;
    public SoundHelper SoundHelper{ get; private set; }
    public string GroupName
    {
        set {
            mGroupName = value;
            SoundHelper = SoundManager.GetSingleton().UI.GetSoundHelper(mGroupName);
        }
        get { return mGroupName; }
    }
    /// <summary>
    ///  子资源组名称
    ///  可以有多个，在窗口退出时卸载
    /// </summary>
    public string[] SubGroupName{ get; set; }
    /// <summary>
    ///  是否是全屏窗口，对应的是之前的一级窗口
    /// </summary>
    public bool IsFullScreenWindow;
    /// <summary>
    /// 是否是屏幕常驻ui，这个标记决定是否收到返回键（安卓）
    /// </summary>
    public bool IsHUD;
    public virtual void OnCreated(params object[] param)
    {

    }
    public virtual void OnClosed()
    {
    }
    public virtual void OnAppearance()
    {

    }
    public virtual void OnDisappearance()
    {

    }
    public virtual void OnBackClick()
    {
        if (!IsHUD)
        {
            Destroy(gameObject);
        }
    }
    void OnDestroy()
    {
        if (DestroyWindow != null)
        {
            DestroyWindow(gameObject);
        }
        ReleaseResource();
    }

    // 创建子ui，子ui只能在窗体所在的group中创建，不能跨group创建
    public GameObject CreateScriptUI(string uiName, GameObject parent, params object[] paramsList)
    {
        GameObject go = CreateUI(uiName, parent);
        ScriptBind sb = go.GetComponent<ScriptBind>();
        if (sb != null)
        {
            sb.OnCreated(paramsList);
        }
        return go;
    }

    public GameObject CreateUI(string uiName, GameObject parent)
    {
        return CreateUI(uiName, parent, mGroupName);
    }
    public static GameObject CreateUI(string uiName, GameObject parent, string groupName)
    {
        GameObject go = CreateUI(uiName, groupName);
        if (parent != null)
        {
            go.transform.SetParent(parent.transform);
        }
        UIUtility.SetZero(go);
        return go;
    }
    public GameObject CreateUI(string uiName)
    {
        return CreateUI(uiName, mGroupName);
    }

    public static GameObject CreateUI(string uiName, string groupName)
    {
        GameObject go = GameObject.Instantiate(ResourceManager.GetSingleton().CreateResource<GameObject>(uiName, groupName), Vector3.zero, Quaternion.identity);
        return go;
    }

    
}
