using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.CLR.Method;


#if  USE_REFLECTION
using System.Reflection;
#endif

public class ScriptManager : Singleton<ScriptManager>,IManagerBase
{
    public string LogicScriptFilename = "";
    public string DebugDllPath = "../ClientScript/bin/Debug/";
    public int DebugPort = 56000;

    ScriptInstance mDefaultScriptInstance;
    public ScriptInstance DefaultScriptInstance { get { return mDefaultScriptInstance; } }

    public bool UseHot = true;
    bool mUseHot = true;
    
    public class ManagedScriptInfo
    {
        int mId = 0;
        public ManagedScriptInfo(int id) { mId = id; }
        public int Id { get { return Id; } }
        public ScriptInstance scriptInstance;
        public IScriptClassInterface classInstance;
        public string name;
        public bool needUpdate;
    }
    List<ManagedScriptInfo> mManagedScriptList = new List<ManagedScriptInfo>();

    void Awake()
    {
        mUseHot = UseHot;
    }
    public void Initial()
    {
        // create a default script instance
        mDefaultScriptInstance = new ScriptInstance(LogicScriptFilename, mUseHot, DebugPort);
    }
    public void Release()
    {
        
    }
    public static List<IType> GetParamsTypeList(AppDomain appDomain, params object[] paramsList)
    {
        if (paramsList == null | paramsList.Length == 0)
        {
            return null;
        }
        List<IType> typeList = new List<IType>();
        for (int i = 0; i < paramsList.Length; i++)
        {
            if (paramsList[i] == null)
            {
                typeList.Add(appDomain.GetType("System.Object"));
            }
            else
            {
                if (paramsList[i] is ILTypeInstance)
                {
                    ILTypeInstance ilIns = paramsList[i] as ILTypeInstance;
                    typeList.Add(ilIns.Type);
                }
                else
                {
                    typeList.Add(appDomain.GetType(paramsList[i].GetType()));
                }
                
            }
        }
        return typeList;
    }

    public static ParamsBind GetParams(GameObject obj, string name)
    {
        System.Collections.Generic.List<ParamsBind> pbList = new System.Collections.Generic.List<ParamsBind>();
        obj.GetComponents<ParamsBind>(pbList);
        for (int i = 0; i < pbList.Count; i++)
        {
            if (pbList[i].ParamsName == name)
                return pbList[i];
        }
        return null;
    }
    public static GameObject GetGameObject(ParamsBind pb, uint id)
    {
        return pb.GameObjectParams[id];
    }
    public static T GetComponent<T>(ParamsBind pb, uint id) where T : Component
    {
        T c = pb.GameObjectParams[id].GetComponent<T>();
        return c;
    }
    public static void DebugObject(object obj)
    {
        Debug.Log(obj.ToString());
    }

    public ManagedScriptInfo CreateManagedScriptClass(ScriptInstance scriptIns, string className, bool needUpdate, params object[] paramList)
    {

        ManagedScriptInfo msInfo = new ManagedScriptInfo(mManagedScriptList.Count + 1);
        msInfo.scriptInstance = scriptIns;
        msInfo.name = className;
        msInfo.needUpdate = needUpdate;
        msInfo.classInstance = scriptIns.CreateScriptClass(className, paramList);
        mManagedScriptList.Add(msInfo);
        return msInfo;
    }
    public void ReleaseManagedScriptClass(ManagedScriptInfo msInfo)
    {
        msInfo.classInstance.CallInstanceFunction("OnDestroy");

        for (int i = 0; i < mManagedScriptList.Count; i++)
        {
            if (mManagedScriptList[i].Id  == msInfo.Id)
            {
                mManagedScriptList.RemoveAt(i);
                return;
            }
        }
            
    }
    void Update()
    {
        for (int i = 0; i < mManagedScriptList.Count; i++)
        {
            if (mManagedScriptList[i].needUpdate)
                mManagedScriptList[i].classInstance.CallInstanceFunction("Update");
        }
    }
    void OnDestroy()
    {
        for (int i = 0; i < mManagedScriptList.Count; i++)
        {
            mManagedScriptList[i].classInstance.CallInstanceFunction("OnDestroy");
        }
        mManagedScriptList.Clear();
    }


}
