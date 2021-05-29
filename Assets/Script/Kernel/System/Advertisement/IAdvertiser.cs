using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAdvertiser
{
    string AdName { get; }
    /// <summary>
    /// 初始化广告
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cb"></param>
    /// <returns>表示这个广告是否可用</returns>
    bool Initial(string userId, IAdvertiserCallback cb, AdvertisementSetting adSetting);
    bool ShowAD(int type);
    bool IsReady(int type);
    void Release();
    bool SupportPlatform();

    
}
