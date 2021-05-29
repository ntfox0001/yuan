using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragMapScrollView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IInitializePotentialDragHandler, IScrollHandler
{
    public ScrollViewBase ScrollRect;
    void Start()
    {
        if (ScrollRect == null)
        {
            ScrollRect = GetComponentInParent<ScrollViewBase>();
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (ScrollRect != null)
        {
            //ScrollRect.OnBeginDrag(eventData);
            ExecuteEvents.Execute(ScrollRect.gameObject, eventData, ExecuteEvents.beginDragHandler);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ScrollRect != null)
        {
            //ScrollRect.OnDrag(eventData);
            ExecuteEvents.Execute(ScrollRect.gameObject, eventData, ExecuteEvents.dragHandler);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ScrollRect != null)
        {
            //ScrollRect.OnEndDrag(eventData);
            ExecuteEvents.Execute(ScrollRect.gameObject, eventData, ExecuteEvents.endDragHandler);
        }
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (ScrollRect != null)
        {
            //ScrollRect.OnInitializePotentialDrag(eventData);
            ExecuteEvents.Execute(ScrollRect.gameObject, eventData, ExecuteEvents.initializePotentialDrag);
        }
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (ScrollRect != null)
        {
            //ScrollRect.OnScroll(eventData);
            ExecuteEvents.Execute(ScrollRect.gameObject, eventData, ExecuteEvents.scrollHandler);
        }
    }
}
