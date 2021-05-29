using UnityEngine;
using System.Collections;

public interface IScriptClassInterface
{
    object CallInstanceFunction(string funcName, params object[] paramList);
    void SetMemberValue(string name, object val);
    object GetMemberValue(string name);
    void RegisterEvent(string eventName, params object[] paramList);
    void UnregisterEvent(string eventName, params object[] paramList);
    void SetPropertyValue(string propName, params object[] paramList);
    object GetPropertyValue(string propName);
}
