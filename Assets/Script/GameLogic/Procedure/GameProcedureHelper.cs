using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProcedureHelper
{
    static public GameProcedure Get()
    {
        return (GameProcedure)GlobalObjects.GetSingleton().GameProcedure;
    }
}