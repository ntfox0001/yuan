using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class DynamicEventGameObject : EventTrigger
{
    // ugui的click在drag后仍然能被调用，所以这里挡一下
    bool mEligibleClick = false;
    float mEligibleClickDis = 0.0f;
    public delegate void PointerEventDataHandler(PointerEventData eventData);
    public delegate void BaseEventDataHandler(BaseEventData eventData);
    public delegate void AxisEventDataHandler(AxisEventData eventData);
    public event PointerEventDataHandler BeginDrag;
    public event BaseEventDataHandler Cancel;
    public event BaseEventDataHandler Deselect;
    public event PointerEventDataHandler Drag;
    public event PointerEventDataHandler Drop;
    public event PointerEventDataHandler EndDrag;
    public event PointerEventDataHandler InitializePotentialDrag;
    public event AxisEventDataHandler Move;
    public event PointerEventDataHandler Click;
    public event PointerEventDataHandler MouseDown;
    public event PointerEventDataHandler MouseUp;
    public event PointerEventDataHandler MouseEnter;
    public event PointerEventDataHandler MouseExit;
    public event PointerEventDataHandler Scroll;
    public event BaseEventDataHandler Select;
    public event BaseEventDataHandler Submit;
    public event BaseEventDataHandler UpdateSelected;

    public object UserData;
    static public DynamicEventGameObject Get(GameObject go)
    {
        DynamicEventGameObject dego = go.GetComponent<DynamicEventGameObject>();
        if (dego == null) dego = go.AddComponent<DynamicEventGameObject>();
        return dego;
    }
    static public DynamicEventGameObject Get(MonoBehaviour mb)
    {
        DynamicEventGameObject dego = mb.GetComponent<DynamicEventGameObject>();
        if (dego == null) dego = mb.gameObject.AddComponent<DynamicEventGameObject>();
        return dego;
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        DebugLog("OnBeginDrag");
        base.OnBeginDrag(eventData);
        if (BeginDrag != null) BeginDrag(eventData);
    }
    public override void OnCancel(BaseEventData eventData)
    {
        DebugLog("OnCancel");
        base.OnCancel(eventData);
        if (Cancel != null) Cancel(eventData);
    }
    public override void OnDeselect(BaseEventData eventData)
    {
        DebugLog("OnDeselect");
        base.OnDeselect(eventData);
        if (Deselect != null) Deselect(eventData);
    }
    public override void OnDrag(PointerEventData eventData)
    {
        mEligibleClickDis += eventData.delta.SqrMagnitude();
        if (mEligibleClickDis > UIUtility.SqrPixelDragThreshold)
        {
            mEligibleClick = false;
        }
        
        DebugLog("OnDrag");
        base.OnDrag(eventData);
        if (Drag != null) Drag(eventData);
    }
    public override void OnDrop(PointerEventData eventData)
    {
        DebugLog("OnDrop");
        base.OnDrop(eventData);
        if (Drop != null) Drop(eventData);
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        DebugLog("OnEndDrag");
        base.OnEndDrag(eventData);
        if (EndDrag != null) EndDrag(eventData);
    }
    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        DebugLog("OnInitializePotentialDrag");
        base.OnInitializePotentialDrag(eventData);
        if (InitializePotentialDrag != null) InitializePotentialDrag(eventData);
    }
    public override void OnMove(AxisEventData eventData)
    {
        DebugLog("OnMove");
        base.OnMove(eventData);
        if (Move != null) Move(eventData);
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (mEligibleClick)
        {
            DebugLog("OnPointerClick");
            base.OnPointerClick(eventData);
            if (Click != null) Click(eventData);
        }
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        mEligibleClick = true;
        mEligibleClickDis = 0.0f;
        DebugLog("OnPointerDown");
        base.OnPointerDown(eventData);
        if (MouseDown != null) MouseDown(eventData);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        DebugLog("OnPointerEnter");
        base.OnPointerEnter(eventData);
        if (MouseEnter != null) MouseEnter(eventData);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        DebugLog("OnPointerExit");
        base.OnPointerExit(eventData);
        if (MouseExit != null) MouseExit(eventData);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        DebugLog("OnPointerUp");
        base.OnPointerUp(eventData);
        if (MouseUp != null) MouseUp(eventData);
    }
    public override void OnScroll(PointerEventData eventData)
    {
        DebugLog("OnScroll");
        base.OnScroll(eventData);
        if (Scroll != null) Scroll(eventData);
    }
    public override void OnSelect(BaseEventData eventData)
    {
        DebugLog("OnSelect");
        base.OnSelect(eventData);
        if (Select != null) Select(eventData);
    }
    public override void OnSubmit(BaseEventData eventData)
    {
        DebugLog("OnSubmit");
        base.OnSubmit(eventData);
        if (Submit != null) Submit(eventData);
    }
    public override void OnUpdateSelected(BaseEventData eventData)
    {
        DebugLog("OnUpdateSelected");
        base.OnUpdateSelected(eventData);
        if (UpdateSelected != null) UpdateSelected(eventData);
    }

    void DebugLog(string eventName){
        //Debug.Log("go:" + name + " event:" + eventName );
    }
}
