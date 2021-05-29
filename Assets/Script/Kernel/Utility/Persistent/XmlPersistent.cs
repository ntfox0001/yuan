using UnityEngine;
using System.IO;
using System.Xml.Serialization;

public class XmlPersistent : IPersistent
{
    string mFilename;
    PersistentFile mPersistentFile;
    public XmlPersistent(string filename)
    {
        mPersistentFile = new PersistentFile(filename);
        mPersistentFile.LoadPersistentData();
    }
    public T LoadData<T>(string key)
    {
        if (mPersistentFile.HasKey(key))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringReader sr = new StringReader(mPersistentFile.GetString(key));
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
        mPersistentFile.SetString(key, sw.ToString());
        sw.Close();
    }
    public void RemoveData(string key)
    {
        mPersistentFile.RemoveString(key);
    }
    public bool HasData(string key)
    {
        return mPersistentFile.HasKey(key);
    }
    public void Clear()
    {
        mPersistentFile.Clear();
    }

}
