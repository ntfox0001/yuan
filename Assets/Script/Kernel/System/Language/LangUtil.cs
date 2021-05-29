using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LangUtil : MonoBehaviour
{
#if UNITY_EDITOR
    static public LitJson.JsonData slangJd = null;
    static public string GetLanguageText(int id, string[] Params = null)
    {
        if (slangJd == null)
        {
            LitJson.JsonReader reader = new LitJson.JsonReader(System.IO.File.ReadAllText("Assets/Tables/ChineseSimplified.json"));
            slangJd = LitJson.JsonMapper.ToObject(reader);
        }
        
        // 编辑器模式下，就用简体中文
        if (slangJd.Keys.Contains(id.ToString()))
        {
            if (Params != null && Params.Length > 0)
            {
               return string.Format(slangJd[id.ToString()]["Text"].GetString(), Params);
            }
            else
            {
                return slangJd[id.ToString()]["Text"].GetString();
            }
        }
        return "";
    }
#endif

    static public string UpperFirstWord(string word)
    {
        if (word.Length > 0)
        {
            string s = word.Substring(0, 1);
            s = s.ToUpper();
            return s + word.Substring(1);
        }
        return word;
    }
}
