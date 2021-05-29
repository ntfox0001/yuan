using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LanguageTextBox : MonoBehaviour
{

    [SerializeField]
    Text mTextUI;

    [SerializeField]
    int mTextId = 0;

    [SerializeField]
    bool mBreakOff = false;

    [SerializeField]
    int mCharBreakOff = 0;
    [SerializeField]
    int mWordBreakOff = 0;

    bool mFormatText = true;

    /// <summary>
    /// 设置Text组合方式，true为format函数方式，false为直接组合，且只读取第一个params
    /// </summary>
    public bool FormatText
    {
        get
        {
            return mFormatText;
        }
        set
        {
            mFormatText = value;
        }
    }
    bool mEnableRefreshText = true;
    public bool EnableRefreshText
    {
        get { return mEnableRefreshText; }
        set { mEnableRefreshText = value; }
    }
    [SerializeField]
    string[] mParams;
    public string[] Params
    {
        set
        {
            mParams = value;
            RefreshText();
        }
        get { return mParams; }
    }

    public int TextId
    {
        set
        {
            if (mTextId == value) return;
            mTextId = value;
            RefreshText();
        }
        get { return mTextId; }
    }
    public string Text
    {
        get { return mTextUI.text; }
    }
    
    private void Start()
    {
        RefreshText();
    }

    /// <summary>
    /// 强制刷新
    /// </summary>
    public void RefreshText()
    {
        if (!mEnableRefreshText) return;
        if (mTextId > 0)
        {
            string text = LanguageText.GetSingleton().GetText(mTextId, false);
            if (mBreakOff)
            {
                int breakOff = 0;
                if (LanguageText.GetSingleton().IsCharacterLang())
                {
                    breakOff = LanguageText.GetSingleton().CharacterLangBreakOff;
                    if (mCharBreakOff >= 0)
                    {
                        breakOff = mCharBreakOff;
                    }
                }
                else
                {
                    breakOff = LanguageText.GetSingleton().WordsLangBreakOff;
                    if (mWordBreakOff >= 0)
                    {
                        breakOff = mWordBreakOff;
                    }

                }
                text = LanguageText.GetSingleton().Break(text, breakOff);
            }
            
            
            text = text.Replace("\\n", "\n");
            if (FormatText)
            {
                if (Params != null && Params.Length > 0)
                {
                    mTextUI.text = string.Format(text, Params);
                }
                else
                {
                    mTextUI.text = text;
                }
            }
            else
            {
                if (Params != null && Params.Length > 0)
                {
                    mTextUI.text = text + Params[0];
                }
                else
                {
                    mTextUI.text = text;
                }
            }
        }
        else if (mTextId == 0)
        {
            mTextUI.text = "";
        }
        else
        {
            // 如果小于0，那么不刷新
        }
    }
    /// <summary>
    /// 使用textId对应的字符串设置params
    /// </summary>
    /// <param name="paramsTextId"></param>
    public void SetParamsByTextId(params int[] paramsTextId)
    {
        Params = new string[paramsTextId.Length];
        for (int i = 0; i < paramsTextId.Length; i++)
        {
            Params[i] = LanguageText.GetSingleton().GetOriginalText(paramsTextId[i].ToString());
        }
        RefreshText();
    }
    /// <summary>
    /// 辅助函数，不用自己生成数组
    /// </summary>
    /// <param name="paramsStr"></param>
    public void SetParams(params string[] paramsStr)
    {
        Params = paramsStr;
    }
    public void SetText(string text)
    {
        mTextId = -1;
        mTextUI.text = text;
    }
    void OnLanguageChange()
    {
        RefreshText();
    }
    /// <summary>
    /// 设置是否截断换行，当字符串使用params功能时，不要使用break功能
    /// </summary>
    public bool BreakOff { get { return mBreakOff; } set { mBreakOff = value; } }
    public int CharBreakOff { get { return mCharBreakOff; } set { mCharBreakOff = value; } }
    public int WordBreakOff { get { return mWordBreakOff; } set { mWordBreakOff = value; } }
}
