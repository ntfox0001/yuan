using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Subtitles), true)]
public class SubtitlesInspector : Editor
{
    SerializedProperty mTraceProperty;
    SerializedProperty mShowTimeProperty;
    SerializedProperty mTextTweenAlplaProperty;
    private void OnEnable()
    {
        mTraceProperty = serializedObject.FindProperty("Trace");
        mShowTimeProperty = serializedObject.FindProperty("ShowTime");

    }
    public override void OnInspectorGUI()
    {
        var subtitles = (Subtitles)serializedObject.targetObject;
        serializedObject.Update();

        EditorGUILayout.PropertyField(mShowTimeProperty);


        EditorGUILayout.Space();
        GUILayout.Label("Trace:");

        int insertButton = -1;
        EditorGUI.indentLevel++;
        for (int i = 0; i < mTraceProperty.arraySize;)
        {
            EditorGUI.indentLevel++;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Id" + i + ":", GUILayout.Width(40));

            bool delbutton = GUILayout.Button("-", GUILayout.Width(30));
            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                insertButton = i;
            }

            GUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(mTraceProperty.GetArrayElementAtIndex(i).FindPropertyRelative("Time"));
            EditorGUILayout.PropertyField(mTraceProperty.GetArrayElementAtIndex(i).FindPropertyRelative("TextId"));
            EditorGUILayout.PropertyField(mTraceProperty.GetArrayElementAtIndex(i).FindPropertyRelative("Pos"));

            //subtitles.Trace[i].Time = EditorGUILayout.FloatField("time", subtitles.Trace[i].Time);
            //subtitles.Trace[i].TextId = EditorGUILayout.IntField("textId", subtitles.Trace[i].TextId);


            string text = LangUtil.GetLanguageText(subtitles.Trace[i].TextId);
            GUILayout.TextArea(text);

            EditorGUI.indentLevel--;

            

            if (delbutton)
            {
                mTraceProperty.DeleteArrayElementAtIndex(i);
            }
            else
            {
                i++;
            }
            
            EditorGUILayout.Space();
        }
        EditorGUI.indentLevel--;
        if (insertButton != -1)
        {
            mTraceProperty.InsertArrayElementAtIndex(insertButton);
        }


        serializedObject.ApplyModifiedProperties();
    }
}
