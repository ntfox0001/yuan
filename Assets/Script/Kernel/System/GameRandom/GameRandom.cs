using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.InteropServices;
public class GameRandomManager : Singleton<GameRandomManager>, IManagerBase
{
    GameRandom mGameRandom = new GameRandom();

    public void Initial()
    {
        mGameRandom.Initial();
    }

    public void Release()
    {
        
    }
}
public class GameRandom
{
    public const string GroupName = "gamerandom";
    static private UInt16[] mRandomArray = null;
    private UInt16 mSeed;

    public UInt16[] RandomArray
    {
        get { return mRandomArray; }
    }
    public void Initial()
    {
        if (mRandomArray == null)
        {
            ResourceManager.GetSingleton().LoadAssetBundle(GroupName);
            TextAsset ta = ResourceManager.GetSingleton().CreateResource<TextAsset>("RandomArray", GroupName);

            MemoryStream ms = new MemoryStream(ta.bytes);
            BinaryReader br = new BinaryReader(ms);

            mRandomArray = new UInt16[ta.bytes.Length / 2];
            //float t = Time.time;
            for (int i = 0; i < mRandomArray.Length; i++)
            {
                mRandomArray[i] = br.ReadUInt16();
            }

            ResourceManager.GetSingleton().ReleaseAssetBundle(GroupName);
        }
    }

    public void setSeed(UInt16 seed)
    {
        if (seed <= 32767)
        {
            mSeed = seed;
        }
        else
        {
            Debug.LogWarning("Failed to set game random seed.");
            mSeed = 0;
        }
    }

    public UInt32 getUInt32Random(UInt16 min, UInt16 max)
    {
        if (min > max)
        {
            Debug.LogWarning("Failed to get uint random for min > max.");
            return 0;
        }

        if (mRandomArray == null)
        {
            Debug.LogWarning("Failed to get uint random for randomarray is null.");
            return 0;
        }

        UInt32 random = mRandomArray[mSeed];
        mSeed++;
        if (mSeed == 32768)
        {
            mSeed = 0;
        }

        return (UInt32)(((float)random / 32767.0f) * ((float)max - (float)min + 0.5f) + (float)min);
    }

    public UInt32 getUInt32Random()
    {
        if (mRandomArray == null)
        {
            Debug.LogWarning("Failed to get uint random for randomarray is null.");
            return 0;
        }
        UInt32 random = mRandomArray[mSeed];
        mSeed++;
        if (mSeed == 32768)
        {
            mSeed = 0;
        }

        return random;
    }

    public float getFloatRandom(float min, float max)
    {
        if (min > max)
        {
            Debug.LogWarning("Failed to get float random for min > max.");
            return 0.0f;
        }

        if (mRandomArray == null)
        {
            Debug.LogWarning("Failed to get float random for randomarray is null.");
            return 0.0f;
        }

        UInt32 random = mRandomArray[mSeed];
        mSeed++;
        if (mSeed == 32768)
        {
            mSeed = 0;
        }

        return (random / 32767.0f) * (max - min) + min;
    }
}
