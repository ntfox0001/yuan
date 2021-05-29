using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowManager : Singleton<FollowManager>
{
    
    public class FollowInfo
    {
        public FollowInfo Target;

        public Transform Follower;
        public bool PosX = false;
        public bool PosY = false;
        public bool PosZ = false;

        public bool Rotation = false;
        public bool Scale = false;
        /// <summary>
        /// 当target是父物体，但是不希望当前物体随着父物体缩放时使用
        /// </summary>
        public bool FollowUnscale = false;

        public List<FollowInfo> Followers = new List<FollowInfo>();

        public FollowInfo(FollowItem fi)
        {
            Follower = fi.Target;
            PosX = fi.FollowPosX;
            PosY = fi.FollowPosY;
            PosZ = fi.FollowPosZ;
            Rotation = fi.FollowRotation;
            Scale = fi.FollowScale;
            FollowUnscale = fi.FollowUnscale;
        }

        // 查找路径下是否有指定的target
        public FollowInfo FindTarget(Transform tf)
        {
            if (Target.Follower == tf) return this;
            for (int i = 0; i < Followers.Count; i++)
            {
                var rt = Followers[i].FindTarget(tf);
                if (rt != null)
                {
                    return rt;
                }
            }
            return null;
        }

        public FollowInfo AddFollower(FollowItem fi)
        {
            var info = new FollowInfo(fi);
            info.Target = this;
            Followers.Add(info);
            return info;
        }
        public bool RemoveFollower(FollowInfo info)
        {
            if (info.Target == this)
            {
                Followers.Remove(info);
                return true;
            }
            else
            {
                for (int i = 0; i < Followers.Count; i++)
                {
                    if (Followers[i].RemoveFollower(info))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    List<FollowInfo> mFollowList = new List<FollowInfo>();
    public FollowInfo RegisterItem(FollowItem fi)
    {
        FollowInfo info;
        for (int i = 0; i < mFollowList.Count; i++)
        {
            info = mFollowList[i].FindTarget(fi.Target);
            if (info != null)
            {
                return info.AddFollower(fi);
            }
        }
        info = new FollowInfo(fi);
        mFollowList.Add(info);
        return info;
    }

    public void UnregisterItem(FollowInfo info)
    {
        if (info.Target == null)
        {
            mFollowList.Remove(info);
        }
        else
        {
            for (int i = 0; i < mFollowList.Count; i++)
            {
                if (mFollowList[i].RemoveFollower(info))
                {
                    return;
                }
            }
        }
    }
}
