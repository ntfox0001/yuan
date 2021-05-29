using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ScriptBindWindow : WindowBase
{
    public string ClassName;
    public bool NeedUpdate = false;

    IScriptClassInterface mScriptClassInstance = null;
    public IScriptClassInterface Instance
    {
        get { return mScriptClassInstance; }
    }
    // Update is called once per frame

    public override void OnCreated(params object[] paramsList)
    {

        if (ClassName.Length != 0)
        {
            if (paramsList == null || paramsList.Length == 0)
            {
                mScriptClassInstance = ScriptManager.GetSingleton().DefaultScriptInstance.CreateScriptClass("ClientScript." + ClassName, gameObject);
            }
            else
            {
                object[] tmppl = new object[paramsList.Length + 1];
                paramsList.CopyTo(tmppl, 1);
                tmppl[0] = this.gameObject;
                mScriptClassInstance = ScriptManager.GetSingleton().DefaultScriptInstance.CreateScriptClass("ClientScript." + ClassName, tmppl);
            }

        }
    }
    void Update()
    {

        if (NeedUpdate && mScriptClassInstance != null)
            mScriptClassInstance.CallInstanceFunction("Update");

    }
    // 返回值表示是否关闭这个窗口
    public override void OnClosed()
    {
        if (mScriptClassInstance != null)
            mScriptClassInstance.CallInstanceFunction("OnClosed");
    }
    public override void OnAppearance()
    {

        if (mScriptClassInstance != null)
            mScriptClassInstance.CallInstanceFunction("OnAppearance");

    }
    public override void OnDisappearance()
    {

        if (mScriptClassInstance != null)
            mScriptClassInstance.CallInstanceFunction("OnDisappearance");


    }
}
