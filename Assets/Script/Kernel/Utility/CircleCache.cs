using System;
using System.Collections.Generic;

/// <summary>
/// 循环cache，当chache满的时候，将覆盖掉最后一个
/// 只能取得最后添加的size个的对象
/// </summary>
/// <typeparam name="T"></typeparam>
public class CircleCache<T>
{
    T mEmpty;
    T[] mBuffer;
    int mIndex = 0;
    int mDirty = 0;

    public CircleCache(T empty, int cacheSize)
    {
        mEmpty = empty;
        mBuffer = new T[cacheSize];
    }
    public void Add(T content)
    {
        mBuffer[mIndex % mBuffer.Length] = content;
        mIndex++;
        if (mIndex - mBuffer.Length > mDirty)
        {
            mDirty = mIndex - mBuffer.Length;
        }
    }
    public bool HasContent()
    {
        return (mDirty != mIndex);
    }
    public T Get()
    {
        if (HasContent())
        {
            T rt = mBuffer[mDirty % mBuffer.Length];
            mDirty++;
            return rt;
        }
        else
        {
            return mEmpty;
        }
    }
    public int Count
    {
        get
        {
            return mIndex - mDirty;
        }
    }
}
