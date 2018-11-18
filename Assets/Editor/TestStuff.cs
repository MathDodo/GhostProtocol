using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SaveManager))]
public class TestStuff : Editor
{
    private SaveManager castedTarget;

    private void OnEnable()
    {
        castedTarget = (SaveManager)target;
    }

    public override void OnInspectorGUI()
    {
        castedTarget.TestStuff = EditorGUILayout.Toggle("TestStuff", castedTarget.TestStuff);
        castedTarget.IntTestStuff = EditorGUILayout.IntField(castedTarget.IntTestStuff);
        castedTarget.StringStuff = EditorGUILayout.TextField(castedTarget.StringStuff);
        castedTarget.FloatTestStuff = EditorGUILayout.FloatField(castedTarget.FloatTestStuff);

        var prop = serializedObject.FindProperty("SomeMoreStuff");
        EditorGUILayout.PropertyField(prop, true);
        serializedObject.ApplyModifiedProperties();

        EditorUtility.SetDirty(castedTarget);
    }
}