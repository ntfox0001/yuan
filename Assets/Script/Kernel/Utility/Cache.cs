using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Cache<T>
{
    Func<T> mNewFunc;
    List<T> mCache = new List<T>();
    public Cache(Func<T> newFunc)
    {
        mNewFunc = newFunc;
    }

    public T Get()
    {
        if (mCache.Count == 0)
        {
            return mNewFunc();
        }
        else
        {
            T rt = mCache[0];
            mCache.RemoveAt(0);
            return rt;
        }
    }

    public void Return(T item)
    {
        mCache.Add(item);
    }
}
