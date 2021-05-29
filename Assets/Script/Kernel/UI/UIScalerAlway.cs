using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScalerAlway : UIScaler
{

    // Update is called once per frame
    void LateUpdate()
    {
        Scale();
    }
}
