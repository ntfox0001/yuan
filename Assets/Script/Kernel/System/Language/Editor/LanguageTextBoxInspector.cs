using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LanguageTextBox), true)]
public class LanguageTextBoxInspector : Editor
{
    SerializedProperty mTextUIProperty;
    SerializedProperty mTextIdProperty;
    SerializedProperty mTextBreakOffProperty;
    SerializedProperty mTextCharBreakOffProperty;
    SerializedProperty mTextWordBreakOffProperty;
    SerializedProperty mTextParamsProperty;
    private void OnEnable()
    {
        mTextUIProperty = serializedObject.FindProperty("mTextUI");
        mTextIdProperty = serializedObject.FindProperty("mTextId");
        mTextBreakOffProperty = serializedObject.FindProperty("mBreakOff");
        mTextCharBreakOffProperty = serializedObject.FindProperty("mCharBreakOff");
        mTextWordBreakOffProperty = serializedObject.FindProperty("mWordBreakOff");
        mTextParamsProperty = serializedObject.FindProperty("mParams");
    }

    public override void OnInspectorGUI()
    {
        var tbox = (LanguageTextBox)serializedObject.targetObject;
        serializedObject.Update();
        var textbox = tbox.GetComponent<UnityEngine.UI.Text>();
        if (mTextUIProperty.objectReferenceValue == null)
        {
            mTextUIProperty.objectReferenceValue = textbox;
        }
        EditorGUILayout.PropertyField(mTextBreakOffProperty, new GUIContent("BreakOff"));
        if (tbox.BreakOff)
        {
            EditorGUILayout.PropertyField(mTextCharBreakOffProperty, new GUIContent("Char"));
            EditorGUILayout.PropertyField(mTextWordBreakOffProperty, new GUIContent("Word"));
        }

        EditorGUILayout.PropertyField(mTextIdProperty, new GUIContent("TextId:"));
        EditorGUILayout.PropertyField(mTextParamsProperty, new GUIContent("Params:"),true);
        string[] param = new string[mTextParamsProperty.arraySize];

        for (int i = 0; i < mTextParamsProperty.arraySize; i++)
        {
            param[i] = mTextParamsProperty.GetArrayElementAtIndex(i).stringValue;
        }
        string text = LangUtil.GetLanguageText(mTextIdProperty.intValue, param);
       
        text = text.Replace("\\n", "\n");
        GUILayout.TextArea(text);
        if (textbox != null)
        {
            textbox.text = text;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
