using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

[CreateAssetMenu(fileName = "Brain_OpenAICompletionLegacy", menuName = "OpenAI/Completion Legacy SO", order = 2)]
public class OpenAICompletionLegacy : BrainSO
{
    [Header("---- Completion Settings ----")]
    [SerializeField] protected CompletionModel _completionModel = CompletionModel.Gpt_3_5_Turbo_Instruct;
    [SerializeField] protected int _maxTokens = 2048;
    [Range(0.0f, 1.0f)]
    [Tooltip("0.0: Deterministic - 1.0: Creative")]
    [SerializeField] protected float _temperature = 0.5f;
    [Space]

    [Header("---- Prompt ----")]
    [TextArea(3, 30)]
    [SerializeField] protected string _systemPrompt = "You are a helpful assistant.";

    private const string OpenAIUrlAPI = "https://api.openai.com/v1/completions";

    // --- Nested classes for API request/response ---

    [Serializable]
    private class RequestBody
    {
        public string model;
        public string prompt;
        public int max_tokens;
        public float temperature;
        
    }

    [Serializable]
    private class ResponseBody
    {
        public string id;
        public List<Choice> choices;
        public Usage usage;

        [Serializable]
        public class Choice
        {
            public string text;
        }

        [Serializable]
        public class Usage
        {
            public int prompt_tokens;
            public int completion_tokens;
            public int total_tokens;
        }
    }

    public override void SendMessage(string message, Action<string> callback)
    {
        string prompt = _systemPrompt + "\n\n" + message;

        RequestBody requestBody = new RequestBody
        {
            model = GetModelString(_completionModel),
            prompt = prompt,
            max_tokens = _maxTokens,
            temperature = _temperature,
            
        };

        MyCoroutineManager.Instance.StartCoroutine(SendRequestAPI(requestBody, callback));
    }

    private IEnumerator SendRequestAPI(RequestBody requestBody, Action<string> callback)
    {
        string jsonData = JsonUtility.ToJson(requestBody);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(OpenAIUrlAPI, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(data);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + _apiKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var responseBody = JsonUtility.FromJson<ResponseBody>(request.downloadHandler.text);
                string result = responseBody.choices[0].text.Trim();
                callback(result);

                if (responseBody.usage != null)
                {
                    Debug.Log($"Tokens Used: Prompt: {responseBody.usage.prompt_tokens}, Completion: {responseBody.usage.completion_tokens}, Total: {responseBody.usage.total_tokens}");
                }
            }
            else
            {
                Debug.LogError($"Completion Legacy Error: {request.error} - {request.downloadHandler.text}");
                callback("Error: " + request.error);
            }
        }
    }

    private string GetModelString(CompletionModel model) => model switch
    {
        CompletionModel.Gpt_3_5_Turbo_Instruct_0914 => "gpt-3.5-turbo-instruct-0914",
        CompletionModel.Gpt_3_5_Turbo_Instruct => "gpt-3.5-turbo-instruct",
        _ => "gpt-3.5-turbo-instruct"
    };

    public string GetModelPriceInfo(CompletionModel model)
    {
        switch (model)
        {
            case CompletionModel.Gpt_3_5_Turbo_Instruct_0914:
            case CompletionModel.Gpt_3_5_Turbo_Instruct:
                return "Input: $1.50 / 1M tokens | Output: $2.00 / 1M tokens";
            default:
                return "Price not available";
        }
    }
}

public enum CompletionModel
{
    Gpt_3_5_Turbo_Instruct_0914,
    Gpt_3_5_Turbo_Instruct,
}
