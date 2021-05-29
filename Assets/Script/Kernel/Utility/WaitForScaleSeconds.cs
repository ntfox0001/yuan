using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForScaleSeconds : CustomYieldInstruction
{
    float mSeconds = 0;
    public WaitForScaleSeconds(float sec)
    {
        mSeconds = sec;
    }

    public override bool keepWaiting
    {
        get
        {
            mSeconds -= Time.deltaTime;
            return (mSeconds <= 0);
        }
    }
}
