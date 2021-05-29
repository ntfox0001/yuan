using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameSvrCompressionCallback : GameSvrCallback
{
    protected override void FinishFunc(UnityWebRequest req, object userData)
    {
        IsDone = true;
        string content;
        CompressManager.DecompressString(req.downloadHandler.data, out content, CompressManager.Decompresszlib);

        LitJson.JsonData msg = LitJson.JsonMapper.ToObject(content);

        Msg = msg;
        ErrorId = Msg["errorId"].GetString();

        if (mFinishFunc != null)
        {
            mFinishFunc(msg, userData);
        }
    }
}
