using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class LangUtilEditor
{
    [MenuItem("Tools/Game Language/EditMode/ChineseSimplified")]
    static void RefreshJsonTable_ChineseSimplified()
    {
        LitJson.JsonReader reader = new LitJson.JsonReader(System.IO.File.ReadAllText("Assets/Tables/ChineseSimplified.json"));
        LangUtil.slangJd = LitJson.JsonMapper.ToObject(reader);
    }
    [MenuItem("Tools/Game Language/EditMode/ChineseTraditional")]
    static void RefreshJsonTable_ChineseTraditional()
    {
        LitJson.JsonReader reader = new LitJson.JsonReader(System.IO.File.ReadAllText("Assets/Tables/ChineseTraditional.json"));
        LangUtil.slangJd = LitJson.JsonMapper.ToObject(reader);
    }
    [MenuItem("Tools/Game Language/EditMode/English")]
    static void RefreshJsonTable_English()
    {
        LitJson.JsonReader reader = new LitJson.JsonReader(System.IO.File.ReadAllText("Assets/Tables/English.json"));
        LangUtil.slangJd = LitJson.JsonMapper.ToObject(reader);
    }
    [MenuItem("Tools/Game Language/EditMode/Japanese")]
    static void RefreshJsonTable_Japanese()
    {
        LitJson.JsonReader reader = new LitJson.JsonReader(System.IO.File.ReadAllText("Assets/Tables/Japanese.json"));
        LangUtil.slangJd = LitJson.JsonMapper.ToObject(reader);
    }
    [MenuItem("Tools/Game Language/EditMode/French")]
    static void RefreshJsonTable_French()
    {
        LitJson.JsonReader reader = new LitJson.JsonReader(System.IO.File.ReadAllText("Assets/Tables/French.json"));
        LangUtil.slangJd = LitJson.JsonMapper.ToObject(reader);
    }
    [MenuItem("Tools/Game Language/EditMode/German")]
    static void RefreshJsonTable_German()
    {
        LitJson.JsonReader reader = new LitJson.JsonReader(System.IO.File.ReadAllText("Assets/Tables/German.json"));
        LangUtil.slangJd = LitJson.JsonMapper.ToObject(reader);
    }
    [MenuItem("Tools/Game Language/EditMode/Korean")]
    static void RefreshJsonTable_Korean()
    {
        LitJson.JsonReader reader = new LitJson.JsonReader(System.IO.File.ReadAllText("Assets/Tables/Korean.json"));
        LangUtil.slangJd = LitJson.JsonMapper.ToObject(reader);
    }
    [MenuItem("Tools/Game Language/EditMode/Spanish")]
    static void RefreshJsonTable_Spanish()
    {
        LitJson.JsonReader reader = new LitJson.JsonReader(System.IO.File.ReadAllText("Assets/Tables/Spanish.json"));
        LangUtil.slangJd = LitJson.JsonMapper.ToObject(reader);
    }
    [MenuItem("Tools/Game Language/EditMode/Portuguese")]
    static void RefreshJsonTable_Portuguese()
    {
        LitJson.JsonReader reader = new LitJson.JsonReader(System.IO.File.ReadAllText("Assets/Tables/Portuguese.json"));
        LangUtil.slangJd = LitJson.JsonMapper.ToObject(reader);
    }
    [MenuItem("Tools/Game Language/EditMode/Russian")]
    static void RefreshJsonTable_Russian()
    {
        LitJson.JsonReader reader = new LitJson.JsonReader(System.IO.File.ReadAllText("Assets/Tables/Russian.json"));
        LangUtil.slangJd = LitJson.JsonMapper.ToObject(reader);
    }
    //------------------------------------------------------------------------------------------------------------------------
    [MenuItem("Tools/Game Language/RunMode/ChineseSimplified")]
    static void UseChineseSimplified()
    {
        if (Application.isPlaying)
        {
            LanguageText.GetSingleton().ChangeLanguage("ChineseSimplified");
        }
        else
        {
            Debug.LogWarning("Need play the game.");
        }   
    }
    [MenuItem("Tools/Game Language/RunMode/ChineseTraditional")]
    static void UseChineseTraditional()
    {
        if (Application.isPlaying)
        {
            LanguageText.GetSingleton().ChangeLanguage("ChineseTraditional");
        }
        else
        {
            Debug.LogWarning("Need play the game.");
        }
    }
    [MenuItem("Tools/Game Language/RunMode/English")]
    static void UseEnglish()
    {
        if (Application.isPlaying)
        {
            LanguageText.GetSingleton().ChangeLanguage("English");
        }
        else
        {
            Debug.LogWarning("Need play the game.");
        }
    }
    [MenuItem("Tools/Game Language/RunMode/Japanese")]
    static void UseJapanese()
    {
        if (Application.isPlaying)
        {
            LanguageText.GetSingleton().ChangeLanguage("Japanese");
        }
        else
        {
            Debug.LogWarning("Need play the game.");
        }
    }
    [MenuItem("Tools/Game Language/RunMode/French")]
    static void UseFrench()
    {
        if (Application.isPlaying)
        {
            LanguageText.GetSingleton().ChangeLanguage("French");
        }
        else
        {
            Debug.LogWarning("Need play the game.");
        }
    }
    [MenuItem("Tools/Game Language/RunMode/German")]
    static void UseGerman()
    {
        if (Application.isPlaying)
        {
            LanguageText.GetSingleton().ChangeLanguage("German");
        }
        else
        {
            Debug.LogWarning("Need play the game.");
        }
    }
    [MenuItem("Tools/Game Language/RunMode/Korean")]
    static void UseKorean()
    {
        if (Application.isPlaying)
        {
            LanguageText.GetSingleton().ChangeLanguage("Korean");
        }
        else
        {
            Debug.LogWarning("Need play the game.");
        }
    }
    [MenuItem("Tools/Game Language/RunMode/Spanish")]
    static void UseSpanish()
    {
        if (Application.isPlaying)
        {
            LanguageText.GetSingleton().ChangeLanguage("Spanish");
        }
        else
        {
            Debug.LogWarning("Need play the game.");
        }
    }
    [MenuItem("Tools/Game Language/RunMode/Portuguese")]
    static void UsePortuguese()
    {
        if (Application.isPlaying)
        {
            LanguageText.GetSingleton().ChangeLanguage("Portuguese");
        }
        else
        {
            Debug.LogWarning("Need play the game.");
        }
    }
    [MenuItem("Tools/Game Language/RunMode/Russian")]
    static void UseRussian()
    {
        if (Application.isPlaying)
        {
            LanguageText.GetSingleton().ChangeLanguage("Russian");
        }
        else
        {
            Debug.LogWarning("Need play the game.");
        }
    }
}
