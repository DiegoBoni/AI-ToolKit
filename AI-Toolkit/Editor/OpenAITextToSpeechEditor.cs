using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OpenAITextToSpeech))]
public class OpenAITextToSpeechEditor : Editor
{
    SerializedProperty _model;
    SerializedProperty _voice;
    SerializedProperty _speed;
    SerializedProperty _responseFormat;
    SerializedProperty _instructions;
    SerializedProperty _apiKey;

    private void OnEnable()
    {
        _apiKey = serializedObject.FindProperty("_apiKey");
        _model = serializedObject.FindProperty("_model");
        _voice = serializedObject.FindProperty("_voice");
        _speed = serializedObject.FindProperty("_speed");
        _responseFormat = serializedObject.FindProperty("_responseFormat");
        _instructions = serializedObject.FindProperty("_instructions");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_apiKey);
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(_model);
        EditorGUILayout.PropertyField(_voice);
        EditorGUILayout.PropertyField(_speed);
        EditorGUILayout.PropertyField(_responseFormat);

        TTSModel currentModel = (TTSModel)_model.enumValueIndex;

        if (currentModel != TTSModel.TTS_1 && currentModel != TTSModel.TTS_1_HD)
        {
            EditorGUILayout.PropertyField(_instructions);
        }

        serializedObject.ApplyModifiedProperties();
    }
}