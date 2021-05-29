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
using System;
public class ScriptPreCode
{
    public delegate TResult OnValidateHandler<T1, T2, TResult>(T1 arg1, T2 arg2, TResult result);
    public static void PreCode(ILRuntime.Runtime.Enviorment.AppDomain appDomain)
    {
        appDomain.RegisterCrossBindingAdaptor(new CoroutineAdapter());

        
        #region//(Uinty委托)参数string,bool，string无返回值
        //  public delegate void AdvertisingIdentifierCallback(string advertisingId, bool trackingEnabled, string errorMsg);
        appDomain.DelegateManager.RegisterMethodDelegate<string,bool,string>();
        appDomain.DelegateManager.RegisterDelegateConvertor<Application.AdvertisingIdentifierCallback>((act) =>
            {
                return new Application.AdvertisingIdentifierCallback((advertisingId, trackingEnabled, errorMsg) =>
                {
                    ((Action<string, bool, string>)act)(advertisingId, trackingEnabled, errorMsg);
                });
            });
        #endregion
        
        #region//(Uinty委托)参数string,string，LogType无返回值
        //public delegate void LogCallback(string condition, string stackTrace, LogType type);
        appDomain.DelegateManager.RegisterMethodDelegate<string, string, LogType>();
        appDomain.DelegateManager.RegisterDelegateConvertor<Application.LogCallback>((act) =>
            {
                return new Application.LogCallback((condition, stackTrace, type) =>
                {
                    ((Action<string, string, LogType>)act)(condition, stackTrace, type);
                });
            });
        #endregion
    
        #region//(Uinty委托)无参数,无返回值
        //public delegate void LowMemoryCallback();
        appDomain.DelegateManager.RegisterDelegateConvertor<Application.LowMemoryCallback>((act) =>
            {
                return new Application.LowMemoryCallback(() =>
                {
                    ((System.Action)act)();
                });
            });

        #endregion

        #region//参数GameObject,无返回值
        appDomain.DelegateManager.RegisterMethodDelegate<GameObject>();
        appDomain.DelegateManager.RegisterDelegateConvertor<WindowBase.OnDestroyWindowHandler>((act) =>
            {
                return new WindowBase.OnDestroyWindowHandler((obj) =>
                {
                    ((Action<GameObject>)act)(obj);
                });
            });    
        #endregion


        #region//参数gameObject,WindowStack 无返回值
        appDomain.DelegateManager.RegisterMethodDelegate<GameObject, WindowStack>();
        appDomain.DelegateManager.RegisterDelegateConvertor<WindowManager.OnCreateWindowHandler>((act) =>
            {
                return new WindowManager.OnCreateWindowHandler((window, ws) =>
                {
                    ((Action<GameObject, WindowStack>)act)(window, ws);
                });
            });
        appDomain.DelegateManager.RegisterDelegateConvertor<WindowStack.OnCreateWindowHandler>((act) =>
        {
            return new WindowStack.OnCreateWindowHandler((window, ws) =>
            {
                ((Action<GameObject, WindowStack>)act)(window, ws);
            });
        });
        appDomain.DelegateManager.RegisterDelegateConvertor<WindowStack.OnWindowCloseHandler>((act) =>
        {
            return new WindowStack.OnWindowCloseHandler((window, ws) =>
            {
                ((Action<GameObject, WindowStack>)act)(window, ws);
            });
        });
        #endregion

        #region//参数WindowStack 无返回值
        appDomain.DelegateManager.RegisterMethodDelegate<WindowStack>();
        appDomain.DelegateManager.RegisterDelegateConvertor<WindowManager.OnCreateWindowStackHandler>((act) =>
        {
            return new WindowManager.OnCreateWindowStackHandler((ws) =>
            {
                ((Action< WindowStack>)act)(ws);
            });
        });
        appDomain.DelegateManager.RegisterDelegateConvertor<WindowStack.OnPrimitiveWindowCloseHandler>((act) =>
        {
            return new WindowStack.OnPrimitiveWindowCloseHandler((ws) =>
            {
                ((Action<WindowStack>)act)(ws);
            });
        });
        appDomain.DelegateManager.RegisterDelegateConvertor<WindowStack.OnReleaseWindowStackHandler>((act) =>
        {
            return new WindowStack.OnReleaseWindowStackHandler((ws) =>
            {
                ((Action<WindowStack>)act)(ws);
            });
        });
        #endregion

        appDomain.DelegateManager.RegisterFunctionDelegate<int, int, int>();
        
    }
}
