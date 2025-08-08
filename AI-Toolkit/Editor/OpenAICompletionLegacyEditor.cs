using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OpenAICompletionLegacy))]
public class OpenAICompletionLegacyEditor : Editor
{
    SerializedProperty _completionModel;
    SerializedProperty _maxTokens;
    SerializedProperty _temperature;
    SerializedProperty _stream;
    SerializedProperty _systemPrompt;
    SerializedProperty _apiKey;

    private void OnEnable()
    {
        _apiKey = serializedObject.FindProperty("_apiKey");
        _completionModel = serializedObject.FindProperty("_completionModel");
        _maxTokens = serializedObject.FindProperty("_maxTokens");
        _temperature = serializedObject.FindProperty("_temperature");
        
        _systemPrompt = serializedObject.FindProperty("_systemPrompt");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // EditorGUILayout.LabelField("---- General Settings ----", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_apiKey);
        EditorGUILayout.Space();

        // EditorGUILayout.LabelField("---- Completion Settings ----", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_completionModel);
        EditorGUILayout.PropertyField(_maxTokens);
        EditorGUILayout.PropertyField(_temperature);
        
        EditorGUILayout.Space();

        // EditorGUILayout.LabelField("---- Prompt ----", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_systemPrompt);
        EditorGUILayout.Space();

        OpenAICompletionLegacy brain = (OpenAICompletionLegacy)target;
        EditorGUILayout.HelpBox(brain.GetModelPriceInfo((CompletionModel)_completionModel.enumValueIndex), MessageType.Info);

        serializedObject.ApplyModifiedProperties();
    }
}