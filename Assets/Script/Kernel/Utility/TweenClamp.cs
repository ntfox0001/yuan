using UnityEngine;
using System.Collections;

public class TweenClamp
{
    // 表示没超过一个单位，需要的offset
    float mTweenCoefficient = 0.1f;

    // 当前位置
    float mPosition;
    float mClampPosition;
    float mOffset;

    // 用户输入速度
    float mUserSpeed;
    // 自有惯性速度
    float mInertiaSpeed;
    // 自有惯性衰减速度
    float mInertiaAttenuationSpeed;
    void SetSpeed(float speed)
    {
        mUserSpeed = speed;
    }

    void Update()
    {
        float currentSpeed;
        // 计算当前惯性
        mInertiaSpeed -= mInertiaAttenuationSpeed;
        currentSpeed = mInertiaSpeed > mUserSpeed ? mInertiaSpeed : mUserSpeed;
        
        // 刷新惯性速度
        mInertiaSpeed = currentSpeed;


        


    }

    // 输入0-1的值，返回阻力速度
    float GetClampInertiaSpeed(float tc)
    {
        if (tc >= 1.0f) tc = 0.999f;
        if (tc < 0) tc = 0;
        return Mathf.Tan(tc);
    }

}
