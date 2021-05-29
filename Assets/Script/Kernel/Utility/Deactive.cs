using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deactive : MonoBehaviour
{
    public float Delay = 0;
    public bool RealTime = true;
    public bool OnlyBehaviour = false;
	// Use this for initialization
	void Awake () {
        if (Delay > 0)
        {
            StartCoroutine(DelayDeactive(Delay));
        }
        else
        {
            SetDeactive();
        }
        
	}
	IEnumerator DelayDeactive(float time)
    {
        if (RealTime)
        {
            yield return new WaitForSecondsRealtime(time);
        }
        else
        {
            yield return new WaitForSeconds(time);
        }
        SetDeactive();
    }

    void SetDeactive()
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
