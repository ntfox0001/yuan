using UnityEngine;
using System.Collections;

namespace uTools {
    public enum Direction
    {
		Reverse = -1,
		Toggle = 0,
		Forward = 1
	}

	public enum Trigger {
        None,
        OnStart,
        OnEnable,
		OnPointerEnter,
		OnPointerDown,
		OnPointerClick,
		OnPointerUp,
		OnPointerExit,
	}

    public enum ShakeType
    {
        ePosition,
        eScale,
        eRotation
    }

    public enum FadeState
    {
        FadeInBegin, // 开始淡入
        FadeInEnd,  // 淡入结束
        FadeOutBegin, // 开始淡出
        FadeOutEnd,     // 淡出结束
    }
}