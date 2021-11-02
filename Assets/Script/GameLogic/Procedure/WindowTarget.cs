using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowTarget : ISwitchTarget
{
    string mWindowName;
    string mGroupName;
    string[] mSubGroupName;
    object[] mParam;
    GameObject mWindow;

    public TargetType Type{ get; set; }

    public WindowTarget(string windowName, string groupName, string[] subGroupName, params object[] param)
    {
        Type = TargetType.None;
        mWindowName = windowName;
        mGroupName = groupName;
        mSubGroupName = subGroupName;
        mParam = param;
    }
    /// <summary>
    /// 重置参数
    /// </summary>
    /// <param name="param"></param>
    public void SetParam(params object[] param)
    {
        mParam = param;
    }
    public WaitForMultiObjects.WaitReturn Preload()
    {
        mWindow = WindowManager.GetSingleton().ActiveWindowStack.CreateWindow(mWindowName, mGroupName, mSubGroupName, mParam);

        return WaitForMultiObjects.WaitReturn.Continue;
    }
    public WaitForMultiObjects.WaitReturn Postload()
    {
        return WaitForMultiObjects.WaitReturn.Continue;
    }
    public void Release()
    {
        Object.Destroy(mWindow);
    }
}
