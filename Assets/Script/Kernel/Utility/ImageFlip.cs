using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFlip : MonoBehaviour
{
    public TweenFloatValue FrontAlpha;
    public TweenVec3Value FrontPos;
    public TweenFloatValue BackAlpha;
    public TweenVec3Value BackPos;

    public TweenValueBase FlipColor;
    public Image[] ImagePair;

    uTools.TweenAlpha[] mTweenAlpha = new uTools.TweenAlpha[2];
    uTools.TweenPosition[] mTweenPosition = new uTools.TweenPosition[2];
    uTools.TweenColor[] mTweenColors = new uTools.TweenColor[2];
    Sprite mNextStripe;
    Color mNextColor = Color.white;
    bool mFilpPos = false;

    // 当前隐藏的image
    int mBackIndex = 0;

    private void Start()
    {
        mTweenAlpha[0] = ImagePair[0].GetComponent<uTools.TweenAlpha>();
        mTweenAlpha[1] = ImagePair[1].GetComponent<uTools.TweenAlpha>();

        mTweenPosition[0] = ImagePair[0].GetComponent<uTools.TweenPosition>();
        mTweenPosition[1] = ImagePair[1].GetComponent<uTools.TweenPosition>();

        mTweenColors[0] = ImagePair[0].GetComponent<uTools.TweenColor>();
        mTweenColors[1] = ImagePair[1].GetComponent<uTools.TweenColor>();

        FlipColor.Apply(mTweenColors[0]);
        FlipColor.Apply(mTweenColors[1]);

        StartCoroutine(UpdateFlip());
    }

    public void SetImage(Sprite sprite, Color color)
    {
        ImagePair[Front].sprite = sprite;
        ImagePair[Front].color = color;
    }
    public void Flip(Sprite sprite, bool filpPos, Color color)
    {
        mNextStripe = sprite;
        mNextColor = color;
        mFilpPos = filpPos;
    }
    public int Back {  get { return mBackIndex; } }
    public int Front { get { return mBackIndex == 0 ? 1 : 0; } }
    IEnumerator UpdateFlip()
    {
        while (true)
        {
            if (mNextStripe != null)
            {
                ImagePair[Back].sprite = mNextStripe;
                mNextStripe = null;
                // 开始移动
                float len = PlayAll(mFilpPos);
                yield return new WaitForSecondsRealtime(len);
                FlipIndex();
            }
            else
            {
                yield return null;
            }
        }
    }
    void FlipIndex()
    {
        mBackIndex = mBackIndex == 0 ? 1 : 0;
    }
    float PlayAll(bool filpY)
    {
        ImagePair[Front].transform.SetSiblingIndex(ImagePair[Back].transform.GetSiblingIndex());
        // front
        FrontAlpha.Apply(mTweenAlpha[Front]);
        mTweenAlpha[Front].ResetToBeginning();
        mTweenAlpha[Front].PlayForward();
        FrontPos.Apply(mTweenPosition[Front]);
        if (filpY)
        {
            mTweenPosition[Front].to = -mTweenPosition[Front].to;
        }
        mTweenPosition[Front].ResetToBeginning();
        mTweenPosition[Front].PlayForward();
        

        // back
        BackAlpha.Apply(mTweenAlpha[Back]);
        mTweenAlpha[Back].ResetToBeginning();
        mTweenAlpha[Back].PlayForward();
        BackPos.Apply(mTweenPosition[Back]);
        if (filpY)
        {
            mTweenPosition[Back].from = -mTweenPosition[Back].from;
        }
        mTweenPosition[Back].ResetToBeginning();
        mTweenPosition[Back].PlayForward();

        // color
        mTweenColors[0].from = mTweenColors[0].to;
        mTweenColors[1].from = mTweenColors[1].to;

        mTweenColors[0].to = mNextColor;
        mTweenColors[1].to = mNextColor;

        mTweenColors[0].ResetToBeginning();
        mTweenColors[0].PlayForward();

        mTweenColors[1].ResetToBeginning();
        mTweenColors[1].PlayForward();


        float maxLen = Mathf.Max(FrontAlpha.Length, FrontPos.Length, BackAlpha.Length, BackPos.Length);

        return maxLen;
    }
}
