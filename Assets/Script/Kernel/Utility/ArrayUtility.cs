using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayUtility
{
    public static T[] Insert<T>(T[] array, int id, T item)
    {
        List<T> list = new List<T>(array);
        list.Insert(id, item);
        return list.ToArray();
    }
    public static T[] Delete<T>(T[] array, int id)
    {
        List<T> list = new List<T>(array);
        list.RemoveAt(id);
        return list.ToArray();
    }
}
