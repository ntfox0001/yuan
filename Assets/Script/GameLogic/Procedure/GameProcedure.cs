using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProcedure : ProcedureBase
{
    public GlobalResHelper GlobalSceneResHelper { get; private set; }
    public GlobalResHelper GlobalUIResHelper { get; private set; }

    bool mSdkInitialSuccess = false;
    bool mGlobalResIntialSuccess = false;


    // 1， 加载global和第一个场景
    // 2， 创建第一个场景
    public override void Initial()
    {        
        StartCoroutine(GameInitCoroutine());
    }

    IEnumerator GameInitCoroutine()
    {
        yield return null;
        Debug.Log("Auth begin initial.");
        var authWait = AuthManager.GetSingleton().Auth.Initial();
        yield return authWait;
        while (true)
        {
            if (AuthManager.GetSingleton().Auth.IsInitialSuccessed())
            {
                Debug.Log("Auth initial success.");

                mSdkInitialSuccess = true;

                // 不管sdk成不成功，都继续加载
                WaitForProgressCallback<string> globalWindowWait = WindowManager.GetSingleton().LoadGlobalUI();
                WaitForProgressCallback<string> globalSceneWait = SceneManager.GetSingleton().LoadGlobalScene();
                // wait
                yield return new WaitForMultiObjects(WaitForMultiObjects.WaitForType.WaitForAll, globalWindowWait, globalSceneWait);
                if (globalSceneWait.Error == null && globalWindowWait.Error == null)
                {
                    WaitForProgressCallback<GameObject> globalSceneResHelperWait = SceneManager.GetSingleton().CreateGameObject(
                        "GlobalResHelper", SceneManager.GlobalSceneName);

                    WaitForProgressCallback<GameObject> globalUIResHelperWait = new WaitForProgressCallback<GameObject>();
                    ResourceManager.GetSingleton().CreateResourceAsync<GameObject>(
                        "GlobalResHelper", WindowManager.GlobalGroupName, globalUIResHelperWait.ProgressCallback);
                    yield return new WaitForMultiObjects(WaitForMultiObjects.WaitForType.WaitForAll, globalSceneResHelperWait, globalUIResHelperWait);

                    GlobalSceneResHelper = Instantiate(globalSceneResHelperWait.Target,
                        SceneManager.GetSingleton().gameObject.transform).GetComponent<GlobalResHelper>();
                    GlobalUIResHelper = Instantiate(globalUIResHelperWait.Target,
                        WindowManager.GetSingleton().gameObject.transform).GetComponent<GlobalResHelper>();

                    mGlobalResIntialSuccess = true;
                }
                else
                {
                    Debug.LogError("Load game error");
                }

                TouchFinished();
                break;
            }
            else
            {
                int waitUser = 0;
                Debug.LogError("failed to auth initial.");
                YesNoMsgBox.ShowTop(1227, YesNoMsgBox.ButtonStyle.OKCancel, (bool yes) =>
                {
                    if (yes)
                    {
                        waitUser = 1;
                    }
                    else
                    {
                        waitUser = -1;
                    }
                });
                yield return new WaitUntil(() =>
                {
                    return waitUser != 0;
                });
                if (waitUser > 0)
                {
                    yield return new WaitForSeconds(1.0f);
                }
                else
                {
                    Application.Quit();
                }
            }
        }
    }
    private void TouchFinished()
    {
        if (OnInitialFinished != null)
        {
            OnInitialFinished();
        }
    }

    // 请开始你的表演
    public override void Do()
    {
        // 加载opening窗口
        WindowManager.GetSingleton().ActiveWindowStack.CreateWindow("OpeningWindow", "opening", mSdkInitialSuccess, mGlobalResIntialSuccess);
    }
}
