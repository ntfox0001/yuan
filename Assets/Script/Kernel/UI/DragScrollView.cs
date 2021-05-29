using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragScrollView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IInitializePotentialDragHandler, IScrollHandler
{
    public UnityEngine.UI.ScrollRect ScrollRect;
    void Start()
    {
        if (ScrollRect == null)
        {
            ScrollRect = GetComponentInParent<UnityEngine.UI.ScrollRect>();
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (ScrollRect != null)
        {
            //ScrollRect.OnBeginDrag(eventData);
            //eventData.pointerDrag = ScrollRect.gameObject;
            ExecuteEvents.Execute(ScrollRect.gameObject, eventData, ExecuteEvents.beginDragHandler);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ScrollRect != null)
        {
            //ScrollRect.OnDrag(eventData);
            //eventData.pointerDrag = ScrollRect.gameObject;
            ExecuteEvents.Execute(ScrollRect.gameObject, eventData, ExecuteEvents.dragHandler);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ScrollRect != null)
        {
            //ScrollRect.OnEndDrag(eventData);
            //eventData.pointerDrag = ScrollRect.gameObject;
            ExecuteEvents.Execute(ScrollRect.gameObject, eventData, ExecuteEvents.endDragHandler);
        }
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (ScrollRect != null)
        {
            //ScrollRect.OnInitializePotentialDrag(eventData);
            //eventData.pointerDrag = ScrollRect.gameObject;
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
