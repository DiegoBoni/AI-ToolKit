using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OpenAIImageGeneratorSO))]
public class OpenAIImageGeneratorSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        OpenAIImageGeneratorSO brain = (OpenAIImageGeneratorSO)target;

        EditorGUILayout.Space();
        
        // Display pricing info for the selected model
        string priceInfo = brain.GetModelPriceInfo(brain._model);
        if (!string.IsNullOrEmpty(priceInfo))
        {
            GUI.enabled = false;
            EditorGUILayout.TextArea(priceInfo);
            GUI.enabled = true;
        }

        // Add a note about DALL-E 2 and 3 pricing
        if (brain._model == ImageModel.DALL_E_2 || brain._model == ImageModel.DALL_E_3)
        {
            EditorGUILayout.HelpBox("DALL-E 2/3 pricing depends on the selected image size and quality. Please refer to the official OpenAI documentation for detailed pricing.", MessageType.Info);
        }
    }
}
