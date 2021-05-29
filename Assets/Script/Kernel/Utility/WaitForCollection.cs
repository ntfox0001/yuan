using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WaitForCollection<T> : CustomYieldInstruction
{
    IEnumerator<T> mEnumerator;
    Func<T, bool> mProcess;
    public WaitForCollection(IEnumerator<T> enumerator, Func<T, bool> process)
    {
        mEnumerator = enumerator;
        mProcess = process;
    }
    public override bool keepWaiting
    {
        get
        {
            while (mEnumerator.MoveNext())
            {
                if (mProcess != null)
                {
                    if (!mProcess(mEnumerator.Current))
                    {
                        // 当有false时，返回等待
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
