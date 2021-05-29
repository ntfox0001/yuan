using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class ConfigXmlBuilder
{
    public const string ValueType_Default = "Default";
    public const string ValueType_iOS = "iOS";
    public const string ValueType_Android = "Android";
    public const string ValueType_Standalone = "Standalone";
    string mFilename;
    XmlDocument mDoc;
    public ConfigXmlBuilder(string filename)
    {
        mFilename = filename;
        mDoc = new XmlDocument();
        mDoc.AppendChild(mDoc.CreateXmlDeclaration("1.0", "utf-8", ""));
    }

    public void SetValue(string key, string defVal, string iosVal, string androidVal, string standaloneVal)
    {
        XmlElement elem = mDoc.CreateElement(key);
        AddValue(elem, ValueType_Default, defVal);
        AddValue(elem, ValueType_iOS, iosVal);
        AddValue(elem, ValueType_Android, androidVal);
        AddValue(elem, ValueType_Standalone, standaloneVal);
    }
    public void SetValue(string key, string defVal)
    {
        XmlElement elem = mDoc.CreateElement(key);
        AddValue(elem, ValueType_Default, defVal);
    }
    public void Save()
    {
        mDoc.Save(mFilename);
    }
    void AddValue(XmlElement parent, string valType, string val)
    {
        XmlElement defElem = mDoc.CreateElement(valType);
        defElem.InnerText = val;
        parent.AppendChild(defElem);
    }

    public static string GetCurrentPlatformString()
    {
#if UNITY_IOS
        return ValueType_iOS;
#elif UNITY_ANDROID
        return ValueType_Android;
#else
        return ValueType_Standalone;
#endif
    }
}
