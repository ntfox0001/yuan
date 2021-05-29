using UnityEngine;

#if UNITY_ANDROID && !UNITY_EDITOR
public class PlatformUtil : MonoBehaviour, IPlatformUtil
{
    public void RequestReview()
    {
        Debug.Log("RequestReview for default.");
    }
}
#endif