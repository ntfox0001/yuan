using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ScreenOrientationMotifier), true)]
public class ScreenOrientationMotifierInspector : Editor
{
    SerializedProperty mOverrideManager;
    SerializedProperty mRunInPad;
    SerializedProperty mRunInPhone;

    protected virtual void OnEnable()
    {
        mOverrideManager = serializedObject.FindProperty("OverrideManager");
        mRunInPad = serializedObject.FindProperty("RunInPad");
        mRunInPhone = serializedObject.FindProperty("RunInPhone");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var modifier = (ScreenOrientationMotifier)serializedObject.targetObject;
        EditorGUILayout.PropertyField(mOverrideManager);
        if (mOverrideManager.boolValue)
        {
            EditorGUILayout.PropertyField(mRunInPad);
            EditorGUILayout.PropertyField(mRunInPhone);
        }

        if (modifier.Portrait == null)
        {
            modifier.Portrait = new ScreenOrientationMotifier.RectTransformRaw(modifier.transform as RectTransform);
        }
        GUILayout.Label("Portrait");
        EditorGUILayout.Vector2Field("anchoredPosition:", modifier.Portrait.anchoredPosition);
        EditorGUILayout.Vector2Field("sizeDelta:", modifier.Portrait.sizeDelta);
        EditorGUILayout.Vector2Field("anchorMin:", modifier.Portrait.anchorMin);
        EditorGUILayout.Vector2Field("anchorMax:", modifier.Portrait.anchorMax);
        EditorGUILayout.Vector2Field("pivot:", modifier.Portrait.pivot);
        EditorGUILayout.Vector3Field("rotation:", modifier.Portrait.rotation.eulerAngles);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("SetCurrent"))
        {
            modifier.Portrait = new ScreenOrientationMotifier.RectTransformRaw(modifier.transform as RectTransform);
        }
        if (GUILayout.Button("Use"))
        {
            modifier.Portrait.Apply(modifier.transform as RectTransform);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (modifier.Landscape == null)
        {
            modifier.Landscape = new ScreenOrientationMotifier.RectTransformRaw(modifier.transform as RectTransform);
        }
        GUILayout.Label("Landscape");
        EditorGUILayout.Vector2Field("anchoredPosition:", modifier.Landscape.anchoredPosition);
        EditorGUILayout.Vector2Field("sizeDelta:", modifier.Landscape.sizeDelta);
        EditorGUILayout.Vector2Field("anchorMin:", modifier.Landscape.anchorMin);
        EditorGUILayout.Vector2Field("anchorMax:", modifier.Landscape.anchorMax);
        EditorGUILayout.Vector2Field("pivot:", modifier.Landscape.pivot);
        EditorGUILayout.Vector3Field("rotation:", modifier.Landscape.rotation.eulerAngles);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("SetCurrent"))
        {
            modifier.Landscape = new ScreenOrientationMotifier.RectTransformRaw(modifier.transform as RectTransform);
        }
        if (GUILayout.Button("Use"))
        {
            modifier.Landscape.Apply(modifier.transform as RectTransform);
        }
        EditorGUILayout.EndHorizontal();


        serializedObject.ApplyModifiedProperties();
    }

}
