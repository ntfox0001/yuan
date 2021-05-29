using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoReadyAdUnit : AdUnitBase
{
    public override void Release()
    {
        
    }

    public override bool Show()
    {
        return false;
    }
}
