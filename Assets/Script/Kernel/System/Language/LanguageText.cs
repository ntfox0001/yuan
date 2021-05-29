using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class LanguageText : Singleton<LanguageText>, IManagerBase
{
    public const string SystemLanguageSetName = "LangText_SystemLanguageSetName";//当前系统语言

    public string Symbol;
    public int WordsLangBreakOff = 15;
    public int CharacterLangBreakOff = 12;
    public bool BreakOff = true;
    SystemLanguage mCurrentLanguage = SystemLanguage.Unknown;
    LitJson.JsonData mLanguageTable;
    LitJson.JsonData mLanguageData;

    public SystemLanguage CurrentLanguage
    {
        get
        {
            return mCurrentLanguage;
        }

        set
        {
            mCurrentLanguage = value;
        }
    }

    public void Initial()
    {
        // 先从本地取，没有以系统为准
        string currlang  = PlayerPrefs.GetString(SystemLanguageSetName,"");
        if (currlang == "")
        {
            Debug.Log(Application.systemLanguage);
#if DREAM_MAINLAND
            string langId = "zh_CN";
            string lang = "zh";
            string region = "CN";
#else
            string langId = PreciseLocale.GetLanguageID();
            string lang = PreciseLocale.GetLanguage();
            string region = PreciseLocale.GetRegion();
#endif
            Debug.Log("langId:" + langId + " lang:" + lang + " region:" + region);
            // 参考 https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes
            switch (lang)
            {
                case "zh":
                    switch (region)
                    {
                        case "MO":
                        case "HK":
                        case "TW":
                            currlang = "ChineseTraditional";
                            break;
                        default: // "CN"
                            currlang = "ChineseSimplified";
                            break;
                    }
                    break;
                case "ja":
                    currlang = "Japanese";
                    break;
                case "es":
                    currlang = "Spanish";
                    break;
                case "de":
                    currlang = "German";
                    break;
                case "ko":
                    currlang = "Korean";
                    break;
                case "fr":
                    currlang = "French";
                    break;
                case "pt":
                    currlang = "Portuguese";
                    break;
                case "ru":
                    currlang = "Russian";
                    break;
                default:
                    currlang = "English";
                    break;
            }
        }
        changeLanguage(currlang);
        //changeLanguage("English");
    }

    public void Release()
    {
        
    }

   /* public bool HasLanguage(string lang)
    {
        return mLanguageData.Keys.Contains(lang);
    }*/
    public bool ChangeLanguage(string lang)
    {
        bool rt = changeLanguage(lang);

        if (rt == true)
        {
            // 广播消息
            GlobalObjects.GetSingleton().BroadcastMessage("OnLanguageChange", SendMessageOptions.DontRequireReceiver);
            GlobalObjects.GetSingleton().UIRoot.BroadcastMessage("OnLanguageChange", SendMessageOptions.DontRequireReceiver);
            // scene下不应该有文字节点
            //GlobalObjects.GetSingleton().SceneRoot.BroadcastMessage("OnLanguageChange", SendMessageOptions.DontRequireReceiver);
        }
        return rt;
    }
    bool changeLanguage(string lang)
    {
            if (mCurrentLanguage.ToString() != lang)
            {
                mCurrentLanguage = (SystemLanguage)System.Enum.Parse(typeof(SystemLanguage), lang);
                mLanguageTable = TableManager.GetSingleton().GetTable(lang);
                return true;
            }
        return false;
    }
    /// <summary>
    /// 根据系统设置自动判断是否break
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public string GetText(int id)
    {
        string text = GetOriginalText(id.ToString());
        if (!BreakOff)
        {
            return text;
        }
        return Break(text, GetCurrentBreakOff());
    }
    public string GetText(int id, bool breakOf)
    {
        string text = GetOriginalText(id.ToString());
        if (!breakOf)
        {
            return text;
        }
        return Break(text, GetCurrentBreakOff());
    }
    public string GetText(int id, int charBreakoff, int wordBreakoff)
    {
        int breakoff = IsCharacterLang(mCurrentLanguage) ? charBreakoff : wordBreakoff;
        string text = GetOriginalText(id.ToString());

        return Break(text, breakoff);
    }
    public string GetOriginalText(string id)
    {
        return mLanguageTable[id]["Text"].GetString();
    }
    public string Break(string text, int breakoff)
    {
        if (text.Length <= breakoff) return text;

        string rttext = "";
        if (IsCharacterLang(mCurrentLanguage))
        {
            // 如果是字语言，那么按照长度来，并且下一句得第一个字符如果是符号的话，那么就把符号合并到上一句。
            int i = 0;
            while (text.Length - i >= breakoff)
            {
                rttext = rttext + text.Substring(i, breakoff);
                i += breakoff;

                if (text.Length > i && IsSymbol(text[i]))
                {
                    rttext = rttext + text.Substring(i, 1) + "\n";
                    i++;
                }
                else
                {
                    rttext = rttext + "\n";
                }
            }

            if (text.Length > i)
            {
                rttext = rttext + text.Substring(i, text.Length - i);
            }
            else if (rttext[rttext.Length - 1] == '\n')
            {
                rttext = rttext.Substring(0, rttext.Length - 1);
            }
        }
        else
        {
            int i = 0;
            // 单词类语言，找到breakoff后第一个空格，插入回车
            while (true)
            {
                int j = text.IndexOf(' ', i + breakoff);
                if (j == -1)
                {
                    rttext = rttext + text.Substring(i);
                    break;
                }
                else
                {
                    rttext = rttext + text.Substring(i, j - i) + "\n";
                }
                i = j + 1;
                if (i + breakoff >= text.Length)
                {
                    rttext = rttext + text.Substring(i);
                    break;
                }
            }
        }

        return rttext;
    }

   /* public List<string> GetAllLanguage()
    {
        return new List<string>(mLanguageData.Keys);
    }*/
    public string GetText(string lang, int id)
    {
        return mLanguageData[lang][id.ToString()]["Text"].GetString();
    }
    // 是以字为单位的语言
    bool IsCharacterLang(SystemLanguage lang)
    {
        return lang == SystemLanguage.Chinese ||
            lang == SystemLanguage.ChineseSimplified ||
            lang == SystemLanguage.ChineseTraditional ||
            lang == SystemLanguage.Japanese ||
            lang == SystemLanguage.Korean;
    }
    /// <summary>
    /// 返回当前语言是否是char语言
    /// </summary>
    /// <returns></returns>
    public bool IsCharacterLang()
    {
        return IsCharacterLang(mCurrentLanguage);
    }
    public int GetCurrentBreakOff()
    {
        if (IsCharacterLang(mCurrentLanguage))
        {
            return CharacterLangBreakOff;
        }

        return WordsLangBreakOff;
    }
    public bool IsSymbol(char c)
    {
        return Symbol.Contains(c);
    }
}
