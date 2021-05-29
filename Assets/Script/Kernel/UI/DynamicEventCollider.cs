
using UnityEngine;
using UnityEngine.EventSystems;

// 将时间上传到父DynamicEventGameObject中
[RequireComponent(typeof(ClickArea))]
public class DynamicEventCollider : EventTrigger
{
    DynamicEventGameObject mDynamicEventGameObject;
    private void Awake()
    {
        mDynamicEventGameObject = GetComponentInParent<DynamicEventGameObject>();
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        ExecuteEvents.Execute(mDynamicEventGameObject.gameObject, eventData, ExecuteEvents.beginDragHandler);
    }
    public override void OnCancel(BaseEventData eventData)
    {
        ExecuteEvents.Execute(mDynamicEventGameObject.gameObject, eventData, ExecuteEvents.cancelHandler);
    }
    public override void OnDeselect(BaseEventData eventData)
    {
        ExecuteEvents.Execute(mDynamicEventGameObject.gameObject, eventData, ExecuteEvents.deselectHandler);
    }
    public override void OnDrag(PointerEventData eventData)
    {
        ExecuteEvents.Execute(mDynamicEventGameObject.gameObject, eventData, ExecuteEvents.dragHandler);
    }
    public override void OnDrop(PointerEventData eventData)
    {
        ExecuteEvents.Execute(mDynamicEventGameObject.gameObject, eventData, ExecuteEvents.dropHandler);
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        ExecuteEvents.Execute(mDynamicEventGameObject.gameObject, eventData, ExecuteEvents.endDragHandler);
    }
    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        ExecuteEvents.Execute(mDynamicEventGameObject.gameObject, eventData, ExecuteEvents.initializePotentialDrag);
    }
    public override void OnMove(AxisEventData eventData)
    {
        ExecuteEvents.Execute(mDynamicEventGameObject.gameObject, eventData, ExecuteEvents.moveHandler);
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        ExecuteEvents.Execute(mDynamicEventGameObject.gameObject, eventData, ExecuteEvents.pointerClickHandler);
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        ExecuteEvents.Execute(mDynamicEventGameObject.gameObject, eventData, ExecuteEvents.pointerDownHandler);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        ExecuteEvents.Execute(mDynamicEventGameObject.gameObject, eventData, ExecuteEvents.pointerEnterHandler);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        ExecuteEvents.Execute(mDynamicEventGameObject.gameObject, eventData, ExecuteEvents.pointerExitHandler);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        ExecuteEvents.Execute(mDynamicEventGameObject.gameObject, eventData, ExecuteEvents.pointerUpHandler);
    }
    public override void OnScroll(PointerEventData eventData)
    {
        ExecuteEvents.Execute(mDynamicEventGameObject.gameObject, eventData, ExecuteEvents.scrollHandler);
    }
    public override void OnSelect(BaseEventData eventData)
    {
        ExecuteEvents.Execute(mDynamicEventGameObject.gameObject, eventData, ExecuteEvents.selectHandler);
    }
    public override void OnSubmit(BaseEventData eventData)
    {
        ExecuteEvents.Execute(mDynamicEventGameObject.gameObject, eventData, ExecuteEvents.submitHandler);
    }
    public override void OnUpdateSelected(BaseEventData eventData)
    {
        ExecuteEvents.Execute(mDynamicEventGameObject.gameObject, eventData, ExecuteEvents.updateSelectedHandler);
    }
}
