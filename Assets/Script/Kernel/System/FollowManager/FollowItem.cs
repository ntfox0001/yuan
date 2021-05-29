using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowItem : MonoBehaviour
{
    public Transform Target;
    public bool FollowPosX = false;
    public bool FollowPosY = false;
    public bool FollowPosZ = false;

    public bool FollowRotation = false;
    public bool FollowScale = false;

    /// <summary>
    /// 当target是父物体，但是不希望当前物体随着父物体缩放时使用
    /// </summary>
    public bool FollowUnscale = false;

    
    private void Awake()
    {
        if (Target != null)
        {
            FollowManager.GetSingleton().RegisterItem(this);
        }
    }
}
