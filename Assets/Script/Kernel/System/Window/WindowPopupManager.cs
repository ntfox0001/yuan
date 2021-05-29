using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowPopupManager : Singleton<WindowPopupManager> {
    public enum AwardType
    {
        Ads = 1,
        Gold,
        Medal
    }
    public string[] Icons;
    public class AwardData
    {
        public AwardType Type;
        public int Count;
    }
    private float mTimeForShow = 1.0f;
    private GameObject mCurrShowItem;
    private List<object[]> mShowList = new List<object[]>();
    private bool mHasWindowShow = false;

    public void ShowCommonAwardPopup(params object[] param)
    {

        if (mHasWindowShow)
        {
            mShowList.Add(param);
            return;
        }


        AwardType type = (AwardType)param[0];
        int count = (int)param[1];
        bool translucent = false;
        if(param[2] != null)
        {
            translucent = (bool)param[2];
        }

        switch (type)
        {
            case AwardType.Ads:
                mCurrShowItem = WindowManager.GetSingleton().ActiveWindowStack.CreateWindow("AdTicketAwardWindow", "awardpopup", count, translucent);
                break;
            case AwardType.Gold:
                mCurrShowItem = WindowManager.GetSingleton().ActiveWindowStack.CreateWindow("GoldAwardWindow", "awardpopup", count, translucent);
                break;
            case AwardType.Medal:
                mCurrShowItem = WindowManager.GetSingleton().ActiveWindowStack.CreateWindow("MedalAwardWindow", "awardpopup", count, translucent);
                break;
            default:
                break;
        };
  
        mHasWindowShow = true;
        if (mCurrShowItem != null)
        {
            StartCoroutine(DelayDestroyCoroutine());
        }
    }

    IEnumerator DelayDestroyCoroutine()
    {
        yield return new WaitForSecondsRealtime(mTimeForShow);
        Destroy(mCurrShowItem);
        mHasWindowShow = false;
        if (mShowList.Count>0)
        {
            ShowCommonAwardPopup(mShowList[0]);
            mShowList.RemoveAt(0);
        }
    }

    public void Initial()
    {}
    public void Release()
    {}
    public static void Show(List<AwardData> l)
    {
        WindowManager.GetSingleton().StartCoroutine(ShowWindow(l));
    }
    static IEnumerator ShowWindow(List<AwardData> l)
    {

        for (int i = 0; i < l.Count; i++)
        {
            GameObject win = null;
            switch (l[i].Type)
            {
                case AwardType.Ads:
                    win = WindowManager.GetSingleton().ActiveWindowStack.CreateWindow("AdTicketAwardWindow", "awardpopup", l[i].Count, false);
                    break;
                case AwardType.Gold:
                    win = WindowManager.GetSingleton().ActiveWindowStack.CreateWindow("GoldAwardWindow", "awardpopup", l[i].Count, false);
                    break;
                case AwardType.Medal:
                    win = WindowManager.GetSingleton().ActiveWindowStack.CreateWindow("MedalAwardWindow", "awardpopup", l[i].Count, false);
                    break;
            };

            yield return new WaitForSeconds(2.0f);

            if (win != null)
            {
                Destroy(win);
            }
            

        }

    }
}
