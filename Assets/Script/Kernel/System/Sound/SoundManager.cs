using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SoundManager : Singleton<SoundManager>, IManagerBase
{
    Cache<AudioSource> AudioCache { get; set; }
    public SoundGroup UI { get; private set; }
    //public SoundGroup BGM { get; private set; }
    public SoundGroup Scene { get; private set; }

    const string GameMuteKeyName = "__GameMuteKey__";

    int mGameMute = 0;
    bool mMute = false;
    /// <summary>
    /// GameSetupWindow专用
    /// </summary>
    public bool GameMute
    {
        get
        {
            return mGameMute == 1;
        }
        set
        {
            mGameMute = value ? 1 : 0;
            PlayerPrefs.SetInt(GameMuteKeyName, mGameMute);
            Mute = value;
        }
    }
    /// <summary>
    /// 游戏内逻辑使用
    /// </summary>
    public bool Mute
    {
        set
        {
            mMute = value;
            bool mute = mMute || mGameMute == 1;
            UI.Mute = mute;
            //BGM.Mute = mute;
            Scene.Mute = mute;
        }
    }
    public enum SoundChannelType
    {
        UI,BGM,Scene,
    }

    public void Initial()
    {
        AudioCache = new Cache<AudioSource>(NewAudioSource);

        UI = new SoundGroupOneShot();
        //BGM = new SoundGroupMutex();
        Scene = new SoundGroupOneShot();

        GameMute = PlayerPrefs.GetInt(GameMuteKeyName, 0) == 1;
    }

    public void Release()
    {
        
    }
    public AudioSource Get()
    {
        var source = AudioCache.Get();
        source.enabled = true;
        return source;
    }
    public void Return(AudioSource source)
    {
        source.enabled = false;
        AudioCache.Return(source);
    }

    public SoundGroup GetSoundGroup(SoundChannelType type)
    {
        switch (type)
        {
//             case SoundChannelType.BGM:
//                 {
//                     return BGM;
//                 }
            case SoundChannelType.Scene:
                {
                    return Scene;
                }
            case SoundChannelType.UI:
                {
                    return UI;
                }
        }
        return null;
    }
    public SoundHelper GetSoundHelper(SoundChannelType type, string groupName)
    {
        return GetSoundGroup(type).GetSoundHelper(groupName);
    }

    
    AudioSource NewAudioSource()
    {
        GameObject go = new GameObject("smgr_cache");
        AudioSource audioSource =  go.AddComponent<AudioSource>();
        go.transform.parent = this.transform;
        go.transform.position = Vector3.zero;
        go.transform.rotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;

        return audioSource;
    }
}
