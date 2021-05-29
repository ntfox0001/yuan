using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundGroup
{
    protected AudioSource mAudioSource;
    protected bool mMutex = false;
    protected float mVolume = 0;
    protected bool mEnable = true;

    protected bool mMute = false;
    /// <summary>
    /// 返回一个当前SoundGroup的helper
    /// </summary>
    /// <param name="groupName"></param>
    /// <returns></returns>
    public SoundHelper GetSoundHelper(string groupName)
    {
        return new SoundHelper(this, groupName);
    }
    protected SoundGroup(bool mutex)
    {
        mMutex = mutex;
        if (mutex)
        {
            mAudioSource = SoundManager.GetSingleton().Get();
        }
        Volume = 1.0f;
        Enable = true;
        
    }
    public bool Mute
    {
        set
        {
            mMute = value;
            foreach (KeyValuePair<int, AudioSource> i in mLoopSoundMap)
            {
                i.Value.mute = value;
            }
        }
    }
    public float Volume
    {
        get
        {
            return mVolume;
        }

        set
        {
            mVolume = value;
            foreach (KeyValuePair<int, AudioSource> i in mLoopSoundMap)
            {
                i.Value.volume = mVolume;
            }
        }
    }
    public bool Enable
    {
        get
        {
            return mEnable;
        }
        set
        {
            mEnable = value;
            if (mEnable)
            {
                foreach (KeyValuePair<int, AudioSource> i in mLoopSoundMap)
                {
                    i.Value.Stop();
                }
            }
            else
            {
                foreach (KeyValuePair<int, AudioSource> i in mLoopSoundMap)
                {
                    i.Value.Play();
                }
            }
        }
    }

    Dictionary<int, AudioSource> mLoopSoundMap = new Dictionary<int, AudioSource>();
    int mLoopIndex = 1;

    public void Play(string clipName, string groupName)
    {
        if (!Enable) return;
        var clip = ResourceManager.GetSingleton().CreateResource<AudioClip>(clipName, groupName);
        Play(clip);
    }
    public void Play(AudioClip clip)
    {
        if (clip == null) return;
        if (!Enable) return;
        SoundManager.GetSingleton().StartCoroutine(PlaySound(clip, false, Volume));
    }
    IEnumerator PlaySound(AudioClip clip, bool loop, float volume)
    {
        AudioSource audioSource = GetAudioSource();
        audioSource.loop = loop;
        audioSource.volume = volume;
        audioSource.mute = mMute;
        audioSource.clip = clip;
        audioSource.Play();
        yield return new WaitForSecondsRealtime(clip.length);

        ReturnAudioSource(audioSource);
    }

    public int PlayLoop(string clipName, string groupName)
    {
        if (!Enable) return 0;
        var clip = ResourceManager.GetSingleton().CreateResource<AudioClip>(clipName, groupName);

        return PlayLoop(clip);
    }
    public int PlayLoop(AudioClip clip)
    {
        if (clip == null) return 0;
        if (!Enable) return 0;
        
        AudioSource audioSource = GetAudioSource();
        audioSource.loop = true;
        audioSource.volume = Volume;
        audioSource.clip = clip;
        audioSource.mute = mMute;
        audioSource.Play();
        mLoopSoundMap[mLoopIndex] = audioSource;

        return mLoopIndex++;
    }
    public void StopLoop(int id)
    {
        if (mLoopSoundMap.ContainsKey(id))
        {
            // 在编辑器下，global节点先于window节点销毁，会导致这里为空
            if (mLoopSoundMap[id] != null)
            {
                mLoopSoundMap[id].Stop();
                ReturnAudioSource(mLoopSoundMap[id]);
                mLoopSoundMap.Remove(id);
            }
        }
        else
        {
            Debug.LogError("SoundGroup: Not found Id:" + id);
        }
    }
    public void StopLoop(int id, float time)
    {
        if (mLoopSoundMap.ContainsKey(id))
        {
            // 关闭时，空指针调用
            if (SoundManager.GetSingleton() != null)
            {
                SoundManager.GetSingleton().StartCoroutine(StopLoopFadeOut(id, time));
            }
        }
        else
        {
            Debug.LogError("SoundGroup: Not found Id:" + id);
        }
    }
    IEnumerator StopLoopFadeOut(int id, float time)
    {
        uTools.TweenAudioVolume.Begin(mLoopSoundMap[id].gameObject, 1, 0, time);
        yield return new WaitForSecondsRealtime(time);
        ReturnAudioSource(mLoopSoundMap[id]);
        mLoopSoundMap[id].Stop();
        mLoopSoundMap.Remove(id);
    }
    protected void ReturnAudioSource(AudioSource audioSource)
    {
        if (mMutex) return;
        audioSource.clip = null;
        SoundManager.GetSingleton().Return(audioSource);
    }
    protected AudioSource GetAudioSource()
    {
        if (mMutex)
        {
            return mAudioSource;
        }
        else
        {
            return SoundManager.GetSingleton().Get();
        }
    }
}
