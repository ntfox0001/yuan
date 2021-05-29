using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The unity thread dispatcher.
/// </summary>
[DisallowMultipleComponent]
public sealed class ActionDispatcher : MonoBehaviour
{
    private static ActionDispatcher mThis;

    // The thread safe task queue.
    private static List<Action> mPostTasks = new List<Action>();

    // The executing buffer.
    private static List<Action> mExecuting = new List<Action>();

    public static ActionDispatcher GetSingleton()
    {
        CheckInstance();
        return mThis;
    }

    /// <summary>
    /// Work thread post a task to the main thread.
    /// </summary>
    public void PostTask(Action task)
    {
        lock (mPostTasks)
        {
            mPostTasks.Add(task);
        }
    }

    /// <summary>
    /// Start to run this dispatcher.
    /// </summary>
    private static void CheckInstance()
    {
        if (mThis == null && Application.isPlaying)
        {
            var go = new GameObject("ActionDispatcher", typeof(ActionDispatcher));
            mThis = go.GetComponent<ActionDispatcher>();
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void OnDestroy()
    {
        mPostTasks.Clear();
        mExecuting.Clear();
        mThis = null;
    }

    private void Update()
    {
        lock (mPostTasks)
        {
            if (mPostTasks.Count > 0)
            {
                for (int i = 0; i < mPostTasks.Count; ++i)
                {
                    mExecuting.Add(mPostTasks[i]);
                }

                mPostTasks.Clear();
            }
        }

        for (int i = 0; i < mExecuting.Count; ++i)
        {
            var task = mExecuting[i];
            try
            {
                task();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message, this);
            }
        }

        mExecuting.Clear();
    }
}