using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;
using UnityEditor;

[CustomEditor(typeof(MapScrollView), true)]
public class MapScrollViewInspector : ScrollViewBaseInspector
{
    SerializedProperty mMouseScaleRatio;
    SerializedProperty mTouchScaleRatio;
    SerializedProperty mMaxScaleRatio;
    SerializedProperty mMinScaleRatio;
    SerializedProperty mScaleDuration;
    SerializedProperty mMaxScaleLimit;
    SerializedProperty mMinScaleLimit;
    protected override void OnEnable()
    {
        base.OnEnable();
        mMouseScaleRatio = serializedObject.FindProperty("MouseScaleRatio");
        mTouchScaleRatio = serializedObject.FindProperty("TouchScaleRatio");
        mMaxScaleRatio = serializedObject.FindProperty("MaxScaleRatio");
        mMinScaleRatio = serializedObject.FindProperty("MinScaleRatio");
        mScaleDuration = serializedObject.FindProperty("ScaleDuration");
        mMaxScaleLimit = serializedObject.FindProperty("MaxScaleLimit");
        mMinScaleLimit = serializedObject.FindProperty("MinScaleLimit");
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.PropertyField(mMouseScaleRatio);
        EditorGUILayout.PropertyField(mTouchScaleRatio);
        EditorGUILayout.PropertyField(mMaxScaleRatio);
        EditorGUILayout.PropertyField(mMinScaleRatio);
        EditorGUILayout.PropertyField(mScaleDuration);
        EditorGUILayout.PropertyField(mMaxScaleLimit);
        EditorGUILayout.PropertyField(mMinScaleLimit);
        serializedObject.ApplyModifiedProperties();
    }
}
