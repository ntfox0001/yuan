using System.Collections;
using System.Collections.Generic;
using System;

public class EventCallback
{
    public delegate void OnEventResponseHandler(object data);
    public event OnEventResponseHandler EventResponse;

    public void Touch(object data)
    {
        if (EventResponse != null)
        {
            EventResponse(data);
        }

    }
}
// 单线程的消息分发
public class EventHandler
{

    public EventHandler()
    {

    }
    Dictionary<string, EventCallback> mEventCallbackMap = new Dictionary<string, EventCallback>();
        
    public void RegisterJsonEvent(string EventName, EventCallback.OnEventResponseHandler e)
    {
        EventCallback msgCallback;

        if (mEventCallbackMap.TryGetValue(EventName, out msgCallback))
        {
            msgCallback.EventResponse += e;
        }
        else
        {
            msgCallback = new EventCallback();
            msgCallback.EventResponse += e;
            mEventCallbackMap.Add(EventName, msgCallback);
        }
    }

    public void UnregisterJsonEvent(string EventName, EventCallback.OnEventResponseHandler e)
    {
        EventCallback msgCallback;

        if (mEventCallbackMap.TryGetValue(EventName, out msgCallback))
        {
            msgCallback.EventResponse -= e;
        }
        else
        {
            UnityEngine.Debug.LogWarning("Failed to Unregister Event:" + EventName);
        }
    }
    /// <summary>
    ///  执行回调
    /// </summary>
    /// <param name="EventName"></param>
    /// <param name="data"></param>
    public void ExecuteEvent(string EventName, object data)
    {
        EventCallback msgcb;
        if (mEventCallbackMap.TryGetValue(EventName, out msgcb))
        {
            try
            {
                msgcb.Touch(data);
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError(e.Message + "  msg:" + EventName + "\n" + e.StackTrace);
            }
        }
    }
}
