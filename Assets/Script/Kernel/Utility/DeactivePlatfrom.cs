using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivePlatfrom : MonoBehaviour
{

    public bool OnlyBehaviour = false;
    public bool OnAndroid = false;
    public bool OnIOS = false;
    public bool OnEditor = false;
    void Awake()
    {
        SetDeactive();
    }
    void SetDeactive()
    {
        if ((Application.platform == RuntimePlatform.Android && OnAndroid) ||
            (Application.platform == RuntimePlatform.IPhonePlayer && OnIOS) ||
            ((Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor) && OnEditor))
        {
            if (OnlyBehaviour)
            {
                var cs = GetComponents<Behaviour>();
                for (int i = 0; i < cs.Length; i++)
                {
                    cs[i].enabled = false;
                }
            }
            else
            {
                gameObject.SetActive(false);
            }

        }
    }

}
