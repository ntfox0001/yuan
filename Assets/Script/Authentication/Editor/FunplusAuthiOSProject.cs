using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.iOS;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public class FunplusAuthiOSProject
{
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget == BuildTarget.iOS)
        {
#if DREAM_MAINLAND
            //This is the default path to the default pbxproj file. Yours might be different
            string projectPath = "/Unity-iPhone.xcodeproj/project.pbxproj";
            //Default target name. Yours might be different
            string targetName = "Unity-iPhone";
            //Set the entitlements file name to what you want but make sure it has this extension
            string entitlementsFileName = "dreamdetective.entitlements";

            var entitlements = new ProjectCapabilityManager(pathToBuiltProject + projectPath, entitlementsFileName, targetName);
            entitlements.AddAssociatedDomains(new string[] { "applinks:sdk-unilinks.campfiregames.cn" });
            entitlements.AddSignInWithApple();
            //Apply
            entitlements.WriteToFile();
#elif DREAM_GLOBAL
            //This is the default path to the default pbxproj file. Yours might be different
            string projectPath = "/Unity-iPhone.xcodeproj/project.pbxproj";
            //Default target name. Yours might be different
            string targetName = "Unity-iPhone";
            //Set the entitlements file name to what you want but make sure it has this extension
            string entitlementsFileName = "dreamdetective.entitlements";

            var entitlements = new ProjectCapabilityManager(pathToBuiltProject + projectPath, entitlementsFileName, targetName);
            entitlements.AddSignInWithApple();
            //Apply
            entitlements.WriteToFile();
#endif
        }
    }
}
