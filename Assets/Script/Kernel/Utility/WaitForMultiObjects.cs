using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForMultiObjects : CustomYieldInstruction
{
    public enum WaitReturn
    {
        Continue,
        Wait,
    }
    public enum WaitForType
    {
        WaitForAny,
        WaitForAll,
    }
    CustomYieldInstruction[] mWaitObject;
    WaitForType mComboType;
    /// <summary>
    /// 等待多个对象，当真时，继续等待，当假时不等待
    /// </summary>
    /// <param name="comboType">组合类型，WaitForAny：有一个触发就触发 WaitForAll：所有都触发才触发</param>
    /// <param name="objects"></param>
    public WaitForMultiObjects(WaitForType comboType, params CustomYieldInstruction[] objects)
    {
        mComboType = comboType;
        mWaitObject = objects;
    }
    public override bool keepWaiting
    {
        get
        {
            if (mComboType == WaitForType.WaitForAll)
            {
                bool keepwait = false;
                foreach (CustomYieldInstruction obj in mWaitObject)
                {
                    keepwait = keepwait || obj.keepWaiting;
                }
                return keepwait;
            }
            else
            {
                bool keepwait = true;
                foreach (CustomYieldInstruction obj in mWaitObject)
                {
                    keepwait = keepwait && obj.keepWaiting;
                }
                return keepwait;
            }
        }
    }


}
