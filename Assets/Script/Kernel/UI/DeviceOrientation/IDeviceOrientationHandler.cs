using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 设备旋转 handler
/// </summary>
public interface IDeviceOrientationHandler 
{
    void OnDeviceOrientation(ScreenOrientation ori);
}
