using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTargetPosition : MonoBehaviour
{
    public enum FollowType
    {
        World,
        Local,
        LocalRect,
    }
    public FollowType Type = FollowType.LocalRect;
    public Transform Target;

    public float MoveRate = 1.0f;
    
    Vector3 mTargetOriginPos;
    Vector3 mMysefOriginPos;

    Vector2 mTargetOriginRectPos;
    Vector2 mMysefOriginRectPos;

    bool mUseRect = false;

    RectTransform mCacheTargetRectTransform;
    RectTransform mCacheMyselfRectTransform;
    // Use this for initialization
    void Awake () {
        if(Target != null)
        {
            switch (Type)
            {
                case FollowType.World:
                    {
                        mTargetOriginPos = Target.position;
                        mMysefOriginPos = transform.position;
                        break;
                    }
                case FollowType.Local:
                    {
                        mTargetOriginPos = Target.localPosition;
                        mMysefOriginPos = transform.localPosition;
                        break;
                    }
                case FollowType.LocalRect:
                    {
                        mCacheTargetRectTransform = Target as RectTransform;
                        mCacheMyselfRectTransform = transform as RectTransform;

                        mTargetOriginRectPos = mCacheTargetRectTransform.anchoredPosition;
                        mMysefOriginRectPos = mCacheMyselfRectTransform.anchoredPosition;
                        break;
                    }
            }
        }
    }
	
    /// <summary>
    /// 手动添加此脚本时调用
    /// </summary>
    /// <returns></returns>
    public void Initial()
    {
        if (Target != null)
        {
            switch (Type)
            {
                case FollowType.World:
                    {
                        mTargetOriginPos = Target.position;
                        mMysefOriginPos = transform.position;
                        break;
                    }
                case FollowType.Local:
                    {
                        mTargetOriginPos = Target.localPosition;
                        mMysefOriginPos = transform.localPosition;
                        break;
                    }
                case FollowType.LocalRect:
                    {
                        mCacheTargetRectTransform = Target as RectTransform;
                        mCacheMyselfRectTransform = transform as RectTransform;

                        mTargetOriginRectPos = mCacheTargetRectTransform.anchoredPosition;
                        mMysefOriginRectPos = mCacheMyselfRectTransform.anchoredPosition;
                        break;
                    }
            }
        }
         
    }
	// Update is called once per frame
	void Update ()
    {
        if (Target != null)
        {
            switch (Type)
            {
                case FollowType.World:
                    {
                        Vector3 move = Target.position - mTargetOriginPos;
                        transform.position = mMysefOriginPos + move * MoveRate;

                        break;
                    }
                case FollowType.Local:
                    {
                        Vector3 move = Target.localPosition - mTargetOriginPos;
                        transform.localPosition = mMysefOriginPos + move * MoveRate;

                        break;
                    }
                case FollowType.LocalRect:
                    {
                        Vector2 move = mCacheTargetRectTransform.anchoredPosition - mTargetOriginRectPos;
                        mCacheMyselfRectTransform.anchoredPosition = mMysefOriginRectPos + move * MoveRate;
                        break;
                    }
            }
        }
    }
}
