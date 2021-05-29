using System.Collections;
using System.Collections.Generic;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.CLR.Method;
using System.Text;
using UnityEngine;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;
#if UNITY_EDITOR
using System.Reflection;
#endif
public class ScriptInstance
{
    public const string GroupName = "il";
    

#if UNITY_EDITOR
    Assembly mAssembly = null;
#endif
    ILRuntime.Runtime.Enviorment.AppDomain mAppDomain = null;
    bool mUseHot = false;
    public ILRuntime.Runtime.Enviorment.AppDomain Environment
    {
        get { return mAppDomain; }
    }

    public ScriptInstance(string LogicScriptFilename, bool useHot, int debugPort)
    {
        mUseHot = useHot;
        if (mUseHot)
        {
            mAppDomain = new ILRuntime.Runtime.Enviorment.AppDomain();
            mAppDomain.DebugService.StartDebugService(debugPort);
            LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(mAppDomain);

            //加载L#模块
            ResourceManager.GetSingleton().LoadAssetBundle(GroupName);
            TextAsset dll = ResourceManager.GetSingleton().CreateResource<TextAsset>(LogicScriptFilename + "_dll", GroupName);
            TextAsset pdb = ResourceManager.GetSingleton().CreateResource<TextAsset>(LogicScriptFilename + "_pdb", GroupName);
            if (dll == null)
            {
                Debug.LogWarning("dll does not exist. use native code.");
                throw new System.IO.FileNotFoundException("dll does not exist:" + LogicScriptFilename);
            }

            System.IO.MemoryStream msDll = new System.IO.MemoryStream(dll.bytes);

            System.IO.MemoryStream msPdb = null;
            if (pdb != null)
            {
                msPdb = new System.IO.MemoryStream(pdb.bytes);

            }
            mAppDomain.LoadAssembly(msDll, msPdb, new Mono.Cecil.Pdb.PdbReaderProvider());

            // 释放代码资源
            ResourceManager.GetSingleton().ReleaseAssetBundle(GroupName);

            unsafe
            {
                System.Type[] types = new System.Type[] { typeof(string) };
                mAppDomain.RegisterCLRMethodRedirection(typeof(Debug).GetMethod("Log", types), DLog);
                mAppDomain.RegisterCLRMethodRedirection(typeof(Debug).GetMethod("LogWarning", types), DLogWarning);
                mAppDomain.RegisterCLRMethodRedirection(typeof(Debug).GetMethod("LogError", types), DLogError);
            }
            ScriptPreCode.PreCode(mAppDomain);
            ILRuntime.Runtime.Generated.CLRBindings.Initialize(mAppDomain);
            
        }
#if UNITY_EDITOR
        else
        {

            string dllfn = string.Format("Temp/{0}.dll", System.IO.Path.GetFileName(System.IO.Path.GetTempFileName()));

            System.IO.File.Copy(ScriptManager.GetSingleton().DebugDllPath + LogicScriptFilename + ".dll", dllfn);
            System.IO.File.Copy(ScriptManager.GetSingleton().DebugDllPath + LogicScriptFilename + ".dll.mdb", dllfn + ".mdb");
            mAssembly = Assembly.LoadFrom(dllfn);
            
            
        }
#endif
    }
    public IScriptClassInterface CreateScriptClass(string className, params object[] paramsList)
    {
        if (mUseHot)
        {
            return new ScriptClass(mAppDomain, className, paramsList);
        }
#if UNITY_EDITOR
        else
        {
            return new ReflectionScriptClass(mAssembly, className, paramsList);
        }
#endif
        return null;
    }
    public object CallStaticFunction(string className, string funcName, params object[] paramList)
    {
        IType classType = mAppDomain.GetType(className) as ILType;
        if (classType == null)
        {
            Debug.LogError("The ClassType don't exist: " + className);
            return null;
        }

        List<IType> paramsTypeList = ScriptManager.GetParamsTypeList(mAppDomain, paramList);
        IMethod method = classType.GetMethod(funcName, paramsTypeList, null);
        if (method == null)
        {
            // 如果没找到函数，那么寻找父类的函数
            if (classType.BaseType != null)
            {
                method = classType.GetMethod(funcName, paramsTypeList, null);
            }
            else
            {
                Debug.LogError("The function don't exist: " + funcName + " class:" + classType.FullName);
                return null;
            }
        }
        try
        {
            if (method.IsPublic)
                return mAppDomain.Invoke(method, null, paramList);
            else
                Debug.LogError("can't call private function: " + funcName + " class:" + classType.FullName);
        }
        catch (ILRuntime.Runtime.Intepreter.ILRuntimeException ile)
        {
            StringBuilder excepMsg = new StringBuilder();
            excepMsg.AppendLine(classType.FullName);
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

    public unsafe static StackObject* DLog(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        //只有一个参数，所以返回指针就是当前栈指针ESP - 1
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);
        //第一个参数为ESP -1， 第二个参数为ESP - 2，以此类推
        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        //获取参数message的值
        object message = StackObject.ToObject(ptr_of_this_method, __domain, __mStack);
        //需要清理堆栈
        __intp.Free(ptr_of_this_method);
        //如果参数类型是基础类型，例如int，可以直接通过int param = ptr_of_this_method->Value获取值，
        //关于具体原理和其他基础类型如何获取，请参考ILRuntime实现原理的文档。

        //通过ILRuntime的Debug接口获取调用热更DLL的堆栈
        string stackTrace = __domain.DebugService.GetStackTrance(__intp);
        Debug.Log(string.Format("{0}\n{1}", message, stackTrace));
        return __ret;
    }
    public unsafe static StackObject* DLogWarning(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        //只有一个参数，所以返回指针就是当前栈指针ESP - 1
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);
        //第一个参数为ESP -1， 第二个参数为ESP - 2，以此类推
        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        //获取参数message的值
        object message = StackObject.ToObject(ptr_of_this_method, __domain, __mStack);
        //需要清理堆栈
        __intp.Free(ptr_of_this_method);
        //如果参数类型是基础类型，例如int，可以直接通过int param = ptr_of_this_method->Value获取值，
        //关于具体原理和其他基础类型如何获取，请参考ILRuntime实现原理的文档。

        //通过ILRuntime的Debug接口获取调用热更DLL的堆栈
        string stackTrace = __domain.DebugService.GetStackTrance(__intp);
        Debug.LogWarning(string.Format("{0}\n{1}", message, stackTrace));
        return __ret;
    }
    public unsafe static StackObject* DLogError(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        //只有一个参数，所以返回指针就是当前栈指针ESP - 1
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);
        //第一个参数为ESP -1， 第二个参数为ESP - 2，以此类推
        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        //获取参数message的值
        object message = StackObject.ToObject(ptr_of_this_method, __domain, __mStack);
        //需要清理堆栈
        __intp.Free(ptr_of_this_method);
        //如果参数类型是基础类型，例如int，可以直接通过int param = ptr_of_this_method->Value获取值，
        //关于具体原理和其他基础类型如何获取，请参考ILRuntime实现原理的文档。

        //通过ILRuntime的Debug接口获取调用热更DLL的堆栈
        string stackTrace = __domain.DebugService.GetStackTrance(__intp);
        Debug.LogError(string.Format("{0}\n{1}", message, stackTrace));
        return __ret;
    }
}
