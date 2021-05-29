using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class ClickToDestroyObject : MonoBehaviour
{
    public GameObject TargetObject;
    public float DelayTime = 0f;
    public UnityEvent onDestroy = null;
    // Use this for initialization
    void Start () 
    {
        DynamicEventGameObject.Get(gameObject).Click += ClickToDestroyObject_Click;
	}

    private void ClickToDestroyObject_Click(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (DelayTime <= 0)
        {
            DestroyTarget();
        }
        else
        {
            StartCoroutine(TimeUpDestroyCallBack(DelayTime));
        }
    }
    void DestroyTarget()
    {
        if (TargetObject == null)
        {
            Destroy(gameObject);
            TouchDestroy();
        }
        else
        {
            Destroy(TargetObject);
            TouchDestroy();
        }
    }
    void TouchDestroy()
    {
        if (onDestroy != null)
        {
            onDestroy.Invoke();
        }
    }
    IEnumerator TimeUpDestroyCallBack(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        DestroyTarget();
    }
}
