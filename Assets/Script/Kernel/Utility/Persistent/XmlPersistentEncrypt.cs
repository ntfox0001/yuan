using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class XmlPersistentEncrypt : IPersistent
{
    XmlDictionary<string, string> mPersistentMap;
    string mPassword;
    string mFilename;
    public XmlPersistentEncrypt(string filename, string password)
    {
        mPassword = password;
        mFilename = string.Format("{0}/{1}", Application.persistentDataPath, filename); ;

        FileStream fs = new FileStream(mFilename, FileMode.OpenOrCreate, FileAccess.Read);
        if (fs != null)
        {
            MemoryStream ms = new MemoryStream();

            if (EncryptUtility.SHA_Dencrypt(fs, ms, mPassword) == EncryptUtility.Error.OK)
            {
                ms.Position = 0;

                XmlSerializer serializer = new XmlSerializer(typeof(XmlDictionary<string, string>));
                StreamReader sr = new StreamReader(ms);
                mPersistentMap = (XmlDictionary<string, string>)serializer.Deserialize(sr);
                return;
            }

        }

        mPersistentMap = new XmlDictionary<string, string>();


    }
    public T LoadData<T>(string key)
    {
        if (mPersistentMap.ContainsKey(key))
        {
            string value = mPersistentMap[key];
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringReader sr = new StringReader(value);
            return (T)serializer.Deserialize(sr);
        }
        else
        {
            return default(T);
        }
    }
    public int LoadDataInt(string key)
    {
        return LoadData<int>(key);
    }
    public float LoadDataFloat(string key)
    {
        return LoadData<float>(key);
    }
    public uint LoadDataUInt(string key)
    {
        return LoadData<uint>(key);
    }
    public string LoadDataString(string key)
    {
        return LoadData<string>(key);
    }
    public bool LoadDataBool(string key)
    {
        return LoadData<bool>(key);
    }

    public void SaveData<T>(string key, T source)
    {

        XmlSerializer serializer = new XmlSerializer(typeof(T));
        StringWriter sw = new StringWriter();
        serializer.Serialize(sw, source);
        mPersistentMap[key] = sw.ToString();
        sw.Close();

        FlushData();
    }
    public void FlushData()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(XmlDictionary<string, string>));
        MemoryStream ms = new MemoryStream();
        serializer.Serialize(ms, mPersistentMap);

        FileStream fs = new FileStream(mFilename, FileMode.Create, FileAccess.Write);
        
        ms.Position = 0;

        EncryptUtility.SHA_Encrypt(ms, fs, mPassword);
        ms.Close();
        fs.Close();
        
    }
    public void RemoveData(string key)
    {
        mPersistentMap.Remove(key);
        FlushData();
    }
    public bool HasData(string key)
    {
        return mPersistentMap.ContainsKey(key);
    }
    public void Clear()
    {
        mPersistentMap.Clear();
        FlushData();
    }
}
