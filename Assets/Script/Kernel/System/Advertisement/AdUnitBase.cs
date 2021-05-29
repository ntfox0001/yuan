using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AdUnitBase
{
    public virtual int Id { get; protected set; }
    public virtual int Type { get; protected set; }
    public virtual string CodeId { get; protected set; }
    public virtual string UserId { get; protected set; }
    public virtual bool IsReady { get; protected set; }
    public virtual bool HasError { get; protected set; }
    public virtual float BuildTime { get; protected set; }

    protected IAdInteractionListener mListener;
    public virtual float HoldTime
    {
        get
        {
            if (IsReady)
            {
                return Time.unscaledTime - BuildTime;
            }
            return 0;

        }
    }
    public virtual void Initial(IAdInteractionListener listener, string userId, string codeId, int type, int id, params object[] param)
    {
        Id = id;
        Type = type;
        CodeId = codeId;
        UserId = userId;
        mListener = listener;
        IsReady = false;
        HasError = false;
        BuildTime = 0;
    }
    public abstract bool Show();

    public abstract void Release();
}
