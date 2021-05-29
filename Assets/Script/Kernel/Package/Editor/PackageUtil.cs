using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

public class PackageUtil 
{
    public static string BuildAssetbundlePathBase = "../AssetBundles/";
    static BuildAssetBundleOptions BuildABOption = BuildAssetBundleOptions.StrictMode | BuildAssetBundleOptions.DeterministicAssetBundle;

#if UNITY_IOS
    const string CurrentTargetString = "iOS";
#elif UNITY_ANDROID
    const string CurrentTargetString = "Android";
#elif UNITY_STANDALONE_WIN
    const string CurrentTargetString =  "StandaloneWindows";
#elif UNITY_STANDALONE_OSX
    const string CurrentTargetString = "StandaloneOSX";
#endif

    const string MenuString = "Tools/Build AssetBundle/Build AssetBundle - " + CurrentTargetString;

    static BuildTarget BuildTarget
    {
        get
        {
            
#if UNITY_IOS
            return BuildTarget.iOS;
#elif UNITY_ANDROID
            return BuildTarget.Android;
#elif UNITY_STANDALONE_WIN
            return BuildTarget.StandaloneWindows64;
#elif UNITY_STANDALONE_OSX
            return BuildTarget.StandaloneOSXIntel64;
#endif
        }
    }

    class PackageSetting
    {
        public string Description;
        public List<KeyValuePair<string, string>> CopyFiles = new List<KeyValuePair<string, string>>();
        public List<string> StreamingFiles = new List<string>();
        public List<string> PreExecute = new List<string>();

        public string AssetBundleOutPath { get; private set; }

        public PackageSetting()
        {
            AssetBundleOutPath = BuildAssetbundlePathBase;

            var psFiles = Directory.GetFiles(Application.dataPath, "*.packagesetting", SearchOption.AllDirectories);
            if (psFiles.Length == 0)
            {
                Debug.LogError("Not found packagesetting file.");
                return;
            }
            if (psFiles.Length > 1)
            {
                Debug.LogError("Only first setting files processed: " + psFiles[0]);
            }

            string file = psFiles[0];
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(File.ReadAllText(file));

            var desc = doc.DocumentElement.SelectSingleNode("Description");
            Description = desc.InnerText;

            Debug.Log("-------------------------- < " + Description + " > -----------------------");

            var assetBundle = doc.DocumentElement.SelectSingleNode("AssetBundle");

            if (assetBundle != null && assetBundle.Attributes["outPath"] != null)
            {
                AssetBundleOutPath = assetBundle.Attributes["outPath"].InnerText;
            }
                       

            var copyFiles = doc.DocumentElement.SelectNodes("CopyFile");
            for (int i = 0; i < copyFiles.Count; i++)
            {
                var elem = (XmlElement)copyFiles[i];
                var src = elem.Attributes["src"];
                var dest = elem.Attributes["dest"];
                CopyFiles.Add(new KeyValuePair<string, string>(src.InnerText, dest.InnerText));
            }

            var preExec = doc.DocumentElement.SelectNodes("PreExecute");
            for (int i = 0; i < preExec.Count; i++)
            {
                var elem = (XmlElement)preExec[i];
                var item = elem.Attributes.GetNamedItem(Application.platform.ToString());
                if (item != null)
                {
                    PreExecute.Add(item.InnerText);
                }
            }

            var streamingFiles = doc.DocumentElement.SelectNodes("StreamingFile");
            for (int i = 0; i < streamingFiles.Count; i++)
            {
                var elem = (XmlElement)streamingFiles[i];
                StreamingFiles.Add(elem.InnerText);
            }
        }

        public void CopyFile()
        {
            foreach (var i in CopyFiles)
            {
                File.Copy(i.Key, i.Value, true);
            }
        }
        public void Execute()
        {
            for (int i = 0; i < PreExecute.Count; i++)
            {
                Debug.Log(ExecuteCommandSync(Path.GetFileName(PreExecute[i]), Path.GetDirectoryName(PreExecute[i])));
            }
        }
        public List<string> GetStreamingFiles()
        {
            List<string> sfs = new List<string>();
            foreach (var i in CopyFiles)
            {
                sfs.Add(Path.GetFileName(i.Value));
            }

            sfs.AddRange(StreamingFiles);
            return sfs;
        }

    }


    [MenuItem(MenuString)]
    static void GenerateAssetBundle()
    {
        PackageSetting ps = new PackageSetting();

        ps.CopyFile();
//        ps.Execute();

//#if UNITY_EDITOR_OSX
//        EditorUtility.DisplayDialog("wait for execute", "wait for execute", "ok");
//#endif
//        EditorUtility.FocusProjectWindow();
//        AssetDatabase.Refresh();


        string dir = GetBuildAssetBundleCacheDirectory(ps.AssetBundleOutPath);
        Directory.CreateDirectory(dir);
        BuildPipeline.BuildAssetBundles(dir, BuildABOption, BuildTarget);


        string packageDir = GetBuildAssetBundlePackageDirectory(ps.AssetBundleOutPath);

        string packagePath = packageDir + "/" + GetNextResourcesVersion(packageDir);
        Directory.CreateDirectory(packagePath);

        CleanDirectory("Assets/StreamingAssets", ps.GetStreamingFiles());

        BuildPackageFile(CopyFile(BuildTarget, dir, packagePath), packagePath);
        // 暂时不用产生这个文件
        //GenerateVersion(BuildTarget);
    }
    [MenuItem("Tools/Build AssetBundle/Exec Batch")]
    static void Execute()
    {
        PackageSetting ps = new PackageSetting();

        ps.CopyFile();
        ps.Execute();

        AssetDatabase.Refresh();
    }
    public static string GetBuildAssetBundleCacheDirectory(string path)
    {
        return path + "/Cache/" + CurrentTargetString;
    }
    public static string GetBuildAssetBundlePackageDirectory(string path)
    {
        return path + "/Package/" + CurrentTargetString;
    }
    static string GetNextResourcesVersion(string path)
    {
        string lastVersion = GetLastVersion(path);
        if (lastVersion == string.Empty)
        {
            // 如果没找到，那么使用第一个版本号
            lastVersion = "1";
            return lastVersion;
        }

        int v = int.Parse(lastVersion) + 1;
        
        return v.ToString();
    }
    // 获取当前平台最近一次版本号
    static string GetLastVersion(string path)
    {
        List<string> dirList = GetDirectoriesList(path);
        dirList.Sort(CompareVersion);
        if (dirList.Count == 0)
        {
            return string.Empty;
        }
        return dirList[dirList.Count - 1];
    }
    static List<string> GetDirectoriesList(string path)
    {
        List<string> dirlist = new List<string>();

        if (Directory.Exists(path))
        {
            string[] dirs = Directory.GetDirectories(path);
            for (int i = 0; i < dirs.Length; i++)
            {
                string dir = dirs[i];
                dir = dir.Replace('\\', '/');
                string[] s = dir.Split('/');
                dirlist.Add(s[s.Length - 1]);
            }
        }
        return dirlist;
    }
    static public int CompareVersion(string v1, string v2)
    {
        if (int.Parse(v1) > int.Parse(v2))
        {
            return 1;
        }
        else if (int.Parse(v1) < int.Parse(v2))
        {
            return -1;
        }
        else
        {
            return 0;
        }
        
    }
    static List<List<string>> CopyFile(BuildTarget target, string cachePath, string packagePath)
    {
        var packageFiles = new List<string>();
        var streamingFiles = new List<string>();
        var rgjs = File.ReadAllText(Path.Combine(Application.dataPath, "Tables/ResourceGroup.json"));
        LitJson.JsonData resGroupTable = LitJson.JsonMapper.ToObject(rgjs);

        string[] files = Directory.GetFiles(cachePath);
        
        foreach (string f in files)
        {
            string fullfn = Path.GetFileName(f);
            string fn = Path.GetFileNameWithoutExtension(f);
            string fext = Path.GetExtension(f);

            if (fext == ".manifest")
            {
                // 跳过manifest文件
                continue;
            }

            string found = "";
            foreach (var rg in resGroupTable.Keys)
            {
                if (MathFile(rg, fn))
                {
                    found = rg;
                }
            }
            if (!string.IsNullOrEmpty(found))
            {
                int download = int.Parse(resGroupTable[found]["Download"].GetString());
                switch (download)
                {
                    case 0:
                        {
                            // 拷贝到程序目录一起打包
                            string targetFile = "Assets/StreamingAssets/" + fullfn;
                            File.Copy(f, targetFile);
                            streamingFiles.Add(fn);
                            break;
                        }
                    case 1:
                        {
                            // 拷贝文件到网络打包目录
                            string targetFile = packagePath + "/" + fullfn;
                            File.Copy(f, targetFile);
                            packageFiles.Add(targetFile);
                            break;
                        }
                    default:
                        {
                            // 标记是2或者没出现在表里的，什么都不干
                            Debug.LogError("assetbundle ignore: " + f);
                            break;
                        }
                }
            }
            
            //if (resGroupTable.ContainsKey(fn) && resGroupTable[fn]["Download"].GetString() == "1")
            //{
            //    // 如果文件标记为1，那么拷贝文件到网络打包目录
            //    string targetFile = packagePath + "/" + fullfn;
            //    File.Copy(f, targetFile);
            //    packageFiles.Add(targetFile);
            //}
            //else if (resGroupTable.ContainsKey(fn) && resGroupTable[fn]["Download"].GetString() == "2")
            //{
            //    // 什么都不干
            //    Debug.LogError("assetbundle ignore: " + f);
            //}
            //else
            //{
            //    // 没出现在表里的和出现在表里，但是标记是0的
            //    // 否则拷贝到程序目录一起打包
            //    string targetFile = "Assets/StreamingAssets/" + fullfn;
            //    File.Copy(f, targetFile);
            //    streamingFiles.Add(fn);
            //}
        }
        

        var rtList = new List<List<string>>();

        rtList.Add(packageFiles);
        rtList.Add(streamingFiles);

        return rtList;
    }
    static bool MathFile(string math, string file)
    {
        if (math.Contains("*"))
        {
            var regex = math.Replace("*", ".*");
            return Regex.IsMatch(file, regex);
        }
        
        return math == file;
    }
    static void BuildPackageFile(List<List<string>> buildFiles, string packagePath)
    {
        var packageFiles = buildFiles[0];
        LitJson.JsonData md5jd = new LitJson.JsonData();
        LitJson.JsonData packageFilelist = new LitJson.JsonData();
        packageFilelist.SetJsonType(LitJson.JsonType.Object);

        for (int i = 0; i < packageFiles.Count; i++)
        {
            if (Path.GetExtension(packageFiles[i]) == "")
            {
                string fn = Path.GetFileNameWithoutExtension(packageFiles[i]);
                FileInfo fi = new FileInfo(packageFiles[i]);
                string filemd5 = MD5Utility.GetFastMD5HashFromFile(packageFiles[i]);
                LitJson.JsonData fileDesc = new LitJson.JsonData();
                fileDesc["md5"] = filemd5;
                fileDesc["len"] = fi.Length;
                packageFilelist[fn] = fileDesc;
            }
        }

        md5jd["filelist"] = packageFilelist;
        md5jd["ExportDate"] = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

        File.WriteAllText(packagePath + "/" + BackendDownloadManager.PackageFilename, md5jd.ToJson());

        var streamingFiles = buildFiles[1];
        LitJson.JsonData streamingFilelist = new LitJson.JsonData();
        for (int i = 0; i < streamingFiles.Count; i++)
        {
            if (Path.GetExtension(streamingFiles[i]) == "")
            {
                // 资源文件
                streamingFilelist[streamingFiles[i]] = 1;
            }
        }
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, AssetBundleLoader.StreamingListFilename), streamingFilelist.ToJson());
    }

    static void CleanDirectory(string directory, List<string> streamingFiles)
    {
        string[] files = Directory.GetFiles(directory);
        foreach (string f in files)
        {
            // 不要删除文件
            bool isIgnore = false;
            for (int i = 0; i < streamingFiles.Count; i++)
            {
                if (Path.GetFileName(f) == streamingFiles[i])
                {
                    isIgnore = true;
                    break;
                }
            }
            if (!isIgnore)
            {
                File.Delete(f);
            }
        }
    }
    [MenuItem("Tools/Package/GenerateRandomFile")]
    static void PackageRandomFile()
    {
        string fn = "../Design/RandomData/RandomArray.dump";
        BytesObject obj = ScriptableObject.CreateInstance<BytesObject>();
        obj.Name = "RandomArray";
        obj.Content = File.ReadAllBytes(fn);
        AssetDatabase.CreateAsset(obj, "Assets/GameRandom/RandomArray.asset");
    }
    static void GenerateVersion(BuildTarget target)
    {
        string src,dest,fn;
        switch (target)
        {
            case BuildTarget.Android:
                {
                    fn = "androidVersion.json";
                    break;
                }
            case BuildTarget.iOS:
                {
                    fn = "iosVersion.json";
                    break;
                }
            default:
                {
                    Debug.LogError("unknow target:" + target.ToString());
                    return;
                }
        }
        src = Application.dataPath + "/Version/" + fn;
        dest = Application.streamingAssetsPath + "/" + fn;

        LitJson.JsonData ver;
        if (File.Exists(src))
        {
            ver = LitJson.JsonMapper.ToObject(File.ReadAllText(src));
        }
        else
        {
            ver = new LitJson.JsonData();
            ver["version"] = 0;
        }
        ver["version"] = ver["version"].GetInt() + 1;
        File.WriteAllText(src, ver.ToJson());
        File.Copy(src, dest);
    }
    //#if UNITY_IOS
    //    [MenuItem("Tools/AddBuild",false,1002)]
    //    static void AddBuildNum()
    //    {
    //        int num  = int.Parse(PlayerSettings.iOS.buildNumber) + 1;
    //        PlayerSettings.iOS.buildNumber = num.ToString();
    //    }
    //#endif
    //    [MenuItem("Tools/AddVersion",false,1001)]
    //    static void AddBundleVersion()
    //    {
    //        float version = float.Parse(PlayerSettings.bundleVersion) + 0.01f;
    //        PlayerSettings.bundleVersion = version.ToString();
    //    }

    static public string ExecuteCommandSync(string command, string workDir)
    {
        //try
        {
            // create the ProcessStartInfo using "cmd" as the program to be run,
            // and "/c " as the parameters.
            // Incidentally, /c tells cmd that we want it to execute the command that follows,
            // and then exit.
            System.Diagnostics.ProcessStartInfo procStartInfo =
                new System.Diagnostics.ProcessStartInfo();
#if UNITY_EDITOR_WIN
            procStartInfo.FileName = "cmd";
            procStartInfo.Arguments = "/c " + command;
#elif UNITY_EDITOR_OSX
                procStartInfo.FileName = "/bin/bash";
                procStartInfo.Arguments = " -c \"./" + command + "\"";
#else
#error "No Define"
#endif
            Debug.Log("shell: " + workDir + "/>" + command);
            procStartInfo.WorkingDirectory = workDir;
            // The following commands are needed to redirect the standard output.
            // This means that it will be redirected to the Process.StandardOutput StreamReader.
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            // Do not create the black window.
            procStartInfo.CreateNoWindow = false;
            // Now we create a process, assign its ProcessStartInfo and start it
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.OutputDataReceived += (object sender, System.Diagnostics.DataReceivedEventArgs e)=> 
            {
                Debug.Log(e.Data);
            };
            proc.ErrorDataReceived += (object sender, System.Diagnostics.DataReceivedEventArgs e) => 
            {
                Debug.LogError(e.Data);
            };
            proc.Start();
            // Get the output into a string
            string result = "";
            result = proc.StandardOutput.ReadToEnd();
            // Display the command output.
            return result;
        }
        // catch (System.Exception objException)
        // {
        //     // Log the exception
        //     Debug.LogError(objException.Message);
        // }
        // return "error";
    }
    //[MenuItem("test/test")]
    static void Test()
    {
        var buildfile = Directory.GetFiles(Application.dataPath, "*.buildxcode", SearchOption.AllDirectories);
        for (int k = 0; k < buildfile.Length; k++)
        {
            string buildSettingFilename = buildfile[k];

            XmlDocument buildSettingDoc = new XmlDocument();
            buildSettingDoc.Load(buildSettingFilename);

            XmlNodeList shellList = buildSettingDoc.DocumentElement.GetElementsByTagName("Shell");

            for (int i = 0; i < shellList.Count; i++)
            {
                XmlElement elem = (XmlElement)shellList[i];
                Debug.Log(PackageUtil.ExecuteCommandSync(elem.InnerText, "/Users/zhaoma/Documents/GitHub2/xcodeProj/errcode"));
            }
        }
    }

}
