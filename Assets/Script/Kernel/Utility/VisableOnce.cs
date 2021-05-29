using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisableOnce : MonoBehaviour
{
    public string VisableKey;
	// Use this for initialization
	void Start ()
    {
		if (VisableKey != "")
        {
            gameObject.SetActive(PlayerPrefs.GetInt("_VisableOnce_" + VisableKey, 0) == 0);
            PlayerPrefs.SetInt("_VisableOnce_" + VisableKey, 1);
        }
	}
	
}
