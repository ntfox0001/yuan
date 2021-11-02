public enum TargetType
{
    MainUI,      // 加入到堆栈
    None
}
public interface ISwitchTarget
{
    /// <summary>
    /// 开始加载这个target,要求可重入
    /// 如果返回continue，那么每帧都会被调用
    /// </summary>
    /// <returns></returns>
    WaitForMultiObjects.WaitReturn Preload();
    /// <summary>
    /// load结束
    /// 如果返回continue，那么每帧都会被调用
    /// </summary>
    /// <returns></returns>
    WaitForMultiObjects.WaitReturn Postload();
    /// <summary>
    /// 开始释放这个target
    /// </summary>
    void Release();
    /// <summary>
    /// target类型
    /// </summary>
    TargetType Type { get; }
    /// <summary>
    /// 设置额外参数
    /// </summary>
    /// <param name="param"></param>
    void SetParam(params object[] param);
}

