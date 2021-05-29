using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySubtitles : MonoBehaviour
{
    [System.Serializable]
    public class SubtitlesUnit
    {
        public LanguageTextBox Text;
        public uTools.TweenAlpha TextTweenAlpla;
    }
    public SubtitlesUnit CenterPos;
    public SubtitlesUnit BottomPos;
    /// <summary>
    /// 开始播放字幕
    /// </summary>
    /// <param name="subtitles">字幕数据</param>
    /// <param name="perTime">每行字持续时间</param>
    public void Play(Subtitles subtitles)
    {
        StartCoroutine(PlayCoroutine(subtitles));
    }
    SubtitlesUnit GetUnit(Subtitles.Line line)
    {
        switch (line.Pos)
        {
            case Subtitles.Position.Bottom:
                {
                    return BottomPos;
                }
            case Subtitles.Position.Center:
                {
                    return CenterPos;
                }
        }
        return null;
    }
    public void Stop()
    {
        if (CenterPos.Text != null)
        {
            CenterPos.Text.TextId = 0;
        }
        if (BottomPos.Text != null)
        {
            BottomPos.Text.TextId = 0;
        }
    }
    IEnumerator PlayCoroutine(Subtitles subtitles)
    {
        float preTime = 0;
        for (int i = 0; i < subtitles.Trace.Length - 1; i++)
        {
            var line = subtitles.Trace[i];
            float t = line.Time - preTime;
            if (t < 0)
            {
                Debug.LogError("Invalid to subtitles time.");
                continue;
            }
            else if (t > 0)
            {
                yield return new WaitForSecondsRealtime(t);
            }

            ShowText(GetUnit(line), line.TextId, subtitles.ShowTime);
            if (subtitles.Trace[i + 1].Time - line.Time > subtitles.ShowTime)
            {
                preTime = line.Time + subtitles.ShowTime;
                yield return new WaitForSecondsRealtime(subtitles.ShowTime);
                ShowText(GetUnit(line), 0, subtitles.ShowTime);
            }
            else
            {
                preTime = subtitles.Trace[i].Time;
            }
        }

        float t2 = subtitles.Trace[subtitles.Trace.Length - 1].Time - preTime;

        if (t2 > 0)
        {
            yield return new WaitForSecondsRealtime(t2);
        }
        if (t2 < 0)
        {
            Debug.LogError("Invalid to subtitles time.");
        }
        else
        {
            // 最后一个，没有next time
            ShowText(GetUnit(subtitles.Trace[subtitles.Trace.Length - 1]), subtitles.Trace[subtitles.Trace.Length - 1].TextId, subtitles.ShowTime);
            yield return new WaitForSecondsRealtime(subtitles.ShowTime);
            ShowText(GetUnit(subtitles.Trace[subtitles.Trace.Length - 1]), 0, subtitles.ShowTime);
        }
    }
    public void ShowText(SubtitlesUnit unit, int textId, float time)
    {
        unit.TextTweenAlpla.ResetToBeginning();
        unit.TextTweenAlpla.duration = time;
        unit.TextTweenAlpla.PlayForward();
        unit.Text.TextId = 0;
        StartCoroutine(ShowTextCoroutine(unit, textId));
    }
    IEnumerator ShowTextCoroutine(SubtitlesUnit unit, int textId)
    {
        yield return null;
        unit.Text.TextId = textId;

    }

}
