using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class WindowManager : Singleton<WindowManager>, IManagerBase
{
    public const string WindowGroupPrefix = "ui_";
    public const string GlobalGroupName = "ui_global";
    Dictionary<Int32, WindowStack> mWindowStackDict = new Dictionary<Int32, WindowStack>();

    WindowStack.OnPrimitiveWindowCloseHandler mOnPrimitiveWindowCloseHandler;
    WindowStack.OnCreateWindowHandler mOnCreateWindowHandler;
    int mWindowStackCount = 2;
    //--------------------------------------------------------------------------------------------------------------------------------------------
    public delegate void OnCreateWindowHandler(GameObject window, WindowStack ws);
    public event OnCreateWindowHandler CreateWindowEvent;
    public delegate void OnCreateWindowStackHandler(WindowStack ws);
    public event OnCreateWindowStackHandler CreateWindowStackEvent;

    public WindowStack ActiveWindowStack { get; private set; }
    public WindowStack TopWindowStack { get; private set; }
    public WindowStack BottomWindowStack { get; private set; }

    public RectTransform WindowRoot;
    public RectTransform TopWindowRoot;
    public RectTransform BottomWindowRoot;

    public void Initial()
    {
        mOnPrimitiveWindowCloseHandler = new WindowStack.OnPrimitiveWindowCloseHandler(OnPrimitiveWindowClose);
        mOnCreateWindowHandler = new WindowStack.OnCreateWindowHandler(OnCreateWindow);

        TopWindowStack = NewWindowStack("TopWindowStack", TopWindowRoot);
        BottomWindowStack = NewWindowStack("BottomWindowStack", BottomWindowRoot);
    }

    public void Release()
    {
        CloseAllWindowStack();
    }
    void Update()
    {
#if !UNITY_IOS
        // 检车安卓返回键和桌面系统的esc键
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            WindowBase win = null;
            while (true)
            {
                win = GetDialogWindow(TopWindowStack);
                if (win != null)
                {
                    break;
                }

                win = GetDialogWindow(ActiveWindowStack);
                if (win != null)
                {
                    break;
                }

                win = GetDialogWindow(BottomWindowStack);

                break;
            } 

            if (win != null)
            {
                win.OnBackClick();
            }
        }
#endif
    }
    /// <summary>
    /// 获得给stack里最上层的dialog（非hud）窗口
    /// </summary>
    /// <returns></returns>
    WindowBase GetDialogWindow(WindowStack stack)
    {
        for (int i = stack.Stack.Count - 1; i >= 0 ; i--)
        {
            if (!stack.Stack[i].mWindowInstantiate.IsHUD)
            {
                return stack.Stack[i].mWindowInstantiate;
            }
        }
        return null;
    }
    /// <summary>
    /// 加载全局ui
    /// 要在网络下载完成后调用
    /// </summary>
    public WaitForProgressCallback<string> LoadGlobalUI()
    {
        return AsyncLoadUIGroup("global");
    }
    /// <summary>
    /// 异步加载指定UI资源组，需要手动释放
    /// </summary>
    /// <param name="groupName"></param>
    /// <returns></returns>
    public WaitForProgressCallback<string> AsyncLoadUIGroup(string groupName)
    {
        groupName = WindowGroupPrefix + groupName;
        WaitForProgressCallback<string> uiWait = new WaitForProgressCallback<string>(groupName);
        ResourceManager.GetSingleton().AssetBundleLoadHelper.LoadAssetBundleAsync(groupName, uiWait.ProgressCallback);

        return uiWait;
    }
    public void UnloadUIGroup(string groupName)
    {
        ResourceManager.GetSingleton().AssetBundleLoadHelper.ReleaseAssetBundle(groupName);
    }
    void OnCreateWindow(GameObject window, WindowStack ws)
    {
        if (CreateWindowEvent != null)
        {
            CreateWindowEvent(window, ws);
        }
    }
    /// <summary>
    /// 创建普通Stack
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    WindowStack NewWindowStack(string name)
    {
        return NewWindowStack(name, WindowRoot);
    }
    /// <summary>
    /// 在指定的位置创建Stack
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    WindowStack NewWindowStack(string name, Transform parent)
    {
        GameObject go = UIUtility.NewRectGameObject(name);
        go.transform.SetParent(parent);
        UIUtility.SetFullRectTransform(go.GetComponent<RectTransform>());
        //UIUtility.SetZero(go);
        WindowStack ws = go.AddComponent<WindowStack>();
        ws.Initial(mWindowStackCount);
        mWindowStackCount++;
        return ws;
    }
    public WindowStack CreateWindowStack(bool delPrevious = true)
    {
        WindowStack ws = NewWindowStack("WindowStack" + mWindowStackCount);
        
        mWindowStackDict.Add(ws.Id, ws);
        
        ws.PrimitiveWindowCloseEvent += mOnPrimitiveWindowCloseHandler;
        ws.CreateWindowEvent += mOnCreateWindowHandler;

        if (CreateWindowStackEvent != null)
        {
            CreateWindowStackEvent(ws);
        }

        if (ActiveWindowStack == null)
        {
            ActiveWindowStack = ws;
        }
        else
        {
            if (delPrevious)
                DestroyWindowStack(ActiveWindowStack.Id);

            ActiveWindowStack = ws;
        }
        return ws;
    }
    public void SetActiveWindowStack(Int32 id)
    {
        if (ActiveWindowStack != null)
        {
            ActiveWindowStack.Show(false);
        }
        WindowStack ws;
        if (mWindowStackDict.TryGetValue(id, out ws))
        {
            ActiveWindowStack = ws;
            ActiveWindowStack.Show(true);
        }
    }
    public void DestroyWindowStack(Int32 id)
    {
        WindowStack ws;
        if (mWindowStackDict.TryGetValue(id, out ws))
        {
            ws.Release();
            mWindowStackDict.Remove(ws.Id);
        }

        if (ws == ActiveWindowStack)
        {
            ActiveWindowStack = null;
        }
    }
    public void CloseAllWindowStack()
    {
        foreach (KeyValuePair<Int32, WindowStack> iter in mWindowStackDict)
        {
            iter.Value.Release();
        }
        mWindowStackDict.Clear();
        ActiveWindowStack = null;
    }
    void OnPrimitiveWindowClose(WindowStack ws)
    {
        
    }
    /// <summary>
    /// 延迟销毁窗口，一般用于渐隐效果
    /// </summary>
    /// <param name="window">要销毁的窗口</param>
    /// <param name="delay">延时</param>
    public void DelayDestroyWindow(WindowBase window, float delay)
    {
        StartCoroutine(DelayDestroyWindowCallback(window, delay));
    }
    IEnumerator DelayDestroyWindowCallback(WindowBase window, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(window.gameObject);
    }
    
}
