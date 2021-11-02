public interface ISwitch
{
    /// <summary>
    /// 开始淡入
    /// </summary>
    /// <param name="pre"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    WaitForMultiObjects.WaitReturn OnBeginFadeIn(ISwitchTarget pre, ISwitchTarget target);
    /// <summary>
    /// 淡入结束
    /// </summary>
    /// <param name="pre"></param>
    /// <param name="target"></param>
    /// <param name="onFadeInFunc">当淡入结束时被调用</param>
    /// <returns></returns>
    WaitForMultiObjects.WaitReturn OnEndFadeIn(ISwitchTarget pre, ISwitchTarget target, System.Action onFadeInFunc);
    /// <summary>
    /// 开始淡出
    /// </summary>
    /// <param name="pre"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    WaitForMultiObjects.WaitReturn OnBeginFadeOut(ISwitchTarget pre, ISwitchTarget target);
    /// <summary>
    /// 淡出结束
    /// </summary>
    /// <param name="pre"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    WaitForMultiObjects.WaitReturn OnEndFadeOut(ISwitchTarget pre, ISwitchTarget target);
    /// <summary>
    /// 返回最小loading时间，loadingWindow会读取这个值
    /// </summary>
    /// <returns></returns>
    float MinLoadingTime();
}