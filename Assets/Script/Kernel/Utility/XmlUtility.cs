using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class XmlUtility
{
    public static string GetValue(XmlElement elem, string key, string defVal = "")
    {
        XmlNodeList nodeList = elem.SelectNodes(key);
        foreach (XmlNode n in nodeList)
        {
            string rt = "";
            GetPlatformValue((XmlElement)n, defVal, out rt);
            return rt;
        }
        return defVal;
    }
    public static List<string> GetValues(XmlElement elem, string key)
    {
        List<string> rtList = new List<string>();
        XmlNodeList nodeList = elem.SelectNodes(key);
        foreach (XmlNode n in nodeList)
        {
            string rt;
            if (GetPlatformValue((XmlElement)n, "", out rt))
            {
                rtList.Add(rt);
            }
        }

        return rtList;
    }
    public static bool HasPlatformValue(XmlElement elem)
    {
        XmlNode platformNode = elem.SelectSingleNode(ConfigXmlBuilder.GetCurrentPlatformString());
        return !string.IsNullOrEmpty(elem.InnerText) || platformNode != null;
    }
    public static bool GetPlatformValue(XmlElement elem, string defVal, out string rtVal)
    {
        XmlNode n = elem.SelectSingleNode(ConfigXmlBuilder.GetCurrentPlatformString());
        if (n != null)
        {
            rtVal = n.InnerText;
            return true;
        }
        else
        {
            if (!string.IsNullOrEmpty(elem.InnerText))
            {
                rtVal = elem.InnerText;
                return true;
            }
        }

        rtVal = defVal;
        return false;
    }

}
