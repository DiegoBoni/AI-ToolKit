using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OpenAIChatCompletion))]
public class OpenAIChatCompletionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        OpenAIChatCompletion brain = (OpenAIChatCompletion)target;

        EditorGUILayout.Space();
        GUI.enabled = false;
        EditorGUILayout.TextArea(brain.GetModelPriceInfo(brain._model));
        GUI.enabled = true;
    }
}
