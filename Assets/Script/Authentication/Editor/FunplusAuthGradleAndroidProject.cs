#if UNITY_ANDROID
using UnityEditor.Android;
using UnityEngine;
using System.IO;

public class FunplusAuthGradleAndroidProject : IPostGenerateGradleAndroidProject
{
    public int callbackOrder { get { return 0; } }

    public void OnPostGenerateGradleAndroidProject(string path)
    {
#if DREAM_GLOBAL
        //        // 拷贝sdk需要的json文件
        //        string src = Path.Combine(Application.streamingAssetsPath, "ddCommon.json");
        //        string dest = Path.Combine(path, Application.productName + "/src/main/assets/ddCommon.json");
        //        if (!File.Exists(dest))
        //        {
        //            File.Copy(src, dest, false);
        //            Debug.Log("Copy" + dest);
        //        }


        string gpPath = path + "\\gradle.properties";
        string gradleProperties = File.ReadAllText(gpPath);
        gradleProperties += "\nandroid.enableJetifier=true\nandroid.useAndroidX=true";

        File.WriteAllText(gpPath, gradleProperties);
#elif DREAM_MAINLAND
#if CHUANSHANJIA_UNITY
        if (!Directory.Exists(Path.Combine(path, "src/main/res/xml")))
        {
            Directory.CreateDirectory(Path.Combine(path, "src/main/res/xml"));
        }
        File.WriteAllText(Path.Combine(path, "src/main/res/xml/file_paths.xml"), "<?xml version=\"1.0\" encoding=\"utf-8\"?><paths xmlns:android=\"http://schemas.android.com/apk/res/android\"><external-files-path name=\"external_files_path\" path=\".\" /><!--为了适配所有路径可以设置 path=\".\"--></paths>");
#endif

#endif
    }
}

#endif