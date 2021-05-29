using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformUtility
{
    /// <summary>
    /// 返回streaming目录，目前只支持安卓ios和桌面
    /// </summary>
    /// <returns></returns>
    public static string GetStreamingAssets()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                return Application.dataPath + "!/assets/";
            case RuntimePlatform.IPhonePlayer:
                return Application.dataPath + "/Raw";
            default:
                return Application.dataPath + "/StreamingAssets";
        }
    }
}
