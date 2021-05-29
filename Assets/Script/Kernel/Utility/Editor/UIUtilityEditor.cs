using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEditor;
public class UIUtilityEditor
{
    [MenuItem("Tools/UI/DisableRaycastTarget")]
    static void DisableRaycastTarget()
    {
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            var texts = Selection.gameObjects[i].GetComponentsInChildren<Text>();
            for (int t = 0; t < texts.Length; t++)
            {
                texts[t].raycastTarget = false;
            }

            var images = Selection.gameObjects[i].GetComponentsInChildren<Image>();
            for (int m = 0; m < texts.Length; m++)
            {
                images[m].raycastTarget = false;
            }
        }
    }
    [MenuItem("Tools/UI/EnableRaycastTarget")]
    static void EnableRaycastTarget()
    {
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            var texts = Selection.gameObjects[i].GetComponentsInChildren<Text>();
            for (int t = 0; t < texts.Length; t++)
            {
                texts[t].raycastTarget = true;
            }

            var images = Selection.gameObjects[i].GetComponentsInChildren<Image>();
            for (int m = 0; m < texts.Length; m++)
            {
                images[m].raycastTarget = true;
            }
        }
    }
}
