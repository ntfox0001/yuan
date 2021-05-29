using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAdvertiserCallback
{
    void OnInitial(bool result, string msg);
    void OnShowAd(AdvertisementManager.ShowAdCallbackState state, string placeId, string msg);

}
