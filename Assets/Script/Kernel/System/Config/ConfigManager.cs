using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.Networking;
using System;
using System.IO;

// 从config文件中读取配置
public class ConfigManager : Singleton<ConfigManager>, IManagerBase
{
    public enum SourceType
    {
        Local, Net,
    }
    public XmlDocument ConfigXmlDocument = new XmlDocument();
    bool mLoadXmlFinished = false;

    public bool LoadSuccess { get; private set; }
    public SourceType LoadSourceType { get; private set; }
    public bool LoadXml(string content)
    {
        ConfigXmlDocument.LoadXml(content);
        LoadSourceType = SourceType.Local;
        LoadSuccess = true;
        return true;
    }
    public CustomYieldInstruction LoadXmlFromUrl(string url)
    {
        LoadSuccess = false;
        StartCoroutine(DelayWebGetXml(url, 0, 0));
        return new WaitUntil(() => { return mLoadXmlFinished; });
    }
    
    IEnumerator DelayWebGetXml(string url, float delay, int retryCount)
    {
        if (retryCount < 3)
        {
            yield return new WaitForSeconds(delay);

            var cb = new ProgressCallback<UnityWebRequest>();
            cb.OnFinish = (UnityWebRequest req, object userData) =>
            {
                LoadXml(req.downloadHandler.text);
                LoadSourceType = SourceType.Net;
                mLoadXmlFinished = true;
            };
            cb.OnError = (Exception e, object userData) =>
            {
                Debug.Log("loadXml error:" + e.Message);
                // 读取失败,再试一次
                retryCount++;
                StartCoroutine(DelayWebGetXml(url, 1.0f, retryCount));
            };

            HttpManager.GetSingleton().HttpGet(url, null, cb, 3);
        }
        else
        {
            mLoadXmlFinished = true;
        }
    }

    public bool LoadXmlFile(string name)
    {
        FileStream fs = new FileStream(name, FileMode.Open, FileAccess.Read);
        MemoryStream ms = new MemoryStream();
        if (EncryptUtility.SHA_Dencrypt(fs, ms, GlobalObjects.GetSingleton().GetIndieNiNiName()) == EncryptUtility.Error.OK)
        {
            ms.Seek(0, SeekOrigin.Begin);
            ConfigXmlDocument.Load(ms);
            ms.Close();
            fs.Close();
        }

        ms.Close();
        fs.Close();

        LoadSuccess = true;

        return true;
    }
    public void SaveXmlFile(string name)
    {
        MemoryStream xmlStream = new MemoryStream();
        ConfigXmlDocument.Save(xmlStream);
        xmlStream.Seek(0, SeekOrigin.Begin);
        FileStream fs = new FileStream(name, FileMode.Create, FileAccess.Write);
        EncryptUtility.SHA_Encrypt(xmlStream, fs, GlobalObjects.GetSingleton().GetIndieNiNiName());
        fs.Close();
        xmlStream.Close();
    }
    public void Initial()
    {
    }

    public void Release()
    {
        
    }
    public string GetString(string key, string defVal = "")
    {
        if (ConfigXmlDocument.DocumentElement == null)
        {
            return defVal;
        }

        XmlNode n = ConfigXmlDocument.DocumentElement.SelectSingleNode(key);
        if (n == null)
        {
            return defVal;
        }

        return n.InnerText;
    }

    string GetPlatformPrefix()
    {
#if UNITY_ANDROID 
        return "Android";
#elif UNITY_IOS
        return "iOS";
#else
#error "no define"
#endif
    }

    public string GetPlatformString(string key, string defVal = "")
    {
        key = GetPlatformPrefix() + key;

        return GetString(key, defVal);
    }

    public int GetInt(string key, int defVal = 0)
    {
        var s = GetString(key, "0");

        int i;
        if (!int.TryParse(s, out i))
        {
            return defVal;
        }
        return i;
    }

    public int GetPlatformInt(string key, int defVal = 0)
    {
        var s = GetPlatformString(key, "0");

        int i;
        if (!int.TryParse(s, out i))
        {
            return defVal;
        }
        return i;
    }

}
