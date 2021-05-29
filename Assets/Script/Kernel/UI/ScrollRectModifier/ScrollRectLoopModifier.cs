using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Infinite scroll view with automatic configuration 
/// 
/// Fields
/// - InitByUSer - in case your scrollrect is populated from code, you can explicitly Initialize the infinite scroll after your scroll is ready
/// by callin Init() method
/// 
/// Notes
/// - doesn't work in both vertical and horizontal orientation at the same time.
/// - in order to work it disables layout components and size fitter if present(automatically)
/// 
/// </summary>
public class ScrollRectLoopModifier : MonoBehaviour
{
    enum ChildPos
    {
        First,Last,
    }
    public delegate void OnChildChangeHandler(int id, Transform item);
    public delegate void OnChildRefreshHandler(List<RectTransform> items);
    public event OnChildChangeHandler ChildChange;
    public event OnChildRefreshHandler ChildRefresh;

    [Tooltip("BigList 需要自定义Content的大小，并且设置好间距，一般是负的，只需要少量item能覆盖整个可见区域即可")]
    public bool BigList = false;
    [Tooltip("List count")]
    public int ListCount = 0;
    [Tooltip("Space for list")]
    public float Space = 0;
    //if true user will need to call Init() method manually (in case the contend of the scrollview is generated from code or requires special initialization)
    [Tooltip("If false, will Init automatically, otherwise you need to call Init() method")]
    public bool InitByUser = false;
    [Tooltip("超过阈值的item将被放到后面")]
    public float Treshold = 100f;

    private ScrollRect mScrollRect;
    private ContentSizeFitter mContentSizeFitter;
    private VerticalLayoutGroup mVerticalLayoutGroup;
    private HorizontalLayoutGroup mHorizontalLayoutGroup;
    private GridLayoutGroup mGridLayoutGroup;
    private bool mIsVertical = false;
    private bool mIsHorizontal = false;
    private float mDisableMarginX = 0;
    private float mDisableMarginY = 0;
    private bool mHasDisabledGridComponents = false;
    private List<RectTransform> mItems = new List<RectTransform>();
    private Vector2 mNewAnchoredPosition = Vector2.zero;
    //TO DISABLE FLICKERING OBJECT WHEN SCROLL VIEW IS IDLE IN BETWEEN OBJECTS
    
    private int mItemCount = 0;
    private float mRecordOffsetX = 0;
    private float mRecordOffsetY = 0;

    private int mTopIndex = 0;
    private int mEndIndex = 0;

    private bool mInitial = false;
    void Awake()
    {
        if (!InitByUser)
            Init();
    }

    /// <summary>
    /// 使用外部参数动态初始化
    /// </summary>
    /// <param name="bigList">true 使用大列表，false则为无限循环</param>
    /// <param name="items">外部创建的item列表，实际创建的item，循环使用，这些列表应该至少覆盖整个scrollrect并且再多出30%</param>
    /// <param name="listCount">当使用大列表时，表示虚拟多少个item</param>
    /// <param name="space">每个item之间的间隔</param>
    /// <param name="treshold">当item离开scrollrect多少距离时，即可重复利用</param>
    public void Init(bool bigList, List<RectTransform> items, int listCount, float space, float treshold)
    {
        if (mInitial == true)
        {
            Release();
        }
        BigList = bigList;
        ListCount = listCount;
        Space = space;
        Treshold = treshold;
        mItems = items;
        Init();
    }
    /// <summary>
    /// 动态控制scrollrect时，释放控制
    /// </summary>
    public void Release()
    {
        if (mScrollRect != null)
            mScrollRect.onValueChanged.RemoveAllListeners();


        mItems.Clear();
        EnableGridComponents();
        mHasDisabledGridComponents = false;
    }
    public void Init()
    {
        mInitial = true;
        if (GetComponent<ScrollRect>() != null)
        {
            mScrollRect = GetComponent<ScrollRect>();
            mScrollRect.onValueChanged.AddListener(OnScroll);
            mScrollRect.movementType = BigList ? ScrollRect.MovementType.Elastic : ScrollRect.MovementType.Unrestricted;

            // 如果items是空的，那么自己获取child
            if (mItems.Count == 0)
            {
                for (int i = 0; i < mScrollRect.content.childCount; i++)
                {
                    mItems.Add(mScrollRect.content.GetChild(i).GetComponent<RectTransform>());
                }
            }
            if (mScrollRect.content.GetComponent<VerticalLayoutGroup>() != null)
            {
                mVerticalLayoutGroup = mScrollRect.content.GetComponent<VerticalLayoutGroup>();
            }
            if (mScrollRect.content.GetComponent<HorizontalLayoutGroup>() != null)
            {
                mHorizontalLayoutGroup = mScrollRect.content.GetComponent<HorizontalLayoutGroup>();
            }
            if (mScrollRect.content.GetComponent<GridLayoutGroup>() != null)
            {
                mGridLayoutGroup = mScrollRect.content.GetComponent<GridLayoutGroup>();
            }
            if (mScrollRect.content.GetComponent<ContentSizeFitter>() != null)
            {
                mContentSizeFitter = mScrollRect.content.GetComponent<ContentSizeFitter>();
            }


            mIsHorizontal = mScrollRect.horizontal;
            mIsVertical = mScrollRect.vertical;

            if (mIsHorizontal && mIsVertical)
            {
                Debug.LogError("UI_InfiniteScroll doesn't support scrolling in both directions, plase choose one direction (horizontal or vertical)");
            }
            var rect = mScrollRect.content.transform as RectTransform;
            if (mIsHorizontal)
            {
                var pos = rect.anchoredPosition;
                pos.x = 0;
                rect.anchoredPosition = pos;
            }
            else if (mIsVertical)
            {
                var pos = rect.anchoredPosition;
                pos.y = 0;
                rect.anchoredPosition = pos;
            }

            mItemCount = mItems.Count;
            mTopIndex = 0;
            mEndIndex = mItemCount - 1;
            SetContentSize();
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(DelaySetContentSize());
            }
        }
        else
        {
            Debug.LogError("UI_InfiniteScroll => No ScrollRect component found");
        }
    }
    IEnumerator DelaySetContentSize()
    {
        yield return null;
        SetContentSize();
        Refresh();
    }
    /// <summary>
    /// 刷新现有所有item
    /// </summary>
    public void Refresh()
    {
        // 调用change 刷新每一个child
        //for (int i = 0; i < mItems.Count; i++)
        //{
        //    TouchChildChange(i, mItems[i]);
        //}
        if (ChildRefresh != null)
        {
            ChildRefresh(mItems);
        }
    }
    void DisableGridComponents()
    {
        if (mItems.Count > 1)
        {
            if (mIsVertical)
            {
                if (mGridLayoutGroup && mGridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
                {
                    mRecordOffsetY = (mItems[0].GetComponent<RectTransform>().anchoredPosition.y - mItems[mGridLayoutGroup.constraintCount].GetComponent<RectTransform>().anchoredPosition.y) / mGridLayoutGroup.constraintCount;
                    mDisableMarginY = mRecordOffsetY * mItemCount / 2;// _scrollRect.GetComponent<RectTransform>().rect.height/2 + items[0].sizeDelta.y;
                }
                else
                {
                    mRecordOffsetY = mItems[0].GetComponent<RectTransform>().anchoredPosition.y - mItems[1].GetComponent<RectTransform>().anchoredPosition.y;
                    mDisableMarginY = mRecordOffsetY * mItemCount / 2;// _scrollRect.GetComponent<RectTransform>().rect.height/2 + items[0].sizeDelta.y;
                }
            }
            if (mIsHorizontal)
            {
                if (mGridLayoutGroup && mGridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedRowCount)
                {
                    mRecordOffsetX = (mItems[mGridLayoutGroup.constraintCount].GetComponent<RectTransform>().anchoredPosition.x - mItems[0].GetComponent<RectTransform>().anchoredPosition.x) / mGridLayoutGroup.constraintCount;
                    mDisableMarginX = mRecordOffsetX * mItemCount / 2;//_scrollRect.GetComponent<RectTransform>().rect.width/2 + items[0].sizeDelta.x;
                }
                else
                {
                    mRecordOffsetX = mItems[1].GetComponent<RectTransform>().anchoredPosition.x - mItems[0].GetComponent<RectTransform>().anchoredPosition.x;
                    mDisableMarginX = mRecordOffsetX * mItemCount / 2;//_scrollRect.GetComponent<RectTransform>().rect.width/2 + items[0].sizeDelta.x;
                }
            }
        }
        

        if (mVerticalLayoutGroup)
        {
            mVerticalLayoutGroup.enabled = false;
        }
        if (mHorizontalLayoutGroup)
        {
            mHorizontalLayoutGroup.enabled = false;
        }
        if (mContentSizeFitter)
        {
            mContentSizeFitter.enabled = false;
        }
        if (mGridLayoutGroup)
        {
            mGridLayoutGroup.enabled = false;
        }
        
        mHasDisabledGridComponents = true;
    }
    void EnableGridComponents()
    {
        if (mVerticalLayoutGroup)
        {
            mVerticalLayoutGroup.enabled = true;
        }
        if (mHorizontalLayoutGroup)
        {
            mHorizontalLayoutGroup.enabled = true;
        }
        if (mContentSizeFitter)
        {
            mContentSizeFitter.enabled = true;
        }
        if (mGridLayoutGroup)
        {
            mGridLayoutGroup.enabled = true;
        }

        mHasDisabledGridComponents = true;
    }

    public void OnScroll(Vector2 pos)
    {
        if (!mHasDisabledGridComponents)
            DisableGridComponents();

        if (BigList)
        {
            if (mIsHorizontal)
            {
                if (pos.x >= 1.0f || pos.x <= 0)
                {
                    return;
                }
            }
            if (mIsVertical)
            {
                if (pos.y >= 1.0f || pos.y <= 0)
                {
                    return;
                }
            }
        }

        for (int i = 0; i < mItems.Count; i++)
        {
            if (mIsHorizontal)
            {
                if (mScrollRect.transform.InverseTransformPoint(mItems[i].gameObject.transform.position).x > mDisableMarginX + Treshold)
                {

                    if (TouchPreChildChange(ChildPos.First, mItems[i]))
                    {
                        mNewAnchoredPosition = mItems[i].anchoredPosition;
                        mNewAnchoredPosition.x -= mItemCount * mRecordOffsetX;
                        mItems[i].anchoredPosition = mNewAnchoredPosition;
                        mItems[i].SetAsLastSibling();
                        TouchChildChange(mEndIndex, mItems[i]);
                    }

                }
                else if (mScrollRect.transform.InverseTransformPoint(mItems[mItems.Count - 1 - i].gameObject.transform.position).x < -mDisableMarginX)
                {
                    if (TouchPreChildChange(ChildPos.Last, mItems[mItems.Count - 1 - i]))
                    {
                        mNewAnchoredPosition = mItems[mItems.Count - 1 - i].anchoredPosition;
                        mNewAnchoredPosition.x += mItemCount * mRecordOffsetX;
                        mItems[mItems.Count - 1 - i].anchoredPosition = mNewAnchoredPosition;
                        mItems[mItems.Count - 1 - i].SetAsFirstSibling();
                        TouchChildChange(mTopIndex, mItems[mItems.Count - 1 - i]);
                    }
                }
            }

            if (mIsVertical)
            {
                if (mScrollRect.transform.InverseTransformPoint(mItems[i].gameObject.transform.position).y > mDisableMarginY + Treshold)
                {
                    if (TouchPreChildChange(ChildPos.Last, mItems[i]))
                    {
                        mNewAnchoredPosition = mItems[i].anchoredPosition;
                        mNewAnchoredPosition.y -= mItemCount * mRecordOffsetY;
                        mItems[i].anchoredPosition = mNewAnchoredPosition;
                        mItems[i].SetAsLastSibling();
                        TouchChildChange(mEndIndex, mItems[i]);
                    }
                }
                else if (mScrollRect.transform.InverseTransformPoint(mItems[mItems.Count - 1 - i].gameObject.transform.position).y < -mDisableMarginY)
                {
                    if (TouchPreChildChange(ChildPos.First, mItems[mItems.Count - 1 - i]))
                    {
                        mNewAnchoredPosition = mItems[mItems.Count - 1 - i].anchoredPosition;
                        mNewAnchoredPosition.y += mItemCount * mRecordOffsetY;
                        mItems[mItems.Count - 1 - i].anchoredPosition = mNewAnchoredPosition;
                        mItems[mItems.Count - 1 - i].SetAsFirstSibling();
                        TouchChildChange(mTopIndex, mItems[mItems.Count - 1 - i]);
                    }
                }
            }
        }
    }

    void TouchChildChange(int id, Transform item)
    {
        if (ChildChange != null)
        {
            ChildChange(id, item);
        }
    }
    bool TouchPreChildChange(ChildPos pos, Transform item)
    {
        if (!BigList) return true;
        switch (pos)
        {
            case ChildPos.First:
                {
                    if (mTopIndex == 0)
                    {
                        return false;
                    }
                    mTopIndex--;
                    mEndIndex--;
                    break;
                }
            case ChildPos.Last:
                {
                    if (mEndIndex == ListCount - 1)
                    {
                        return false;
                    }
                    mTopIndex++;
                    mEndIndex++;
                    break;
                }
        }
        return true;
    }
    [ContextMenu("SetContentSize")]
    void SetContentSize()
    {
        var scrollRect = GetComponent<ScrollRect>();
        VerticalLayoutGroup verticalLayoutGroup = null;
        HorizontalLayoutGroup horizontalLayoutGroup = null;
        GridLayoutGroup gridLayoutGroup = null;
        ContentSizeFitter contentSizeFitter = null;
        if (scrollRect.content.GetComponent<VerticalLayoutGroup>() != null)
        {
            verticalLayoutGroup = scrollRect.content.GetComponent<VerticalLayoutGroup>();
        }
        if (scrollRect.content.GetComponent<HorizontalLayoutGroup>() != null)
        {
            horizontalLayoutGroup = scrollRect.content.GetComponent<HorizontalLayoutGroup>();
        }
        if (scrollRect.content.GetComponent<GridLayoutGroup>() != null)
        {
            gridLayoutGroup = scrollRect.content.GetComponent<GridLayoutGroup>();
        }
        if (scrollRect.content.GetComponent<ContentSizeFitter>() != null)
        {
            contentSizeFitter = scrollRect.content.GetComponent<ContentSizeFitter>();
            contentSizeFitter.enabled = false;
        }
        if (scrollRect != null && scrollRect.content != null && scrollRect.content.childCount > 0 && ListCount > 0)
        {
            float startDim = 0;
            float endDim = 0;
            int listCount = ListCount;
            int childCount = scrollRect.content.childCount;
            if (scrollRect.horizontal)
            {
                if (horizontalLayoutGroup != null)
                {
                    startDim = horizontalLayoutGroup.padding.left;
                    endDim = horizontalLayoutGroup.padding.right;
                }
                else if (gridLayoutGroup != null)
                {
                    startDim = gridLayoutGroup.padding.left;
                    endDim = gridLayoutGroup.padding.right;
                    if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedRowCount)
                    {
                        listCount = (listCount + gridLayoutGroup.constraintCount) / gridLayoutGroup.constraintCount;
                        childCount = (childCount + gridLayoutGroup.constraintCount) / gridLayoutGroup.constraintCount;
                        gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Vertical;
                    }
                }
                var cs = GetContentSize((scrollRect.content.GetChild(0).transform as RectTransform).sizeDelta.x
                    , listCount, childCount, startDim, endDim, Space);
                var rect = scrollRect.content.transform as RectTransform;
                var sizeDelta = rect.sizeDelta;
                sizeDelta.x = cs[0];
                rect.sizeDelta = sizeDelta;
                if (horizontalLayoutGroup != null)
                {
                    horizontalLayoutGroup.spacing = cs[1];
                    horizontalLayoutGroup.CalculateLayoutInputHorizontal();
                    horizontalLayoutGroup.SetLayoutHorizontal();
                    horizontalLayoutGroup.CalculateLayoutInputVertical();
                    horizontalLayoutGroup.SetLayoutVertical();
                }
                else if (gridLayoutGroup != null)
                {
                    var gs = gridLayoutGroup.spacing;
                    gs.x = Space;
                    gridLayoutGroup.spacing = gs;
                    gridLayoutGroup.CalculateLayoutInputHorizontal();
                    gridLayoutGroup.SetLayoutHorizontal();
                    gridLayoutGroup.CalculateLayoutInputVertical();
                    gridLayoutGroup.SetLayoutVertical();
                }
            }

            if (scrollRect.vertical)
            {
                if (verticalLayoutGroup != null)
                {
                    startDim = verticalLayoutGroup.padding.top;
                    endDim = verticalLayoutGroup.padding.bottom;
                }
                else if (gridLayoutGroup != null)
                {
                    startDim = gridLayoutGroup.padding.top;
                    endDim = gridLayoutGroup.padding.bottom;
                    if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
                    {
                        listCount = (listCount + gridLayoutGroup.constraintCount - 1) / gridLayoutGroup.constraintCount;
                        childCount = (childCount + gridLayoutGroup.constraintCount - 1) / gridLayoutGroup.constraintCount;
                        gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
                    }
                }
                var cs = GetContentSize((scrollRect.content.GetChild(0).transform as RectTransform).sizeDelta.y
                    , listCount, childCount, startDim, endDim, Space);
                var rect = scrollRect.content.transform as RectTransform;
                var sizeDelta = rect.sizeDelta;
                sizeDelta.y = cs[0];
                rect.sizeDelta = sizeDelta;
                if (verticalLayoutGroup != null)
                {
                    verticalLayoutGroup.spacing = cs[1];
                    verticalLayoutGroup.CalculateLayoutInputHorizontal();
                    verticalLayoutGroup.SetLayoutHorizontal();
                    verticalLayoutGroup.CalculateLayoutInputVertical();
                    verticalLayoutGroup.SetLayoutVertical();
                }
                else if (gridLayoutGroup != null)
                {
                    var gs = gridLayoutGroup.spacing;
                    gs.y = Space;
                    gridLayoutGroup.spacing = gs;
                    gridLayoutGroup.CalculateLayoutInputHorizontal();
                    gridLayoutGroup.SetLayoutHorizontal();
                    gridLayoutGroup.CalculateLayoutInputVertical();
                    gridLayoutGroup.SetLayoutVertical();
                }
            }
        }
    }
    float[] GetContentSize(float itemDim, int itemTotalCount, int itemCacheCount, float startDim, float endDim, float space) 
    {
        float[] rt = new float[2];
        // 总维度
        rt[0] = itemDim * itemTotalCount + startDim + endDim + (itemTotalCount - 1) * space;

        // 相对间距
        rt[1] = (space - ((rt[0] - startDim - endDim) / itemCacheCount - itemDim)) * itemCacheCount;
        return rt;
    }
}
