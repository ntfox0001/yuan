using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 固定大小的列表，先入后出，超过设定大小时，先释放最先加入的
/// </summary>
public class FixedList<T>
{
    List<T> mData = new List<T>();
    uint mSize = 0;
    public FixedList(uint size) 
    {
        mSize = size;
    }
    public void Add(T item)
    {
        mData.Insert(0, item);
        if (mData.Count >5)
        {
            mData.RemoveAt(mData.Count - 1);
        }
    }
    public int Count
    {
        get
        {
            return mData.Count;
        }
    }
    public T Get()
    {
        T item = mData[0];
        mData.RemoveAt(0);
        return item;
    }
    public void Clear()
    {
        mData = new List<T>();
    }
}
