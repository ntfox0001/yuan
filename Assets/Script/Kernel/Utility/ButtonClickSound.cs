using UnityEngine;
using System.Collections;

public class ButtonClickSound : MonoBehaviour 
{

    public string ClickSoundName;
    public string GroupName;
    void OnClick()
    {
        SoundManager.GetSingleton().UI.Play(ClickSoundName, GroupName);
    }

}
