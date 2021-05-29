using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.CLR.Method;
using System.Text;
public class ScriptClass : IScriptClassInterface
{
    //ICLRType mClassType = null;
    //ICLRType mParentClassType = null;
    //CLRSharp_Instance mClassInstance = null;
    ILType mClassType = null;
    AppDomain mAppDomain;
    ILTypeInstance mClassInstance = null;
    string mClassName;
    public ILType ClassType { get { return mClassType; } }
    public ILTypeInstance ClassInstance { get { return mClassInstance; } }
    public ScriptClass(AppDomain appDomain, string className, params object[] paramList)
    {
        mAppDomain = appDomain;
        mClassType = mAppDomain.GetType(className) as ILType;
        if (mClassType == null)
        {
            Debug.LogError("The ClassType don't exist: " + className);
            return;
        }

        mClassInstance = mClassType.Instantiate(false);
        if (mClassInstance == null)
        {
            Debug.LogError("Error in Instance Class: " + className);
            return;
        }

        List<IType> typeList = ScriptManager.GetParamsTypeList(appDomain, paramList);
        IMethod method = mClassType.GetConstructor(typeList);
        try
        {
            if (method.IsPublic)
                mAppDomain.Invoke(method, mClassInstance, paramList);
        }
        catch (ILRuntime.Runtime.Intepreter.ILRuntimeException ile)
        {
            StringBuilder excepMsg = new StringBuilder();
            excepMsg.AppendLine(mClassType.FullName);
            excepMsg.AppendLine(ile.Message);
            if (!string.IsNullOrEmpty(ile.ThisInfo))
            {
                excepMsg.AppendLine("this:");
                excepMsg.AppendLine(ile.ThisInfo);
            }
            excepMsg.AppendLine("Local Variables:");
            excepMsg.AppendLine(ile.LocalInfo);
            excepMsg.AppendLine(ile.StackTrace);
            if (ile.InnerException != null)
                excepMsg.AppendLine(ile.InnerException.ToString());

            Debug.LogError(excepMsg.ToString());
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }        
    }
    public object CallInstanceFunction(string funcName, params object[] paramList)
    {
        List<IType> paramsTypeList = ScriptManager.GetParamsTypeList(mAppDomain, paramList);
        IMethod method = mClassType.GetMethod(funcName, paramsTypeList, null);
        if (method == null)
        {
            // 如果没找到函数，那么寻找父类的函数
            if (mClassType.BaseType != null)
            {
                method = mClassType.GetMethod(funcName, paramsTypeList, null);
            }
            else
            {
                Debug.LogError("The function don't exist: " + funcName + " class:" + mClassType.FullName);
                return null;
            }
        }
        try
        {
            if (method.IsPublic)
                return mAppDomain.Invoke(method, mClassInstance, paramList);
            else
                Debug.LogError("can't call private function: " + funcName + " class:" + mClassType.FullName);
        }
        catch (ILRuntime.Runtime.Intepreter.ILRuntimeException ile)
        {
            StringBuilder excepMsg = new StringBuilder();
            excepMsg.AppendLine(mClassType.FullName);
            excepMsg.AppendLine(ile.Message);
            if (!string.IsNullOrEmpty(ile.ThisInfo))
            {
                excepMsg.AppendLine("this:");
                excepMsg.AppendLine(ile.ThisInfo);
            }
            excepMsg.AppendLine("Local Variables:");
            excepMsg.AppendLine(ile.LocalInfo);
            excepMsg.AppendLine(ile.StackTrace);
            if (ile.InnerException != null)
                excepMsg.AppendLine(ile.InnerException.ToString());

            Debug.LogError(excepMsg.ToString());
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
        return null;
    }
    public void SetMemberValue(string name, object val)
    {
        System.Reflection.FieldInfo field = mClassType.ReflectionType.GetField(name);
        if (field == null)
        {
            
            Debug.LogError("The field don't exist: " + name + " class:" + mClassType.FullName);
            return;
        }
        try
        {
            field.SetValue(mClassInstance, val);
        }
        catch (ILRuntime.Runtime.Intepreter.ILRuntimeException ile)
        {
            StringBuilder excepMsg = new StringBuilder();
            excepMsg.AppendLine(mClassType.FullName);
            excepMsg.AppendLine(ile.Message);
            if (!string.IsNullOrEmpty(ile.ThisInfo))
            {
                excepMsg.AppendLine("this:");
                excepMsg.AppendLine(ile.ThisInfo);
            }
            excepMsg.AppendLine("Local Variables:");
            excepMsg.AppendLine(ile.LocalInfo);
            excepMsg.AppendLine(ile.StackTrace);
            if (ile.InnerException != null)
                excepMsg.AppendLine(ile.InnerException.ToString());

            Debug.LogError(excepMsg.ToString());
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
    public object GetMemberValue(string name)
    {
        System.Reflection.FieldInfo field = mClassType.ReflectionType.GetField(name);
        if (field == null)
        {

            Debug.LogError("The field don't exist: " + name + " class:" + mClassType.FullName);
            return null;
        }
        try
        {
            return field.GetValue(mClassInstance);
        }
        catch (ILRuntime.Runtime.Intepreter.ILRuntimeException ile)
        {
            StringBuilder excepMsg = new StringBuilder();
            excepMsg.AppendLine(mClassType.FullName);
            excepMsg.AppendLine(ile.Message);
            if (!string.IsNullOrEmpty(ile.ThisInfo))
            {
                excepMsg.AppendLine("this:");
                excepMsg.AppendLine(ile.ThisInfo);
            }
            excepMsg.AppendLine("Local Variables:");
            excepMsg.AppendLine(ile.LocalInfo);
            excepMsg.AppendLine(ile.StackTrace);
            if (ile.InnerException != null)
                excepMsg.AppendLine(ile.InnerException.ToString());

            Debug.LogError(excepMsg.ToString());
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
        return null;
    }
    public void RegisterEvent(string eventName, params object[] paramList)
    {
        CallInstanceFunction("add_" + eventName, paramList);
    }
    public void UnregisterEvent(string eventName, params object[] paramList)
    {
        CallInstanceFunction("remove_" + eventName, paramList);
    }
    public void SetPropertyValue(string propName, params object[] paramList)
    {
        CallInstanceFunction("set_" + propName, paramList);
    }
    public object GetPropertyValue(string propName)
    {
        return CallInstanceFunction("get_" + propName);
    }
}

