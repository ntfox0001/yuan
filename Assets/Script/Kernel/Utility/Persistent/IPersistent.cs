using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPersistent
{
    T LoadData<T>(string key);
    int LoadDataInt(string key);
    float LoadDataFloat(string key);
    uint LoadDataUInt(string key);
    string LoadDataString(string key);
    bool LoadDataBool(string key);
    void SaveData<T>(string key, T source);
    void RemoveData(string key);
    bool HasData(string key);
    void Clear();
}
