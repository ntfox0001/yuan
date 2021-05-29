using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VisableOnce), true)]
public class VisableOnceInspector : Editor
{
    SerializedProperty mVisableKeyProperty;
    void OnEnable()
    {
        mVisableKeyProperty = serializedObject.FindProperty("VisableKey");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        if (mVisableKeyProperty.stringValue == "")
        {
            mVisableKeyProperty.stringValue = System.Guid.NewGuid().ToString();
        }
        if (GUILayout.Button("Clear"))
        {
            PlayerPrefs.DeleteKey(mVisableKeyProperty.stringValue);
        }

        serializedObject.ApplyModifiedProperties();

        
    }
}