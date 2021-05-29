#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
using UnityEngine;

public class PlatformUtil : MonoBehaviour, IPlatformUtil
{

    [DllImport("__Internal")]
    private static extern void requestReview();
    public void RequestReview()
    {
        requestReview();
    }
}
#endif