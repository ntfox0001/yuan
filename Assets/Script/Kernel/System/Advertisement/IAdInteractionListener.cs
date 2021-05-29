public interface IAdInteractionListener
{
    /// <summary>
    /// Invoke when load Ad error.
    /// </summary>
    void OnError(AdUnitBase self, int code, string message);

    /// <summary>
    /// Invoke when the Ad load success.
    /// </summary>
    void OnAdLoad(AdUnitBase self);

    /// <summary>
    /// The Ad loaded locally, user can play local video directly.
    /// </summary>
    void OnAdCached(AdUnitBase self);

    /// <summary>
    /// Invoke when the Ad is shown.
    /// </summary>
    void OnAdShow(AdUnitBase self);

    /// <summary>
    /// Invoke when the Ad video bar is clicked.
    /// </summary>
    void OnAdVideoBarClick(AdUnitBase self);

    /// <summary>
    /// Invoke when the Ad is closed.
    /// </summary>
    void OnAdClose(AdUnitBase self);

    /// <summary>
    /// Invoke when the video complete.
    /// </summary>
    void OnVideoComplete(AdUnitBase self);

    /// <summary>
    /// Invoke when the video has an error.
    /// </summary>
    void OnVideoError(AdUnitBase self);

    /// <summary>
    /// Invoke when the download process actived.
    /// </summary>
    void OnDownloadActive(string appName, AdUnitBase self);

    /// <summary>
    /// Invoke when the download process paused.
    /// </summary>
    void OnDownloadPaused(string appName, AdUnitBase self);

    /// <summary>
    /// Invoke when the download process failed.
    /// </summary>
    void OnDownloadFailed(string appName, AdUnitBase self);

    /// <summary>
    /// Invoke when the download process finished.
    /// </summary>
    void OnDownloadFinished(string appName, AdUnitBase self);

    /// <summary>
    /// Invoke when installed.
    /// </summary>
    void OnInstalled(string appName, AdUnitBase self);
}
