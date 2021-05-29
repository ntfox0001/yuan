using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class PersistentFile
{
    private Dictionary<string, string> mPersistentDataMap = new Dictionary<string, string>();
    string mPersistentDataFileName;
    public string Path { get; private set; }
    public PersistentFile(string filename)
    {
        mPersistentDataFileName = filename;
        Path = string.Format("{0}/{1}", Application.persistentDataPath, mPersistentDataFileName);
    }
    public Dictionary<string, string> PersistentData
    {
        get { return mPersistentDataMap; }
    }
    public void SavePersistentData()
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(Path, FileMode.Create)))
        {
            writer.Write(mPersistentDataMap.Count);
            foreach (KeyValuePair<string, string> i in mPersistentDataMap)
            {
                writer.Write(i.Key);
                writer.Write(i.Value);
            }
        }
    }
    public void LoadPersistentData()
    {
        mPersistentDataMap.Clear();
        try
        {
            using (BinaryReader reader = new BinaryReader(File.Open(Path, FileMode.OpenOrCreate)))
            {
                if (reader.BaseStream.Length > 0)
                {
                    int count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        string key = reader.ReadString();
                        string value = reader.ReadString();
                        mPersistentDataMap.Add(key, value);
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public void SetString(string key, string value)
    {
        if (mPersistentDataMap.ContainsKey(key))
        {
            mPersistentDataMap[key] = value;
        }
        else
        {
            mPersistentDataMap.Add(key, value);
        }

        SavePersistentData();

    }
    public void RemoveString(string key)
    {
        mPersistentDataMap.Remove(key);
        SavePersistentData();
    }
    public string GetString(string key)
    {
        string value;
        if (mPersistentDataMap.TryGetValue(key, out value))
        {
            return value;
        }
        return string.Empty;
    }
    public bool HasKey(string key)
    {
        return mPersistentDataMap.ContainsKey(key);
    }
    public void Clear()
    {
        mPersistentDataMap.Clear();
        SavePersistentData();
    }
}
