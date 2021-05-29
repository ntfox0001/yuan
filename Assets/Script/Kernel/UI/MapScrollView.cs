using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapScrollView : ScrollViewBase
{
    public bool DisableScale
    {
        get { return mDisableScale; }
        set { mDisableScale = value; }
    }
    public float MouseScaleRatio = 0.05f;
    public float TouchScaleRatio = 0.05f;
    public float MaxScaleRatio = 2.0f;
    public float MinScaleRatio = 0.3f;
    public float MaxScaleLimit = 2.5f;
    public float MinScaleLimit = 0.2f;
    public float ScaleDuration = 0.3f;

    public float ScreenMathRatio = 1.0f;
    public float ScreenMathBase = 1080f;

    public delegate void PreScaleHander(float deltaScale, Vector2 screenPos, Vector2 localPos);
    public delegate void ScaleHandler();
    
    /// <summary>
    /// 当update中，触发scale之前调用, centerPos为屏幕坐标
    /// </summary>
    public event PreScaleHander PreScale;

    /// <summary>
    /// 当缩放时，触发
    /// </summary>
    public event ScaleHandler Scale;
    

    uTools.TweenScale mTweenScale;
    bool mDisableScale = false;

    int mTouchCount = 0;
    bool mBeginDrag = false;
    protected override void Start()
    {
        mTweenScale = content.GetComponent<uTools.TweenScale>();
        if (mTweenScale == null)
        {
            mTweenScale = content.gameObject.AddComponent<uTools.TweenScale>();
        }
    }
    protected override void LateUpdate()
    {
        if (Input.touchCount == 2 && !DisableScale)
        {
            if (mTouchCount == 1)
            {
                PointerEventData peData = new PointerEventData(GlobalObjects.GetSingleton().EventySystem);
                base.OnEndDrag(peData);
                base.OnInitializePotentialDrag(peData);
            }
            // 滑动
            if (Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Stationary ||
                Input.GetTouch(0).phase == TouchPhase.Stationary && Input.GetTouch(1).phase == TouchPhase.Moved ||
                Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved)
            {
                Touch t0 = Input.GetTouch(0);
                Touch t1 = Input.GetTouch(1);

                //获得上一帧位置
                Vector2 prePos0 = t0.position + t0.deltaPosition;
                Vector2 prePos1 = t1.position + t1.deltaPosition;

                Vector2 proj0 = MathUtility.GetProjectOfPointToLine(t0.position, prePos0, prePos1);
                Vector2 proj1 = MathUtility.GetProjectOfPointToLine(t1.position, prePos0, prePos1);

                float distence0 = Vector2.Distance(prePos0, prePos1);
                float distence1 = Vector2.Distance(proj0, proj1);

                float scale = (distence0 - distence1) * TouchScaleRatio * Time.deltaTime;

                Vector2 screenCenterPos = (prePos0 - prePos1) * 0.5f + prePos1;
                Vector2 centerPos;
                ScreenPointToLocalPointInRectangle(content, screenCenterPos, out centerPos);
                if (PreScale != null)
                {
                    PreScale(scale, screenCenterPos, centerPos);
                }
                ScaleContent(scale, centerPos);
            }
        }

        mTouchCount = Input.touchCount;
        base.LateUpdate();
    }
    void ScreenPointToLocalPointInRectangle(RectTransform rect, Vector2 screenPoint, out Vector2 localPoint)
    {
        Canvas root = rect.root.GetComponent<Canvas>();
        if (root.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPoint, null, out localPoint);
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPoint, root.worldCamera, out localPoint);
        }
        
    }


    // 滚动轴用于缩放
    public override void OnScroll(PointerEventData data)
    {
        float deltaScale = data.scrollDelta.y * MouseScaleRatio;
        Vector2 pos;
        ScreenPointToLocalPointInRectangle(content, data.position, out pos);

        ScaleContent(deltaScale, pos);
    }
    /// <summary>
    /// 缩放内容，超过max min，会自动回弹
    /// </summary>
    /// <param name="scale"></param>
    /// <param name="centerPos">缩放中心点</param>
    public void ScaleContent(float deltaScale, Vector2 centerPos)
    {
        mTweenScale.enabled = false;
        float scale = content.localScale.x * (1 + deltaScale);
        scale = scale > MaxScaleLimit ? MaxScaleLimit : scale < MinScaleLimit ? MinScaleLimit : scale;
        Vector2 newPivot = new Vector2((content.rect.width * content.pivot.x + centerPos.x) / content.rect.width,
                                (content.rect.height * content.pivot.y + centerPos.y) / content.rect.height);
        Vector4 wpos = content.localToWorldMatrix * new Vector4(centerPos.x, centerPos.y, 0, 0);
        Vector4 ppos = transform.worldToLocalMatrix * wpos;

        content.pivot = newPivot;
        content.localPosition = content.localPosition + new Vector3(ppos.x, ppos.y);
        if (mTweenScale == null)
        {
            scale = scale > MaxScaleRatio ? MaxScaleRatio : scale < MinScaleRatio ? MinScaleRatio : scale;
            content.localScale = new Vector3(scale, scale, scale);
        }
        else
        {
            float targetScale = 0;
            if (scale > MaxScaleRatio)
            {
                targetScale = MaxScaleRatio;
            }
            else if (scale < MinScaleRatio)
            {
                targetScale = MinScaleRatio;
            }
            else
            {
                targetScale = scale;
                content.localScale = new Vector3(targetScale, targetScale, 1);
                if (Scale != null)
                {
                    Scale();
                }
                return;
            }
            uTools.TweenScale.Begin(mTweenScale.gameObject, new Vector3(scale, scale, 1), new Vector3(targetScale, targetScale, 1), ScaleDuration, mTweenScale.delay);
        }

        if (Scale != null)
        {
            Scale();
        }
    }
    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (Input.touchCount == 2) return;

        //Debug.Log("OnInitializePotentialDrag");
        base.OnInitializePotentialDrag(eventData);
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (Input.touchCount == 2) return;

        mBeginDrag = true;
        //Debug.Log("OnBeginDrag");
        base.OnBeginDrag(eventData);
    }
    public override void OnDrag(PointerEventData eventData)
    {
        if (Input.touchCount == 2) return;
        
        //Debug.Log("OnDrag delta" + eventData.delta.ToString() + "  pos:" + eventData.position);
        base.OnDrag(eventData);
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        if (Input.touchCount == 2) return;
        mBeginDrag = false;
        //Debug.Log("OnEndDrag");
        base.OnEndDrag(eventData);
    }
}
