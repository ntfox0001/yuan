using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public interface ICoolDown
{
    bool Hit();
    void Clear();
}
public class CoolDown
{
    /// <summary>
    /// 创建一个cd，使用共享cd数据
    /// </summary>
    /// <param name="name"></param>
    /// <param name="data"></param>
    /// <param name="cd"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    static public ICoolDown CreateCoolDown(string name, LitJson.JsonData data, float cd, float time = 0)
    {
        CoolDownShareData cdobj = new CoolDownShareData(name, data, cd, time);
        return cdobj;
    }
    /// <summary>
    /// 创建一个cd
    /// </summary>
    /// <param name="cd"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    static public ICoolDown CreateCoolDown(float cd, float time = 0)
    {
        CoolDownPrivateData cdobj = new CoolDownPrivateData(cd, time);
        return cdobj;
    }
}

public class CoolDownShareData : ICoolDown
{
    string mName;
    LitJson.JsonData mData = null;
    float mCoolDown;
    public CoolDownShareData(string name, LitJson.JsonData data, float cd, float time)
    {
        mName = name;
        mData = data;
        mCoolDown = cd;
        mData[name] = 0.0f;
    }
    /// <summary>
    /// 测试cd
    /// </summary>
    /// <returns>真，不在cd中，并触发新cd，假，在cd中</returns>
    public bool Hit()
    {
        float preTime = (float)mData[mName].GetDouble();
        if (preTime + mCoolDown <= Time.time)
        {
            mData[mName] = Time.time;
            return true;
        }
        return false;
    }
    /// <summary>
    /// 清除cd
    /// </summary>
    public void Clear()
    {
        mData[mName] = 0.0f;
    }
}

public class CoolDownPrivateData : ICoolDown
{
    float mCoolDown;
    float mPreTime;
    public CoolDownPrivateData(float cd, float time)
    {
        mPreTime = time;
        mCoolDown = cd;
    }
    /// <summary>
    /// 测试cd
    /// </summary>
    /// <returns>真，不在cd中，并触发新cd，假，在cd中</returns>
    public bool Hit()
    {
        if (mPreTime + mCoolDown <= Time.time)
        {
            mPreTime = Time.time;
            return true;
        }
        return false;
    }
    /// <summary>
    /// 清除cd
    /// </summary>
    public void Clear()
    {
        mPreTime = 0;
    }
}