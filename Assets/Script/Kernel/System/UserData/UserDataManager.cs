using System.Collections;
using System.Collections.Generic;

// 内存级全局临时数据存放
public class UserDataManager : Singleton<UserDataManager>, IManagerBase
{
    LitJson.JsonData mUserData = new LitJson.JsonData();
    public LitJson.JsonData UserData
    {
        get { return mUserData; }
    }
    public string GetValue(string key)
    {
        if (mUserData.Keys.Contains(key))
        {
            return mUserData[key].ToString();
        }
        else
        {
            return string.Empty;
        }
    }
    public LitJson.JsonData GetJsonValue(string key)
    {
        return mUserData[key];
    }
    public void SetValue(string key, string value)
    {
        mUserData[key] = value;
    }
    public bool HasKey(string key)
    {
        return mUserData.Keys.Contains(key);
    }
    
    public void SetValue(string key, LitJson.JsonData jd)
    {
        mUserData[key] = jd;
    }
    public void Clear()
    {
        mUserData = new LitJson.JsonData();
    }

    public void Initial()
    {
        
    }

    public void Release()
    {
        
    }
}
