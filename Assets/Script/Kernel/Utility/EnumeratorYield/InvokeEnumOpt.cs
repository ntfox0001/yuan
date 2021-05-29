
public class InvokeEnumOpt : IEnumOpt
{
    protected bool mFinished = false;
    protected bool mSuccessed = false;


    /// <summary>
    /// 清除所有状态
    /// </summary>
    public void Clear()
    {
        mFinished = false;
        mSuccessed = false;
    }
    /// <summary>
    /// 设置完成
    /// </summary>
    /// <param name="success">是否成功</param>
    public void SetFinish(bool success)
    {
        mFinished = true;
        mSuccessed = success;
    }
    /// <summary>
    /// 检查是否完成
    /// </summary>
    public bool Finished { get { return mFinished; } }
    /// <summary>
    /// 检查是否成功
    /// </summary>
    public bool Successed { get { return mSuccessed; } }
}
