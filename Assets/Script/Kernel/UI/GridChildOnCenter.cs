using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// item居中或触发进入中线事件
/// 必须是GridLayoutGroup
/// todo：暂时还没实现居中效果
/// </summary>
[RequireComponent(typeof(GridLayoutGroup))]
public class GridChildOnCenter : MonoBehaviour
{
    
    public delegate void OnCenterHandler(Transform child);
    /// <summary>
    /// 当child完全对准中心时
    /// </summary>
    public event OnCenterHandler OnCenter;


    public delegate void OnEnterCenterHandler(Transform child);
    public event OnEnterCenterHandler EnterCenter;

    public ScrollViewBase ScrollView;
    public GridLayoutGroup Grid;

    public bool EnableScale = false;
    public float ScaleFrom = 1.0f;
    public float ScaleTo = 2.0f;
    public AnimationCurve ScaleCurve;

    public bool EnableAlpha = false;
    public float AlphaFrom = 0;
    public float AlphaTo = 1.0f;
    public AnimationCurve AlphaCurve;

    public bool Capture = false;
    public AnimationCurve CaptureCurve;
    public float CaptureSpeed = 12.0f; // 每秒移动, 大于这个速度，就移动到下一个
    public float CaptureTime = 1.0f;
    float mDragSpeed = 0; // 本次滑动最后速度
                          // 开始捕获居中
                          //bool mBeginCapture = true;

    RectTransform mGridRect;
    Vector2 mPrePos = new Vector2(float.MaxValue, float.MaxValue);

    CanvasGroup[] mCanvasGroupChildCache;

    int mCurrentChild = -1;
    float mCurrentDistence = 0;

    // 开始一个用户设置的居中对象
    bool mBeginUserCustomCapture = false;
    float mUserCustomTargetDistance = 0;
    float mUserCustomTime = 0;
    float mUserCustomFactor = 0;
    Vector2 mUserCustomOrigin;
    void Start()
    {
        mGridRect = Grid.transform as RectTransform;
        //mPrePos = mGridRect.anchoredPosition;

        SetupPadding();
        if (EnableAlpha)
        {
            mCanvasGroupChildCache = new CanvasGroup[Grid.transform.childCount];
            for (int i = 0; i < Grid.transform.childCount; i++)
            {
                mCanvasGroupChildCache[i] = Grid.transform.GetChild(i).GetComponent<CanvasGroup>();
            }
        }

        if (ScrollView.horizontal || ScrollView.vertical)
        {
            DynamicEventGameObject.Get(ScrollView.gameObject).InitializePotentialDrag += ScrollViewInitializePotentialDrag;
            DynamicEventGameObject.Get(ScrollView.gameObject).BeginDrag += ScrollViewBeginDrag;
            DynamicEventGameObject.Get(ScrollView.gameObject).Drag += ScrollViewDrag;
            DynamicEventGameObject.Get(ScrollView.gameObject).EndDrag += ScrollViewEndDrag;
            DynamicEventGameObject.Get(ScrollView.gameObject).Scroll += ScrollViewScroll; ;
        }
    
        if (Capture)
        {
            ScrollView.inertia = false;
            if (CaptureCurve == null)
            {
                CaptureCurve = new AnimationCurve();
            }
        }
        Refresh();
    }

    private void ScrollViewScroll(UnityEngine.EventSystems.PointerEventData eventData)
    {
        // 触发移动
        if (Grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            // 竖着
            SetChildToCenter(mCurrentChild + (int)Mathf.Sign(eventData.scrollDelta.y) * 1, CaptureTime);
        }
        else
        {
            SetChildToCenter(mCurrentChild - (int)Mathf.Sign(eventData.scrollDelta.y) * 1, CaptureTime);
        }
    }

    private void ScrollViewInitializePotentialDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        //mBeginCapture = false;
        mBeginUserCustomCapture = false;
        mDragSpeed = 0;
    }

    private void ScrollViewEndDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        //mBeginCapture = true;
        //Debug.Log(mDragSpeed);
        if (Mathf.Abs(mDragSpeed) > CaptureSpeed)
        {
            // 触发移动
            if (Grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
            {
                // 竖着
                SetChildToCenter(mCurrentChild + (int)Mathf.Sign(mDragSpeed) *  1, CaptureTime);
            }
            else
            {
                SetChildToCenter(mCurrentChild - (int)Mathf.Sign(mDragSpeed) * 1, CaptureTime);
            }
        }
        else
        {
            SetChildToCenter(mCurrentChild, CaptureTime);
        }
    }

    private void ScrollViewBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {

        

    }

    private void ScrollViewDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        float dim = 0;
        if (Grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            dim = eventData.delta.y;
        }
        else
        {
            dim = eventData.delta.x;
        }

        mDragSpeed = dim;
    }

    int ItemCount { get { return Grid.transform.childCount;} }

    public int CurrentChild
    {
        get
        {
            return mCurrentChild;
        }

        set
        {
            mCurrentChild = value;
        }
    }

    /// <summary>
    /// 计算距离，有正负
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    Vector2 CalculateItemDisByCenter(int row, int col)
    {
        // scroll中心相对于grid中心的位置
        var center = new Vector2(mGridRect.anchoredPosition.x, -mGridRect.anchoredPosition.y);
        var padding = new Vector2(Grid.padding.left, Grid.padding.top);
        // item相对左上的位置
        var itemPos = padding + new Vector2(Grid.cellSize.x * row, Grid.cellSize.y * col) + Grid.cellSize * 0.5f + new Vector2(Grid.spacing.x * row, Grid.spacing.y * col);
        // item相对中心的位置
        var itemPosByCenter = mGridRect.rect.size * 0.5f - itemPos;
        // item与scroll中心的距离
        return itemPosByCenter - center;
    }
    float CalculateItemDisByCenter(int index)
    {
        float dis;
        if (Grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            dis = CalculateItemDisByCenter(0, index).y;
        }
        else
        {
            dis = CalculateItemDisByCenter(index, 0).x;
        }
        return dis;
    }
    /// <summary>
    /// 设置指定child到中间
    /// </summary>
    /// <param name="child"></param>
    public void SetChildToCenter(int child)
    {
        if (Grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            mGridRect.anchoredPosition -= CalculateItemDisByCenter(0, child);
        }
        else
        {
            mGridRect.anchoredPosition += CalculateItemDisByCenter(child, 0);
        }
    }
    public void SetChildToCenter(int child, float time)
    {
        if (Grid.transform.childCount <= child)
        {
            child = Grid.transform.childCount - 1;
        }
        else if (child < 0)
        {
            child = 0;
        }
        mBeginUserCustomCapture = true;

        if (Grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            mUserCustomTargetDistance = CalculateItemDisByCenter(0, child).y;
        }
        else
        {
            mUserCustomTargetDistance = CalculateItemDisByCenter(child, 0).x;
        }
        mUserCustomOrigin = mGridRect.anchoredPosition;
        mUserCustomTime = time;
        mUserCustomFactor = 0;
    }
    float GetDimension()
    {
        if (Grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            return (ScrollView.transform as RectTransform).rect.height;
        }
        else
        {
            return (ScrollView.transform as RectTransform).rect.width;
        }
    }
    public void Refresh()
    {
        int minChild = -1;
        float minDis = float.MaxValue;
        for (int i = 0; i < Grid.transform.childCount; i++)
        {
            var child = Grid.transform.GetChild(i);
            float dis = CalculateItemDisByCenter(child.GetSiblingIndex());
            
            float absDis = Mathf.Abs(dis);
            if (absDis < Mathf.Abs(minDis))
            {
                minDis = dis;
                minChild = child.GetSiblingIndex();
            }
            if (EnableScale)
            {
                float f = 1.0f - Mathf.Clamp01(absDis / (GetDimension() * 0.5f));
                float r = ScaleCurve.Evaluate(f);
                float scale = r * (ScaleTo - ScaleFrom) + ScaleFrom;
                child.transform.localScale = new Vector3(scale, scale, 0);
            }
            if (EnableAlpha)
            {
                float f = 1.0f - Mathf.Clamp01(absDis / (GetDimension() * 0.5f));
                float r = AlphaCurve.Evaluate(f);
                float alpha = r * (AlphaTo - AlphaFrom) + AlphaFrom;
                mCanvasGroupChildCache[i].alpha = alpha;
            }
                
        }
        // mindis这里带符号
        mCurrentDistence = minDis;
        if (mCurrentChild != minChild)
        {
            mCurrentChild = minChild;
            if (EnterCenter != null)
            {
                EnterCenter(Grid.transform.GetChild(mCurrentChild));
            }
        }

    }
    //void UpdateCenter()
    //{
    //    if (Capture && mBeginCapture)
    //    {
    //        float deltaSpeed = GetDimension() * CaptureSpeed * Time.deltaTime;
    //        if (Mathf.Abs(mCurrentDistence) < deltaSpeed)
    //        {
    //            deltaSpeed = mCurrentDistence;
    //            mBeginCapture = false;
    //            if (OnCenter != null)
    //            {
    //                OnCenter(Grid.transform.GetChild(mCurrentChild));
    //            }
    //            ScrollView.velocity = Vector2.zero;
    //        }
    //        deltaSpeed *= Mathf.Sign(mCurrentDistence);
    //        Vector2 pos = mGridRect.anchoredPosition;
    //        if (Grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
    //        {
    //            pos.y -= deltaSpeed;
    //        }
    //        else
    //        {
    //            pos.x += deltaSpeed;
    //        }
    //        mGridRect.anchoredPosition = pos;

    //    }
    //}

    public void Update()
    {
        if (mBeginUserCustomCapture)
        {
            mUserCustomFactor += Time.deltaTime;

            float factor = 1.0f;
            if (mUserCustomTime != 0)
            {
                factor = mUserCustomFactor / mUserCustomTime;
            }
            if (mUserCustomFactor >= mUserCustomTime)
            {
                // 结束
                mUserCustomFactor = mUserCustomTime;
                mBeginUserCustomCapture = false;

                float dis = CaptureCurve.Evaluate(factor) * mUserCustomTargetDistance;

                if (Grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
                {
                    mGridRect.anchoredPosition = mUserCustomOrigin - new Vector2(0, dis);
                }
                else
                {
                    mGridRect.anchoredPosition = mUserCustomOrigin + new Vector2(dis, 0);
                }
                if (OnCenter != null)
                {
                    OnCenter(Grid.transform.GetChild(mCurrentChild));
                }
            }
            else
            {

                float dis = CaptureCurve.Evaluate(factor) * mUserCustomTargetDistance;

                if (Grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
                {
                    mGridRect.anchoredPosition = mUserCustomOrigin - new Vector2(0, dis);
                }
                else
                {
                    mGridRect.anchoredPosition = mUserCustomOrigin + new Vector2(dis, 0);
                }
            }

            if (mPrePos != mGridRect.anchoredPosition)
            {
                Refresh();
                mPrePos = mGridRect.anchoredPosition;
            }

        }
        else
        {
            if (mPrePos != mGridRect.anchoredPosition)
            {
                Refresh();
                mPrePos = mGridRect.anchoredPosition;
            }

            //if (mBeginCapture)
            //{
            //    UpdateCenter();
            //}
        }
    }

    public void SetupPadding()
    {
        float len = DeviceOrientation.ScreenOrientation == ScreenOrientation.Portrait ? UIUtility.UIReferenceHeight : UIUtility.UIReferenceWidth;
        //float space = DeviceOrientation.ScreenOrientation == ScreenOrientation.Portrait ? Grid.spacing.y : Grid.spacing.x;
        float cellSize = DeviceOrientation.ScreenOrientation == ScreenOrientation.Portrait ? Grid.cellSize.y : Grid.cellSize.x;
        int padding = (int)(len * 0.5f - cellSize * 0.5f);
        Grid.padding.left = padding;
        Grid.padding.top = padding;
        Grid.padding.right = padding;
        Grid.padding.bottom = padding;
    }
}
