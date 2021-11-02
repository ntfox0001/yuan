using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTarget : ISwitchTarget
{
    int mTargetSceneId;
    SceneInfo mSceneInfo;
    SceneData mSceneData;
    Scene mScene;
    bool mResLoadFinished = false;
    public TargetType Type
    {
        get
        {
            return TargetType.None;
        }
    }
    public SceneTarget(int targetSceneId)
    {
        mTargetSceneId = targetSceneId;
    }
    public void SetParam(params object[] param)
    {
        
    }
    public WaitForMultiObjects.WaitReturn Preload()
    {
        SceneInfo SceneInfoShowType = null;//判断显示类型用
        // 启动场景加载
        mSceneInfo = SceneManager.GetSingleton().GetSceneInfo(mTargetSceneId);

        //判断场景类型
        int mainSceneId = SceneManager.GetSingleton().GetMainIdByRelationId(mTargetSceneId);

        if (mainSceneId != mTargetSceneId)
        {//非主场景
            SceneInfoShowType = SceneManager.GetSingleton().GetSceneInfo(mainSceneId);
            Myself.GetSingleton().PlayerData.SetLastSceneId(mainSceneId);
        }
        else
        {
            SceneInfoShowType = mSceneInfo;
            mainSceneId = mTargetSceneId;
            //lastsceneid作为区分第一二页的标记存在，当新增页面时 需重构逻辑
            Myself.GetSingleton().PlayerData.SetLastSceneId(mTargetSceneId);
        }
            

        if (SceneInfoShowType.ShowType == SceneInfo.SceneShowType.SST_SHOWINFIRST)
        {
            Myself.GetSingleton().PlayerData.SetLastNoramlSceneId(mainSceneId);
        }
        else if (SceneInfoShowType.ShowType == SceneInfo.SceneShowType.SST_SHOWINSECOND)
        {
            Myself.GetSingleton().PlayerData.SetLastMasterSceneId(mainSceneId);
        }
        else if (SceneInfoShowType.ShowType == SceneInfo.SceneShowType.SST_SHOWINTHIRD)
        {
            Myself.GetSingleton().PlayerData.SetLastPlayerMakerSceneId(mainSceneId);
        }

        mSceneData = Myself.GetSingleton().PlayerData.GetOrNewScene(mSceneInfo.SceneName);
        LoadingManager.GetSingleton().StartCoroutine(StartResLoad());

        return WaitForMultiObjects.WaitReturn.Continue;
    }
    IEnumerator StartResLoad()
    {
        WaitForProgressCallback<string> targetSceneInfoWait = SceneManager.GetSingleton().LoadScene(mSceneInfo);
        // wait...
        yield return targetSceneInfoWait;

        if (targetSceneInfoWait.Error == null)
        {
            // 创建第一个场景
            WaitForProgressCallback<GameObject> targetSceneWait = SceneManager.GetSingleton().CreateGameObject(
                mSceneInfo.GetScenePrefabName(mSceneData), mSceneInfo.GroupName);
            yield return targetSceneWait;

            GameObject scenego = GameObject.Instantiate(targetSceneWait.Target, SceneManager.GetSingleton().SceneRoot.transform);
            mScene = scenego.GetComponent<Scene>();
            mScene.SceneInitial(mSceneInfo, mSceneData);
        }
        else
        {
            Debug.LogError("Load scene error");
        }
        mResLoadFinished = true;
    }

    public WaitForMultiObjects.WaitReturn Postload()
    {
        if (mResLoadFinished)
        {
            mScene.Do();
            return WaitForMultiObjects.WaitReturn.Continue;
        }
        else
        {
            return WaitForMultiObjects.WaitReturn.Wait;
        }
    }
    public void Release()
    {
        mScene.Release();
        Object.Destroy(mScene.gameObject);
        SceneManager.GetSingleton().UnloadScene(mSceneInfo.GroupName);
    }


}
