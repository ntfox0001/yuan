using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProcedureBase : MonoBehaviour {
    public System.Action OnInitialFinished; // 初始化完成回调

    public abstract void Initial();

    // 请开始你的表演
    public abstract void Do();
}
