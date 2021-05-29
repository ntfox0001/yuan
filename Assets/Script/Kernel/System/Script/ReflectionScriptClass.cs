#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Reflection;
using System;
class ReflectionScriptClass : IScriptClassInterface
{
    System.Type mClassType;
    System.Object mClassInstance;
    public System.Type[] MakeParams(params object[] paramList)
    {
        if (paramList == null || paramList.Length == 0)
        {
            return null;
        }
        System.Type[] ts = new System.Type[paramList.Length];
        for (int i = 0; i < paramList.Length; i++)
        {
            ts[i] = paramList[i].GetType();
        }
        return ts;
    }

    public ReflectionScriptClass(Assembly assembly, string className, params object[] paramList)
    {
        mClassType = assembly.GetType(className);
        if (mClassType == null)
        {
            Debug.LogError("The ClassType don't exist: " + className);
            return;
        }
        try
        {
            mClassInstance = mClassType.Assembly.CreateInstance(className, false, BindingFlags.CreateInstance, null, paramList, null, null);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error in instance class: " + className);
        }
        if (mClassInstance == null)
        {
            Debug.LogError("Error in Instance Class: " + className);
            return;
        }
    }
    public object CallInstanceFunction(string funcName, params object[] paramList)
    {
        MethodInfo mi = null;
        if (paramList == null || paramList.Length == 0)
        {
            mi = mClassType.GetMethod(funcName);
        }
        else
        {
            mi = mClassType.GetMethod(funcName, MakeParams(paramList));
        }
        try
        {
            return mi.Invoke(mClassInstance, paramList);
        }
        catch (System.Exception e)
        {
            Debug.LogError(funcName + "\n" + e.Message);
        }
        return null;
    }
    public void SetMemberValue(string name, object val)
    {
        FieldInfo fi = mClassType.GetField(name);
        try
        {
            fi.SetValue(mClassInstance, val);
        }
        catch(System.Exception e)
        {
            Debug.LogError(name + "\n" + e.Message);
        }
    }
    public object GetMemberValue(string name)
    {
        FieldInfo fi = mClassType.GetField(name);
        try
        {
            return fi.GetValue(mClassInstance);
        }
        catch (System.Exception e)
        {
            Debug.LogError(name + "\n" + e.Message);
        }
        return null;
    }
    public void RegisterEvent(string eventName, params object[] paramList)
    {
        EventInfo ei = mClassType.GetEvent(eventName);
        try
        {
            ei.AddEventHandler(mClassInstance, (System.Delegate)paramList[0]);
        }
        catch (System.Exception e)
        {
            Debug.LogError(eventName + "\n" + e.Message);
        }
    }
    public void UnregisterEvent(string eventName, params object[] paramList)
    {
        EventInfo ei = mClassType.GetEvent(eventName);
        try
        {
            ei.RemoveEventHandler(mClassInstance, (System.Delegate)paramList[0]);
        }
        catch (System.Exception e)
        {
            Debug.LogError(eventName + "\n" + e.Message);
        }
    }
    public void SetPropertyValue(string propName, params object[] paramList)
    {
        PropertyInfo pi = mClassType.GetProperty(propName);
        try
        {
            pi.SetValue(mClassInstance, paramList[0], null);
        }
        catch (System.Exception e)
        {
            Debug.LogError(propName + "\n" + e.Message);
        }
        
    }
    public object GetPropertyValue(string propName)
    {
        PropertyInfo pi = mClassType.GetProperty(propName);
        try
        {
            return pi.GetValue(mClassInstance, null);
        }
        catch (System.Exception e)
        {
            Debug.LogError(propName + "\n" + e.Message);
        }
        return null;
    }
}

#endif