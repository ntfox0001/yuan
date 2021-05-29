using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragMutliMapScrollView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IInitializePotentialDragHandler, IScrollHandler
{
    public MapScrollView ParentScrollRect;
    public List<MapScrollView> ScrollRect;
    void Start()
    {
        if (ParentScrollRect == null)
        {
            ParentScrollRect = GetComponentInParent<MapScrollView>();
            ParentScrollRect.PreScale += ParentScrollRect_PreScale; ;
        }
    }

    private void ParentScrollRect_PreScale(float deltaScale, Vector2 screenPos, Vector2 localPos)
    {
        foreach (var sr in ScrollRect)
        {
            sr.ScaleContent(deltaScale, localPos);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (ParentScrollRect != null)
        {
            ExecuteEvents.Execute(ParentScrollRect.gameObject, eventData, ExecuteEvents.beginDragHandler);
        }
        foreach (var sr in ScrollRect)
        {
            ExecuteEvents.Execute(sr.gameObject, eventData, ExecuteEvents.beginDragHandler);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ParentScrollRect != null)
        {
            ExecuteEvents.Execute(ParentScrollRect.gameObject, eventData, ExecuteEvents.dragHandler);
        }
        foreach (var sr in ScrollRect)
        {
            ExecuteEvents.Execute(sr.gameObject, eventData, ExecuteEvents.dragHandler);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ParentScrollRect != null)
        {
            ExecuteEvents.Execute(ParentScrollRect.gameObject, eventData, ExecuteEvents.endDragHandler);
        }
        foreach (var sr in ScrollRect)
        {
            ExecuteEvents.Execute(sr.gameObject, eventData, ExecuteEvents.endDragHandler);
        }
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (ParentScrollRect != null)
        {
            ExecuteEvents.Execute(ParentScrollRect.gameObject, eventData, ExecuteEvents.initializePotentialDrag);
        }
        foreach (var sr in ScrollRect)
        {
            ExecuteEvents.Execute(sr.gameObject, eventData, ExecuteEvents.initializePotentialDrag);
        }
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (ParentScrollRect != null)
        {
            ExecuteEvents.Execute(ParentScrollRect.gameObject, eventData, ExecuteEvents.scrollHandler);
        }
        foreach (var sr in ScrollRect)
        {
            ExecuteEvents.Execute(sr.gameObject, SwitchEventData(sr, ParentScrollRect, eventData), ExecuteEvents.scrollHandler);
        }
    }

    PointerEventData SwitchEventData(ScrollViewBase dest, ScrollViewBase src, PointerEventData eventData)
    {
        var dr = dest.transform as RectTransform;
        var sr = src.transform as RectTransform;

        var deltaPos = dr.position - sr.position;

        var destData = new PointerEventData(null);
        destData.button = eventData.button;
        destData.clickCount = eventData.clickCount;
        destData.clickTime = eventData.clickTime;
        destData.delta = eventData.delta;
        destData.dragging = eventData.dragging;
        destData.eligibleForClick = eventData.eligibleForClick;
        destData.pointerCurrentRaycast = eventData.pointerCurrentRaycast;
        destData.pointerPressRaycast = eventData.pointerPressRaycast;
        // 修改
        destData.position = eventData.position + new Vector2(deltaPos.x, deltaPos.y);

        destData.pressPosition = eventData.pressPosition;
        destData.scrollDelta = eventData.scrollDelta;
        destData.useDragThreshold = eventData.useDragThreshold;

        return destData;
    }
}
