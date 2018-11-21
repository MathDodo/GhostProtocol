using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SaveManager))]
public class TestEditor : Editor
{
    private MonoScript monoScript;
    private SaveManager castedTarget;

    private void OnEnable()
    {
        castedTarget = (SaveManager)target;
        monoScript = MonoScript.FromScriptableObject(castedTarget);
    }

    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        //GUI.enabled = false;
        //EditorGUILayout.ObjectField("Script", monoScript, typeof(MonoScript), false);
        //GUI.enabled = true;

        //SerializedProperty prop = serializedObject.FindProperty("_save");

        //prop.boolValue = EditorGUILayout.Toggle(prop.boolValue);

        //serializedObject.ApplyModifiedProperties();

        //EditorUtility.SetDirty(castedTarget);
    }
}