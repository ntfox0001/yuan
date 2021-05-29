#if UNITY_IOS
using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

using UnityEditor.iOS.Xcode;

public class BuildXcode
{
    const string VersionFilename = "./BuildSetting/XCode/version.json";
    //由FunplusSdk而来，需要和funplusSdk保持一致
    public static readonly string IMPORT_SDK = @"#import <FunplusSdk/FunplusSdk.h>";

    public static readonly string SEND_NOTIFICATION = @"
	AppController_SendNotificationWithArg(kUnityOnOpenURL, notifData);
	return [[FunplusSdk sharedInstance] appDidOpenURL:url application:application sourceApp:sourceApplication annotation:annotation];";

    [PostProcessBuild(99)]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            //ModifyAppController(path);
            //string buildSettingFilename = EditorUtility.OpenFilePanelWithFilters("Open BuildSetting Filename", "BuildSetting/XCode", new string[] { "PostBuild file", "xml" });
            //if (buildSettingFilename == "")
            //{
            //    return;
            //}
            //             var verJd = LitJson.JsonMapper.ToObject(File.ReadAllText(VersionFilename));
            //             verJd["build"] = verJd["build"].GetInt() + 1;
            //             File.WriteAllText(VersionFilename, verJd.ToJson());

            string projPath = PBXProject.GetPBXProjectPath(path);
            PBXProject proj = new PBXProject();
            proj.ReadFromFile(projPath);
            PlistDocument plistDoc = new PlistDocument();
            plistDoc.ReadFromFile(path + "/info.plist");
            string target = proj.TargetGuidByName("Unity-iPhone");

            var buildfile = Directory.GetFiles(Application.dataPath, "*.buildxcode", SearchOption.AllDirectories);
            for (int k = 0; k < buildfile.Length; k++)
            {
                string buildSettingFilename = buildfile[k];

                XmlDocument buildSettingDoc = new XmlDocument();
                buildSettingDoc.Load(buildSettingFilename);

                XmlNodeList frameworkList = buildSettingDoc.DocumentElement.GetElementsByTagName("Framework");
                XmlNodeList libraryList = buildSettingDoc.DocumentElement.GetElementsByTagName("Library");
                XmlNodeList fileList = buildSettingDoc.DocumentElement.GetElementsByTagName("File");
                XmlNodeList urlTypeList = buildSettingDoc.DocumentElement.GetElementsByTagName("UrlType");
                XmlNodeList qschemeList = buildSettingDoc.DocumentElement.GetElementsByTagName("QueriesScheme");
                XmlNodeList propList = buildSettingDoc.DocumentElement.GetElementsByTagName("Property");
                XmlNodeList plistList = buildSettingDoc.DocumentElement.GetElementsByTagName("PList");
                

                for (int i = 0; i < frameworkList.Count; i++)
                {
                    XmlElement elem = (XmlElement)frameworkList[i];
                    proj.AddFrameworkToProject(target, elem.InnerText, elem.GetAttribute("weak") == "true");
                }

                string srcPath = Path.GetDirectoryName(buildSettingFilename);
                for (int i = 0; i < fileList.Count; i++)
                {
                    XmlElement elem = (XmlElement)fileList[i];
                    if (!Directory.Exists(Path.GetDirectoryName(Path.Combine(path ,elem.InnerText))))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(path ,elem.InnerText)));
                    }
                    if (File.Exists(Path.Combine(srcPath ,elem.InnerText)))
                    {
                        File.Copy(Path.Combine(srcPath ,elem.InnerText), Path.Combine(path, elem.InnerText), true);
                    }
                    else if (Directory.Exists(Path.Combine(srcPath ,elem.InnerText)))
                    {
                        CopyDirectory(Path.Combine(srcPath ,elem.InnerText), Path.Combine(path, elem.InnerText));
                    }

                    proj.AddFileToBuild(target, proj.AddFile(elem.InnerText, elem.InnerText, PBXSourceTree.Source));

                }
                for (int i = 0; i < libraryList.Count; i++)
                {
                    XmlElement elem = (XmlElement)libraryList[i];
                    AddLibToProject(proj, target, elem.InnerText);
                }
                for (int i = 0; i < propList.Count; i++)
                {
                    XmlElement elem = (XmlElement)propList[i];
                    string name = elem.GetAttribute("name");
                    string value = elem.GetAttribute("value");
                    proj.SetBuildProperty(target, name, value);
                }

                plistDoc.ReadFromFile(path + "/info.plist");

                for (int i = 0; i < plistList.Count; i++)
                {
                    XmlElement elem = (XmlElement)plistList[i];
                    string name = elem.GetAttribute("name");
                    string value = elem.GetAttribute("value");
                    plistDoc.root.SetString(name, value);
                }
                //在InstallSdkXcode之后，所以不需要校验
                // ********** LSApplicationQueriesSchemes ********** //
                var schemes = plistDoc.root["LSApplicationQueriesSchemes"];
                if (schemes != null)
                {
                    PlistElementArray applicationQueriesSchemes = schemes.AsArray();
                    for (int i = 0; i < qschemeList.Count; i++)
                    {
                        XmlElement elem = (XmlElement)qschemeList[i];
                        string value = elem.GetAttribute("value");
                        applicationQueriesSchemes.AddString(value);
                    }
                }

                // ********** CFBundleURLTypes ********* //
                for (int i = 0; i < urlTypeList.Count; i++)
                {
                    XmlElement elem = (XmlElement)urlTypeList[i];
                    string name = elem.GetAttribute("name");
                    string value = elem.GetAttribute("value");
                    PlistElementArray bundleUrlTypes = plistDoc.root["CFBundleURLTypes"].AsArray();
                    PlistElementDict bundleUrlSchemes = bundleUrlTypes.AddDict();
                    PlistElementArray bundleUrlSchemesArray = null;
                    bundleUrlSchemes.SetString("CFBundleURLName", name);
                    bundleUrlSchemesArray = bundleUrlSchemes.CreateArray("CFBundleURLSchemes");
                    bundleUrlSchemesArray.AddString(value);
                }
            }

            plistDoc.root.values.Remove("UIApplicationExitsOnSuspend");

            plistDoc.root.SetString("CFBundleShortVersionString", PlayerSettings.bundleVersion);
            plistDoc.root.SetString("CFBundleVersion", PlayerSettings.iOS.buildNumber);
            plistDoc.WriteToFile(path + "/info.plist");

            File.WriteAllText(projPath, proj.WriteToString());

            for (int k = 0; k < buildfile.Length; k++)
            {
                string buildSettingFilename = buildfile[k];

                XmlDocument buildSettingDoc = new XmlDocument();
                buildSettingDoc.Load(buildSettingFilename);

                XmlNodeList shellList = buildSettingDoc.DocumentElement.GetElementsByTagName("Shell");

                for (int i = 0; i < shellList.Count; i++)
                {
                    XmlElement elem = (XmlElement)shellList[i];
                    Debug.Log(PackageUtil.ExecuteCommandSync(elem.InnerText, path));
                }
            }

        }
    }
    static string[] SKIP_FILES = { @".*\.meta$" };
    static void CopyDirectory(string srcDirName, string dstDirName)
    {
        
        DirectoryInfo dir = new DirectoryInfo(srcDirName);

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + srcDirName);
        }

        if (!Directory.Exists(dstDirName))
        {
            Directory.CreateDirectory(dstDirName);
        }

        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            if (SKIP_FILES != null)
            {
                bool skip = false;

                foreach (string pattern in SKIP_FILES)
                {
                    if (Regex.IsMatch(file.FullName, pattern, RegexOptions.IgnoreCase))
                    {
                        skip = true;
                        break;
                    }
                }

                if (skip)
                {
                    continue;
                }
            }

            file.CopyTo(Path.Combine(dstDirName, file.Name));
        }

        DirectoryInfo[] subdirs = dir.GetDirectories();

        foreach (DirectoryInfo subdir in subdirs)
        {
            if (SKIP_FILES != null)
            {
                bool skip = false;

                foreach (var pattern in SKIP_FILES)
                {
                    if (Regex.IsMatch(subdir.FullName, pattern, RegexOptions.IgnoreCase))
                    {
                        skip = true;
                        break;
                    }
                }

                if (skip)
                {
                    continue;
                }
            }

            CopyDirectory(subdir.FullName, Path.Combine(dstDirName, subdir.Name));
        }
    }
    //添加lib方法
    static void AddLibToProject(PBXProject inst, string targetGuid, string lib)
    {
        string fileGuid = inst.AddFile("usr/lib/" + lib, "Frameworks/" + lib, PBXSourceTree.Sdk);
        inst.AddFileToBuild(targetGuid, fileGuid);
    }
    //private static void ModifyAppController(string BuildPath)
    //{
    //    string appControllerField = "Classes/UnityAppController.mm";
    //    string appControllerClass = Path.Combine(BuildPath, appControllerField);

    //    if (!File.Exists(appControllerClass))
    //    {
    //        Debug.Log("file UnityAppController.mm don't exist!!!");
    //        return;
    //    }

    //    FileModifier appController = new FileModifier(appControllerClass);

    //    appController.InsertAfter(
    //       IMPORT_SDK,
    //       "#import \"NiNiShareDelegate.h\""
    //   );

    //    appController.InsertAfter(
    //        IMPORT_SDK,
    //        "#import \"DouyinOpenSDKApi.h\""
    //    );

    //    appController.InsertBefore(
    //       SEND_NOTIFICATION,
    //       "[DouyinOpenSDKApi handleOpenURL:url delegate:[NiNiShareDelegate sharedInstance]];"
    //   );

    //    appController.Write();
    //}
    public class FileModifier : System.IDisposable
    {

        private string filePath;
        private string fileContent;

        public FileModifier(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(
                    "Target file does not exist: "
                    + filePath);
            }

            //			UnityEngine.Debug.Log ("Opening file: " + filePath);

            this.filePath = filePath;
            StreamReader streamReader = new StreamReader(filePath);
            fileContent = streamReader.ReadToEnd();
            streamReader.Close();
        }

        /// <summary>
        /// Check if file contains contents of specific pattern.
        /// </summary>
        /// <param name="pattern">Pattern.</param>
        public bool Contains(string pattern)
        {
            return fileContent.Contains(pattern);
        }

        public void InsertAfter(string pattern, string textToInsert)
        {
            Replace(pattern, string.Format("{0}\n{1}", pattern, textToInsert));
        }

        public void InsertAfterIfNotExist(string pattern, string textToInsert)
        {
            if (!Contains(textToInsert))
            {
                InsertAfter(pattern, textToInsert);
            }
        }

        public void InsertBefore(string pattern, string textToInsert)
        {
            Replace(pattern, string.Format("{0}\n{1}", textToInsert, pattern));
        }

        public void Replace(string pattern, string newText)
        {
            //			fileContent = fileContent.Replace (pattern, newText);
            var regex = new Regex(Regex.Escape(pattern));
            fileContent = regex.Replace(fileContent, newText, 1);
        }

        public void Append(string newText)
        {
            fileContent += '\n' + newText;
        }

        public void Write()
        {
            //			UnityEngine.Debug.Log ("Writing to file: " + filePath);

            StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.Write(fileContent);
            streamWriter.Close();
        }

        public void Dispose()
        {
        }
    }
}

#endif