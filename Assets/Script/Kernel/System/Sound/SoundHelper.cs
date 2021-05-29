using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHelper
{
    SoundGroup mSoundGroup;
    string mGroupName;
    public SoundHelper(SoundGroup sg, string groupName)
    {
        mSoundGroup = sg;
        mGroupName = groupName;
    }

    public void Play(string clipName)
    {
        mSoundGroup.Play(clipName, mGroupName);
    }
    public void Play(AudioClip clip)
    {
        mSoundGroup.Play(clip);
    }
    public int PlayLoop(string clipName)
    {
        return mSoundGroup.PlayLoop(clipName, mGroupName);
    }
    public int PlayLoop(AudioClip clip)
    {
        return mSoundGroup.PlayLoop(clip);
    }
    public void StopLoop(int id)
    {
        mSoundGroup.StopLoop(id);
    }
    public void StopLoop(int id, float time)
    {
        mSoundGroup.StopLoop(id, time);
    }
}
