using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 执行多个switch target
/// </summary>
public class MultiTarget : ISwitchTarget
{
    public List<ISwitchTarget> Targets1 { get; private set; }
    public List<ISwitchTarget> Targets2 { get; private set; }
    public MultiTarget(params ISwitchTarget[] param)
    {
        Targets1 = new List<ISwitchTarget>(param);
        Targets2 = new List<ISwitchTarget>();
    }

    public TargetType Type
    {
        get
        {
            foreach (var target in Targets1)
            {
                if (target.Type == TargetType.MainUI)
                {
                    return TargetType.MainUI;
                }
            }
            foreach (var target in Targets2)
            {
                if (target.Type == TargetType.MainUI)
                {
                    return TargetType.MainUI;
                }
            }

            return TargetType.None;
        }
    }
    public void SetParam(params object[] param)
    {
        foreach (var target in Targets1)
        {
            target.SetParam(param);
        }
        foreach (var target in Targets2)
        {
            target.SetParam(param);
        }
    }
    public WaitForMultiObjects.WaitReturn Postload()
    {
        for (int i = Targets2.Count - 1; i >= 0; i--)
        {
            var wait = Targets2[i].Postload();
            if (wait == WaitForMultiObjects.WaitReturn.Continue)
            {
                Targets1.Add(Targets2[i]);
                Targets2.RemoveAt(i);
            }
        }
        if (Targets2.Count == 0)
        {
            return WaitForMultiObjects.WaitReturn.Continue;
        }
        else
        {
            return WaitForMultiObjects.WaitReturn.Wait;
        }
    }

    public WaitForMultiObjects.WaitReturn Preload()
    {
        for (int i = Targets1.Count - 1; i >= 0; i--)
        {
            var wait = Targets1[i].Preload();
            if (wait == WaitForMultiObjects.WaitReturn.Continue)
            {
                Targets2.Add(Targets1[i]);
                Targets1.RemoveAt(i);
            }
        }
        if (Targets1.Count == 0)
        {
            return WaitForMultiObjects.WaitReturn.Continue;
        }
        else
        {
            return WaitForMultiObjects.WaitReturn.Wait;
        }
    }

    public void Release()
    {
        for (int i = Targets1.Count - 1; i >= 0; i--)
        {
            Targets1[i].Release();
        }
    }
}
