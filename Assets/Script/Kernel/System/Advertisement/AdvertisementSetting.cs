using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class AdvertisementSetting
{
    /// <summary>
    /// 唯一标识 AdName + platform + area
    /// </summary>
    public string UniqueId;
    public string AdName;
    public string Platform;
    public string Area;
    public string AppId;
    public bool Enable;
    public string[] RewardedId;
    public string[] InterstitialId;
    public string[] TestDevice;

    public static Dictionary<string, AdvertisementSetting> GetAdSetting(XmlDocument doc)
    {
        XmlElement rootElem = doc.DocumentElement;
        Dictionary<string, AdvertisementSetting> adSettingMap = new Dictionary<string, AdvertisementSetting>();

        // test device
        var testDeviceList = rootElem.SelectNodes("TestDevice");
        var testDevices = new string[testDeviceList.Count];
        for (int i = 0; i < testDeviceList.Count; i++)
        {
            testDevices[i] = testDeviceList[i].InnerText;
        }

        var adList = rootElem.SelectNodes("Ad");

        for (int k = 0; k < adList.Count; k++)
        {
            var adElem = (XmlElement)adList[k];



            var psList = adElem.SelectNodes("PackageSetting");

            for (int i = 0; i < psList.Count; i++)
            {
                var settingElem = (XmlElement)psList[i];

                string adName = adElem.GetAttribute("name");
                var adsetting = new AdvertisementSetting();
                adsetting.AdName = adName;

                adsetting.Platform = settingElem.GetAttribute("platform");
                adsetting.Area = settingElem.GetAttribute("area");
                adsetting.Enable = bool.Parse(settingElem.GetAttribute("enable"));

                var appIdElem = (XmlElement)settingElem.SelectSingleNode("AppId");
                if (appIdElem != null)
                {
                    adsetting.AppId = appIdElem.GetAttribute("id");

                    // rewarded
                    var rewardeds = appIdElem.SelectNodes("Rewarded");
                    adsetting.RewardedId = new string[rewardeds.Count];
                    for (int j = 0; j < 2; j++)
                    {
                        if (rewardeds.Count > j)
                        {
                            var rewardedElem = (XmlElement)rewardeds[j];
                            adsetting.RewardedId[j] = rewardedElem.GetAttribute("id");
                        }
                    }

                    // interstitial
                    var interstitials = appIdElem.SelectNodes("interstitial");
                    adsetting.InterstitialId = new string[interstitials.Count];
                    for (int j = 0; j < 2; j++)
                    {
                        if (interstitials.Count > j)
                        {
                            var interstitialElem = (XmlElement)interstitials[j];
                            adsetting.InterstitialId[j] = interstitialElem.GetAttribute("id");
                        }
                    }
                }

                // add test device
                adsetting.TestDevice = testDevices;

                adsetting.UniqueId = adName + adsetting.Platform + adsetting.Area;

                adSettingMap.Add(adsetting.UniqueId, adsetting);
            }

        }

        return adSettingMap;
    }
}
