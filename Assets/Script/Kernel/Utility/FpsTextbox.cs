using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FpsTextbox : MonoBehaviour
{
    public Text FpsText;
    public float showTime = 1f;

    private int count = 0;
    private float deltaTime = 0f;
    string strFpsInfo = "0";
    // Update is called once per frame
    void Update()
    {
        count++;
        deltaTime += Time.deltaTime;
        
        if (deltaTime >= showTime)
        {
            float fps = count / deltaTime;
            float milliSecond = deltaTime * 1000 / count;
            strFpsInfo = string.Format("{1:0.}({0:0.0}ms)", milliSecond, fps);
            count = 0;
            deltaTime = 0f;
        }

        FpsText.text = string.Format("{0}, Screen:{1}x{2}\n{3}", strFpsInfo, Screen.width, Screen.height, SystemInfo.deviceModel);
    }
}
