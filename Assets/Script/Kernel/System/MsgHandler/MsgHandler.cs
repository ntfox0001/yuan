using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MessageCallback
{
    public delegate void OnMessageResponseHandler(LitJson.JsonData jd);
    public event OnMessageResponseHandler MessageResponse;

    public void Touch(LitJson.JsonData jd)
    {
        if (MessageResponse != null)
        {
            MessageResponse(jd);
        }

    }
}
class MessageItem
{
    public string Name;
    public LitJson.JsonData Data;
}
// 单线程的消息分发
public class MsgHandler
{
    Dictionary<string, MessageCallback> mMessageCallbackMap = new Dictionary<string, MessageCallback>();
    List<MessageItem> mMessageBuffer = new List<MessageItem>();
    bool mQuit = false;
    public void Initial()
    {
        GlobalObjects.GetSingleton().StartCoroutine(DispatchMsg());
    }
    IEnumerator DispatchMsg()
    {
        while (!mQuit)
        {
            yield return null;
            foreach (MessageItem i in mMessageBuffer)
            {
                MessageCallback msgcb;
                if (mMessageCallbackMap.TryGetValue(i.Name, out msgcb))
                {
                    try
                    {
                        msgcb.Touch(i.Data);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogWarning(e.Message + ",  msg:" + i.Name);
                    }
                }
            }
            mMessageBuffer.Clear();
        }
    }
    public void Release()
    {
        mQuit = true;
    }
    public void RegisterJsonMessage(string messageName, MessageCallback.OnMessageResponseHandler e)
    {
        MessageCallback msgCallback;

        if (mMessageCallbackMap.TryGetValue(messageName, out msgCallback))
        {
            msgCallback.MessageResponse += e;
        }
        else
        {
            msgCallback = new MessageCallback();
            msgCallback.MessageResponse += e;
            mMessageCallbackMap.Add(messageName, msgCallback);
        }
    }

    public void UnregisterJsonMessage(string messageName, MessageCallback.OnMessageResponseHandler e)
    {
        MessageCallback msgCallback;

        if (mMessageCallbackMap.TryGetValue(messageName, out msgCallback))
        {
            msgCallback.MessageResponse -= e;
        }
        else
        {
            Debug.LogWarning("Failed to Unregister Message:" + messageName);
        }
    }

    public void SendMessage(string messageName, LitJson.JsonData data)
    {
        MessageItem msgItem = new MessageItem();
        msgItem.Name = messageName;
        msgItem.Data = data;
        mMessageBuffer.Add(msgItem);
    }

    static public LitJson.JsonData MsgData(string key, int data)
    {
        LitJson.JsonData jd = new LitJson.JsonData();
        jd[key] = data;
        return jd;
    }
    static public LitJson.JsonData MsgData(string key, float data)
    {
        LitJson.JsonData jd = new LitJson.JsonData();
        jd[key] = data;
        return jd;
    }
    static public LitJson.JsonData MsgData(string key, string data)
    {
        LitJson.JsonData jd = new LitJson.JsonData();
        jd[key] = data;
        return jd;
    }
    static public LitJson.JsonData MsgData(string key, bool data)
    {
        LitJson.JsonData jd = new LitJson.JsonData();
        jd[key] = data;
        return jd;
    }
}
