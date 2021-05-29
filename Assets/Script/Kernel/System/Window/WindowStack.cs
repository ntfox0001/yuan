using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class WindowStack : MonoBehaviour
{
    public delegate void OnPrimitiveWindowCloseHandler(WindowStack ws);
    public event OnPrimitiveWindowCloseHandler PrimitiveWindowCloseEvent;
    public delegate void OnCreateWindowHandler(GameObject window, WindowStack ws);
    public event OnCreateWindowHandler CreateWindowEvent;
    public delegate void OnWindowCloseHandler(GameObject window, WindowStack ws);
    public event OnWindowCloseHandler WindowCloseEvent;
    public delegate void OnReleaseWindowStackHandler(WindowStack ws);
    public event OnReleaseWindowStackHandler ReleaseWindowStackEvent;
    public class WindowInfo
    {
        public WindowBase mWindowInstantiate = null;
        public string mWindowName = "";
        public GameObject mWindowPrefab = null;
        public bool mIsOutBuild = false;
    }
    /// <summary>
    /// 返回窗口信息堆栈
    /// </summary>
    public List<WindowInfo> Stack { get { return mStack; } }
    List<WindowInfo> mStack = new List<WindowInfo>();
    WindowBase.OnDestroyWindowHandler mDestroyWindowHandler;
    bool mDestroyStack = false;
    int mCoefficient = 59;

    int mCurrentDepth = 0;
    bool mShow = true;

    public int Coefficient
    {
        get { return mCoefficient; }
    }
    public int Id { get; private set; }

    public void Initial(int id)
    {
        Id = id;
        mDestroyWindowHandler = new WindowBase.OnDestroyWindowHandler(OnDestroyWindow);
    }
    /// <summary>
    /// 创建窗口
    /// </summary>
    /// <param name="windowName">大小写敏感，不带目录</param>
    /// <param name="groupName">不带前缀的资源组名，小写</param>
    /// <param name="paramsList"></param>
    /// <returns></returns>
    public GameObject CreateWindow(string windowName, string groupName, params object[] paramsList)
    {
        // 读取group资源，window销毁时释放资源
        groupName = WindowManager.WindowGroupPrefix + groupName.ToLower();
        ResourceManager.GetSingleton().AssetBundleLoadHelper.LoadAssetBundle(groupName);
        WindowInfo wi = new WindowInfo();
        mStack.Add(wi);

        GameObject windowGO = GameObject.Instantiate(ResourceManager.GetSingleton().CreateResource<GameObject>(windowName, groupName), Vector3.zero, Quaternion.identity);
        if (windowGO == null)
        {
            Debug.LogError("WindowName is not exist: " + windowName);
            return null;
        }
        windowGO.transform.SetParent(transform);
        UIUtility.SetZero(windowGO);
        UIUtility.SetFullRectTransform(windowGO.GetComponent<RectTransform>());
        windowGO.SendMessage("InitialByWindow", SendMessageOptions.DontRequireReceiver);

        if (CreateWindowEvent != null)
        {
            CreateWindowEvent(windowGO, this);
        }

        wi.mWindowInstantiate = windowGO.GetComponent<WindowBase>();
        
        if (wi.mWindowInstantiate == null)
        {
            Debug.LogError("There is not WindowBase on Window.");
            return null;
        }

        wi.mWindowInstantiate.DestroyWindow += mDestroyWindowHandler;
        wi.mWindowInstantiate.GroupName = groupName;
        wi.mWindowInstantiate.OnCreated(paramsList);
        wi.mWindowInstantiate.WindowStack = this;
        
        wi.mWindowName = windowName;
        wi.mIsOutBuild = false;

        wi.mWindowInstantiate.OnAppearance();

        AdjustWindowAppearance();
        if (mStack.Count >= 2)
        {
            mStack[mStack.Count - 2].mWindowInstantiate.OnDisappearance();
        }
        return windowGO;
    }
    /// <summary>
    /// 创建窗口，并且加载子资源组
    /// </summary>
    /// <param name="windowName">大小写敏感，不带目录</param>
    /// <param name="groupName">不带前缀的资源组名，小写</param>
    /// <param name="subGroupName">不带前缀的资源组名，小写</param>
    /// <param name="paramsList"></param>
    /// <returns></returns>
    public GameObject CreateWindow(string windowName, string groupName, string[] subGroupName, params object[] paramsList)
    {
        if (subGroupName != null)
        {
            for (int i = 0; i < subGroupName.Length; i++)
            {
                // 读取group资源，window销毁时释放资源
                var subgname = WindowManager.WindowGroupPrefix + subGroupName[i].ToLower();
                ResourceManager.GetSingleton().AssetBundleLoadHelper.LoadAssetBundle(subgname);
            }
        }
        var winGo = CreateWindow(windowName, groupName, paramsList);
        winGo.GetComponent<WindowBase>().SubGroupName = subGroupName;
        return winGo;

    }
    // 通过已经存在的实例创建窗口
    public GameObject CreateWindow(GameObject windowGO, params object[] paramsList)
    {
        WindowInfo wi = new WindowInfo();
        windowGO.SetActive(true);

        if (CreateWindowEvent != null)
        {
            CreateWindowEvent(windowGO, this);
        }

        wi.mWindowInstantiate = windowGO.GetComponent<WindowBase>();
        if (wi.mWindowInstantiate == null)
        {
            Debug.LogError("There is not WindowBase on Window.");
            return null;
        }

        wi.mWindowInstantiate.DestroyWindow += mDestroyWindowHandler;
        wi.mWindowInstantiate.OnCreated(paramsList);
        wi.mWindowInstantiate.WindowStack = this;

        wi.mWindowName = windowGO.name;
        wi.mIsOutBuild = true;

        mStack.Add(wi);

        wi.mWindowInstantiate.OnAppearance();

        AdjustWindowAppearance();
        if (mStack.Count >= 2)
        {
            mStack[mStack.Count - 2].mWindowInstantiate.OnDisappearance();
        }
        return windowGO;
    }
    public void DestroyWindow(GameObject window)
    {
        for (int i = 0; i < mStack.Count; i++)
        {
            if (mStack[i].mWindowInstantiate == window.GetComponent<WindowBase>())
            {
                if (!mStack[i].mIsOutBuild)
                    GameObject.Destroy(mStack[i].mWindowInstantiate.gameObject);
                else
                {
                    mStack[i].mWindowInstantiate.OnDisappearance();
                    mStack[i].mWindowInstantiate.OnClosed();
                    mStack[i].mWindowInstantiate.gameObject.SetActive(false);
                    mStack[i].mWindowInstantiate.DestroyWindow -= mDestroyWindowHandler;
                    mStack.RemoveAt(i);
                    AdjustWindowAppearance();
                    if (WindowCloseEvent != null)
                    {
                        WindowCloseEvent(window, this);
                    }
                }
                break;
            }
        }
    }
    
    void OnDestroyWindow(GameObject windowGO)
    {
        if (windowGO == null)
        {
            Debug.LogError("Window GameObject is null.");
            return;
        }
        for (int i = 0; i < mStack.Count; i++)
        {
            if (mStack[i].mWindowInstantiate.gameObject == windowGO)
            {
                WindowBase win = mStack[i].mWindowInstantiate;
                win.OnClosed();
                win.DestroyWindow -= mDestroyWindowHandler;
                mStack.RemoveAt(i);
                // 释放这个窗口的资源
                ResourceManager.GetSingleton().AssetBundleLoadHelper.ReleaseAssetBundle(win.GroupName);
                if (win.SubGroupName != null)
                {
                    for (int j = 0; j < win.SubGroupName.Length; j++)
                    {
                        ResourceManager.GetSingleton().AssetBundleLoadHelper.ReleaseAssetBundle(WindowManager.WindowGroupPrefix + win.SubGroupName[j]);
                    }
                }
                if (WindowCloseEvent != null)
                {
                    WindowCloseEvent(windowGO, this);
                }

                // 当销毁的时候，不做调整操作
                if (!mDestroyStack)
                {
                    AdjustWindowAppearance();
                    if (mStack.Count >= 1 && mShow)
                        mStack[mStack.Count - 1].mWindowInstantiate.OnAppearance();
                }

                if (mStack.Count == 0)
                {
                    if (PrimitiveWindowCloseEvent != null)
                        PrimitiveWindowCloseEvent(this);
                }
                return;
            }
        }

        Debug.LogError("Window: " + windowGO.name + "is not exist.");
    }
    void AdjustWindowAppearance()
    {
        if (!mShow) return;

        if (mStack.Count > 1)
        {
            bool windowState = true;
            // 如果是全屏，那么隐藏之前的窗口
            for (int i = mStack.Count - 1; i >= 0; i--)
            {
                mStack[i].mWindowInstantiate.gameObject.SetActive(windowState);
                if (mStack[i].mWindowInstantiate.IsFullScreenWindow && windowState == true)
                {
                    windowState = false;
                }
            }
        }
        if (mStack.Count > 0)
        {
            mStack[mStack.Count - 1].mWindowInstantiate.gameObject.SetActive(true);
        }
    }

    public void Release()
    {
        mDestroyStack = true;
        for (int i = mStack.Count - 1; i >= 0; i--)
        {
            if (!mStack[i].mIsOutBuild)
                GameObject.Destroy(mStack[i].mWindowInstantiate.gameObject);
            else
            {
                mStack[i].mWindowInstantiate.OnDisappearance();
                mStack[i].mWindowInstantiate.gameObject.SetActive(false);
            }
                
        }
        if (mStack.Count == 0 && PrimitiveWindowCloseEvent != null)
        {
            PrimitiveWindowCloseEvent(this);
        }
        if (ReleaseWindowStackEvent != null)
        {
            ReleaseWindowStackEvent(this);
        }
    }

    public void Show(bool show)
    {
        mShow = show;
        for (int i = 0; i < mStack.Count; i++)
        {
            mStack[i].mWindowInstantiate.gameObject.SetActive(show);
        }
        if (mStack.Count >= 1)
        {
            if (mShow)
            {
                mStack[mStack.Count - 1].mWindowInstantiate.OnAppearance();
            }
            else
            {
                mStack[mStack.Count - 1].mWindowInstantiate.OnDisappearance();
            }
        }
            
        AdjustWindowAppearance();
    }


}
