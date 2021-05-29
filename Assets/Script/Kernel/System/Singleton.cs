using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T mThis;
    public Singleton()
    {
        mThis = (T)this;
    }

    public static T GetSingleton()
    {
        return mThis;
    }
    public void ReleaseSingleton()
    {
        mThis = null;
    }
}
