using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownText : MonoBehaviour
{
    public enum ShowType
    {//天/时/分/秒
        ST_Day,
        ST_Hour,
        ST_Min,
        ST_Sec,
        ST_HM,
    }
    public Text Countdown;
    public bool IsRealTime = false;
    float mCountdown = 0;
    float mTime = 0;
    System.Action mFinishFunc;
    ShowType mShowType;
    public void SetCountdown(long sec, System.Action finishFunc, ShowType type = ShowType.ST_Min)
    {
        mFinishFunc = finishFunc;
        mCountdown = sec;
        gameObject.SetActive(true);
        mShowType = type;
        SetText((int)mCountdown);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (mCountdown <= 0)
        {
            gameObject.SetActive(false);
            if (mFinishFunc != null)
            {
                mFinishFunc();
            }
            return;
        }
        if (IsRealTime)
        {
            mTime += Time.unscaledDeltaTime;
        }
        else
        {
            mTime += Time.deltaTime;
        }
        
        if (mTime > 0.9f)
        {
            mCountdown -= mTime;
            if (mCountdown < 0)
            {
                mCountdown = 0;
            }
            SetText((int)mCountdown);
            mTime = 0;
        }
    }

    void SetText(int t)
    {
        int day = t / 86400, hour = t / 3600, min = t / 60, sec = t % 60;
        switch (mShowType)
        {
            case ShowType.ST_Day:
                {
                    day = t / 86400; hour = (t / 3600) % 24; min = (t / 60) % 60; sec = t % 60;
                    Countdown.text = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", day, hour, min, sec);
                }
                break;
            case ShowType.ST_Hour:
                {
                    hour = (t / 3600); min = (t / 60) % 60; sec = t % 60;
                    Countdown.text = string.Format("{0:D2}:{1:D2}:{2:D2}", hour, min, sec);
                }
                break;
            case ShowType.ST_Min:
                {
                    min = (t / 60); sec = t % 60;
                    Countdown.text = string.Format("{0:D2}:{1:D2}", min, sec);
                }
                break;
            case ShowType.ST_Sec:
                {
                    sec = t % 60;
                    Countdown.text = string.Format("{0:D2}", sec);
                }
                break;
            case ShowType.ST_HM:
                {
                    hour = (t / 3600); min = (t / 60) % 60;
                    Countdown.text = string.Format("{0:D2}:{1:D2}", hour, min);
                }
                break;
            default:
                {
                    min = (t / 60); sec = t % 60;
                    Countdown.text = string.Format("{0:D2}:{1:D2}", min, sec);
                }
                break;
        }
    }
}
