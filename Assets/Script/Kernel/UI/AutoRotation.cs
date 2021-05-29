using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uTools;

public class AutoRotation : MonoBehaviour {


    private float mLowDuation = 0.0f;
    private float mHighDuation = 0.0f;
    private float mrotation = 0;
    public  void Initial(float ldur,float hdur,float rotation)
    {
        mLowDuation = ldur;
        mHighDuation = hdur;
        mrotation = rotation;

        if(mLowDuation >0 && mHighDuation >0 && mLowDuation <= mHighDuation)
        {//不包含0
            StartCoroutine(KeepRotation());
        }  
    }

    IEnumerator KeepRotation()
    {
        while(true)
        {
            var duation = Random.Range(mLowDuation,mHighDuation);
            yield return new WaitForSecondsRealtime(duation);
            transform.Rotate(0, 0, mrotation);
        }
    }
}
