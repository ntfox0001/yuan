using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 跟随目标
/// </summary>
public class FollowTarget : MonoBehaviour
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

    Vector3 mPrePosition;
    Quaternion mPreRotation;
    Vector3 mPreScale;
    
	// Use this for initialization
	void OnEnable ()
    {
        Initial();
    }
	void Initial()
    {
        if (Target == null) return;
        mPrePosition = Target.localPosition;
        mPreRotation = Target.localRotation;
        mPreScale = Target.localScale;
        if (FollowScale && FollowUnscale)
        {
            Debug.LogError("Both FollowScale and FollowUnscale is true on node:" + UIUtility.GetPath(transform));
        }
    }
    public void SetTarget(Transform tf)
    {
        Target = tf;
        Initial();
    }
	// Update is called once per frame
	void Update () {
        if (FollowPosX || FollowPosY || FollowPosZ)
        {
            var pos = Target.localPosition;
            var delta = new Vector3();
            if (FollowPosX && mPrePosition.x != pos.x)
            {
                delta.x = pos.x - mPrePosition.x;
            }
            if (FollowPosY && mPrePosition.y != pos.y)
            {
                delta.y = pos.y - mPrePosition.y;
            }
            if (FollowPosZ && mPrePosition.z != pos.z)
            {
                delta.z = pos.z - mPrePosition.z;
            }
            mPrePosition = pos;
            transform.localPosition += delta;
        }
        if (FollowRotation)
        {
            if (mPreRotation != Target.localRotation)
            {
                var delta = Target.localRotation * Quaternion.Inverse(mPreRotation);
                mPreRotation = Target.localRotation;
                transform.localRotation *= delta;
            }
        }
        if (FollowScale)
        {
            if (mPreScale != Target.localScale)
            {
                var delta = Target.localScale - mPreScale;
                mPreScale = Target.localScale;
                transform.localScale += delta;
            }
        }
        if (FollowUnscale)
        {
            if (mPreScale != Target.localScale)
            {
                Vector3 scale = Target.localScale;
                scale.x = 1.0f / scale.x;
                scale.y = 1.0f / scale.y;
                transform.localScale = scale;
                mPreScale = Target.localScale;
            }
        }
	}
}
