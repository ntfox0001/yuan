using UnityEngine;
using System.Collections;

public class ScriptBind : MonoBehaviour
{
    public string ClassName;
    public bool CreateOnStart = false;
    public bool NeedUpdate = false;

    IScriptClassInterface mScriptClassInstance = null;
    public void OnCreated(params object[] paramsList)
    {

        if (ClassName.Length != 0)
        {
            object[] tmppl = new object[paramsList.Length + 1];
            paramsList.CopyTo(tmppl, 1);
            tmppl[0] = this.gameObject;
            mScriptClassInstance = ScriptManager.GetSingleton().DefaultScriptInstance.CreateScriptClass("ClientScript." + ClassName, tmppl);
        }
    }
    void Start()
    {
        if (CreateOnStart)
        {
            OnCreated();
        }
    }
    void Update()
    {

        if (NeedUpdate && mScriptClassInstance != null)
            mScriptClassInstance.CallInstanceFunction("Update");

    }
    void OnDestroy()
    {

        if (mScriptClassInstance != null)
            mScriptClassInstance.CallInstanceFunction("OnDestroy");

    }

    public IScriptClassInterface Instance
    {
        get { return mScriptClassInstance; }
    }
}
