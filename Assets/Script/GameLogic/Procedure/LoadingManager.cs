using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingManager : Singleton<LoadingManager>, IManagerBase
{
    static string[] MainWindowSubGroup = new string[] { "shop", "home" };

    /// <summary>
    /// 同一时间只能有一个切换过程
    /// </summary>
    public bool IsSwitching { get; private set; }
    ISwitchTarget mPreTarget = null;
    DefaultSwitch mDefaultSwitch = new DefaultSwitch();

    FixedList<ISwitchTarget> mFallbackList = new FixedList<ISwitchTarget>(2);
    public void Initial()
    {
        
    }

    public void Release()
    {
        
    }

    public bool SwitchTo(ISwitch switchProc, ISwitchTarget target, System.Action onFadeInFunc)
    {
        if (IsSwitching) return false;
        GameObject loading = null;
        var preTarget = mPreTarget;
        System.Func<LoadingWindow.LoadingState, WaitForMultiObjects.WaitReturn> f = (LoadingWindow.LoadingState state) =>
        {
            switch (state)
            {
                case LoadingWindow.LoadingState.BeginFadeIn:
                    return switchProc.OnBeginFadeIn(preTarget, target);
                case LoadingWindow.LoadingState.EndFadeOut:
                    Destroy(loading);
                    loading = null;
                    IsSwitching = false;
                    return switchProc.OnEndFadeOut(preTarget, target);
                case LoadingWindow.LoadingState.EndFadeIn:
                    {
                        return switchProc.OnEndFadeIn(preTarget, target, onFadeInFunc);
                    }
                case LoadingWindow.LoadingState.BeginFadeOut:
                    {
                        return switchProc.OnBeginFadeOut(preTarget, target);
                    }
            }
            return WaitForMultiObjects.WaitReturn.Continue;
        };
        loading = WindowManager.GetSingleton().TopWindowStack.CreateWindow("LoadingWindow", "global", f, switchProc.MinLoadingTime());

        mPreTarget = target;
        if (target.Type == TargetType.MainUI)
        {
            mFallbackList.Add(target);
        }
        return true;
    }
    public bool SwitchTo(ISwitchTarget target, System.Action onFadeInFunc = null)
    {
        return SwitchTo(mDefaultSwitch, target, onFadeInFunc);
    }

    public bool SwitchToScene(int sceneId, System.Action onFadeInFunc = null)
    {
        SceneTarget st = new SceneTarget(sceneId);
        return SwitchTo(st, onFadeInFunc);
    }

    public bool SwitchToWindow(string windowName, string groupName, string[] subGroupName, params object[] param)
    {
        return SwitchToWindow(windowName, groupName, subGroupName, null, param);
    }
    public bool SwitchToWindow(string windowName, string groupName, string[] subGroupName, System.Action onFadeInFunc, params object[] param)
    {
        WindowTarget wt = new WindowTarget(windowName, groupName, subGroupName, param);
        return SwitchTo(wt, onFadeInFunc);
    }

    public bool SwitchToSceneSelector(System.Action onFadeInFunc = null, bool useUnlock = false, int targetSceneId = 0)
    {
        if (!useUnlock)
        {
            targetSceneId = Myself.GetSingleton().PlayerData.lastSceneId;
        }
        if (mFallbackList.Count > 0)
        {
            ISwitchTarget back = mFallbackList.Get();

            back.SetParam(targetSceneId, useUnlock);
            return SwitchTo(back, onFadeInFunc);
        }
        return SwitchToMainWindow(onFadeInFunc, useUnlock, targetSceneId);
    }
    public bool SwitchToMainWindow(System.Action onFadeInFunc = null, bool useUnlock = false, int targetSceneId = 0,int playermakerbackId = 0)
    {
        if (!useUnlock)
        {
            targetSceneId = Myself.GetSingleton().PlayerData.lastSceneId;
        }
        if (playermakerbackId != 0)
        {
            targetSceneId = playermakerbackId;
        }
        WindowTarget wt = new WindowTarget("MainWindow", "sceneselector", MainWindowSubGroup, targetSceneId, useUnlock);
        wt.Type = TargetType.MainUI;
        HomeSceneTarget hst = new HomeSceneTarget(1);
        MultiTarget mt = new MultiTarget(wt, hst);
        return SwitchTo(mt, onFadeInFunc);
    }
    public bool SwitchToPlayerSceneWindow(System.Action onFadeInFunc = null, int targetSceneId = 0)
    {
        WindowTarget t = new WindowTarget("PlayerMakerWindow", "playermaker", null, targetSceneId);
        t.Type = TargetType.MainUI;
        return SwitchTo(t, onFadeInFunc);
    }
    public bool SwitchToLogout()
    {
        LogoutTarget target = new LogoutTarget();
        return SwitchTo(target, null);
    }
}
