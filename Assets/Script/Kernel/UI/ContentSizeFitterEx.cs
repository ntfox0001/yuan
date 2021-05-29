using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class ContentSizeFitterEx : UIBehaviour, ILayoutSelfController
{
    bool mDirty = false;
    public enum FitMode
    {
        Unconstrained,
        MinSize,
        PreferredSize
    }

    [SerializeField] protected FitMode m_HorizontalFit = FitMode.Unconstrained;
    public FitMode horizontalFit { get { return m_HorizontalFit; } set { if (SetStruct(ref m_HorizontalFit, value)) SetDirty(); } }

    [SerializeField] protected FitMode m_VerticalFit = FitMode.Unconstrained;
    public FitMode verticalFit { get { return m_VerticalFit; } set { if (SetStruct(ref m_VerticalFit, value)) SetDirty(); } }

    [System.NonSerialized] private RectTransform m_Rect;
    private RectTransform rectTransform
    {
        get
        {
            if (m_Rect == null)
                m_Rect = GetComponent<RectTransform>();
            return m_Rect;
        }
    }

    private DrivenRectTransformTracker m_Tracker;

    protected ContentSizeFitterEx()
    { }

    protected override void OnEnable()
    {
        base.OnEnable();
        SetDirty();
    }

    protected override void OnDisable()
    {
        m_Tracker.Clear();
        LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        base.OnDisable();
    }

    protected override void OnRectTransformDimensionsChange()
    {
        Debug.Log(gameObject.name);
        SetDirty();
    }

    private void HandleSelfFittingAlongAxis(int axis)
    {
        FitMode fitting = (axis == 0 ? horizontalFit : verticalFit);
        if (fitting == FitMode.Unconstrained)
        {
            // Keep a reference to the tracked transform, but don't control its properties:
            m_Tracker.Add(this, rectTransform, DrivenTransformProperties.None);
            return;
        }

        m_Tracker.Add(this, rectTransform, (axis == 0 ? DrivenTransformProperties.SizeDeltaX : DrivenTransformProperties.SizeDeltaY));

        // Set size to min or preferred size
        if (fitting == FitMode.MinSize)
            rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, LayoutUtility.GetMinSize(m_Rect, axis));
        else
            rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, LayoutUtility.GetPreferredSize(m_Rect, axis));
    }

    public virtual void SetLayoutHorizontal()
    {
        m_Tracker.Clear();
        HandleSelfFittingAlongAxis(0);
    }

    public virtual void SetLayoutVertical()
    {
        HandleSelfFittingAlongAxis(1);
    }

    protected void SetDirty()
    {
        if (!IsActive())
            return;

        mDirty = true;
        LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
    }

    

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        SetDirty();
    }

#endif
    public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
    {
        if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
            return false;

        currentValue = newValue;
        return true;
    }
}

