using System;
using System.Collections.Generic;
using UnityEngine;
public class MathUtility
{
    public const double e = 2.718281828459045235360287471352;
    public static bool Equal(float a, float b)
    {
        float c = a - b;
        return FloatEqualZero(c);
    }
    public static bool FloatEqualZero(float a)
    {
        return (Math.Abs(a) > -0.000001f && Math.Abs(a) < 0.000001f);
    }
    /// <summary>
    /// 点到直线的投影，两点式，两条直线相交于m,斜率 k1 * k2 = -1
    /// 所以只需要解两个两点式方程组即可
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="line0"></param>
    /// <param name="line1"></param>
    /// <returns></returns>
    public static Vector2 GetProjectOfPointToLine(Vector2 p0, Vector2 line0, Vector2 line1)
    {
        float x = line1.x - line0.x;
        if (FloatEqualZero(x))
        {
            return new Vector2(p0.x, line0.y);
        }
        float k1 = (line1.y - line0.y) / (line1.x - line0.x);

        Vector2 pos;
        pos.y = (-k1 * line0.x + line0.y + k1 * k1 * p0.y + k1 * p0.x) / (1 + k1 * k1);
        pos.x = -k1 * (pos.y - p0.y) + p0.x;

        return pos;
    }
    /// <summary>
    /// 乱序
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void ListRandSort<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            var id = UnityEngine.Random.Range(0, list.Count);
            if (id == i) continue;
            T t = list[i];
            list[i] = list[id];
            list[id] = t;
        }
    }
    /// <summary>
    /// 产生随机数数组
    /// </summary>
    /// <param name="amount">数组大小</param>
    /// <returns></returns>
    public static List<int> GetRand(int amount)
    {
        var rt = new List<int>();
        for (int i = 0; i < amount; i++)
        {
            rt.Add(i);
        }
        ListRandSort(rt);

        return rt;

    }
    /// <summary>
    /// 随机打乱list，跟上面的不一样
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void GetRand<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r1 = UnityEngine.Random.Range(0, list.Count);
            int r2 = UnityEngine.Random.Range(0, list.Count);
            T t = list[r1];
            list[r1] = list[r2];
            list[r2] = t;
        }
    }
    /// <summary>
    /// 随机打乱array
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    public static void GetRand<T>(T[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int r1 = UnityEngine.Random.Range(0, array.Length);
            int r2 = UnityEngine.Random.Range(0, array.Length);
            T t = array[r1];
            array[r1] = array[r2];
            array[r2] = t;
        }
    }
    /// <summary>
    /// 随机获取整型数组部分索引
    /// </summary>
    /// <param name="amount">数组大小</param>
    /// <param name="count">随机数量</param>
    /// <returns></returns>
    public static List<int> GetPartRandNum(int amount,int count)
    {
        if (count > amount)
            return null;

        var rt = new List<int>();
        for (int i = 0; i < amount; i++)
        {
            rt.Add(i);
        }
        ListRandSort(rt);
        rt.RemoveRange(count, rt.Count- count);
        return rt;
    }

    public static int Clamp(int v, int minv, int maxv)
    {
        return v < minv ? minv : v > maxv ? maxv : v;
    }
    public static float Clamp(float v, float minv, float maxv)
    {
        return v < minv ? minv : v > maxv ? maxv : v;
    }

    /// <summary>
    /// 产生一个随机数
    /// </summary>
    /// <returns></returns>
    public static bool RandBool()
    {
        return UnityEngine.Random.Range(0, 1.0f) > 0.5f;
    }
}
