using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

[CreateAssetMenu(fileName = "Brain_OpenAIChatCompletion", menuName = "OpenAI/Chat Completion SO", order = 1)]
public class OpenAIChatCompletion : BrainSO
{
    [Header("---- Chat Completion Settings ----")]
    [SerializeField] public ChatModel _model = ChatModel.GPT_4o_mini;
    [SerializeField] protected int _maxTokens = 2048;
    [Range(0.0f, 2.0f)]
    [Tooltip("0.0: Deterministic - 2.0: Very Creative")]
    [SerializeField] protected float _temperature = 1.0f;
    [Space]

    [Header("---- Personality Setting ----")]
    [TextArea(3, 30)]
    [SerializeField] protected string _systemPrompt = "You are a helpful assistant.";

    private const string OpenAIUrlAPI = "https://api.openai.com/v1/chat/completions";

    public override void SendMessage(string message, Action<string> callback)
    {
        MyCoroutineManager.Instance.StartCoroutine(SendRequestAPI(message, callback));
    }

    private IEnumerator SendRequestAPI(string userMessage, Action<string> callback)
    {
        List<Message> messages = new List<Message>();
        if (!string.IsNullOrEmpty(_systemPrompt))
        {
            messages.Add(new Message { role = "system", content = _systemPrompt });
        }
        messages.Add(new Message { role = "user", content = userMessage });

        RequestBodyOpenAI requestBody = new RequestBodyOpenAI
        {
            model = GetModelString(_model),
            messages = messages,
            temperature = _temperature,
            max_tokens = _maxTokens
        };

        string jsonBody = JsonUtility.ToJson(requestBody);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest request = new UnityWebRequest(OpenAIUrlAPI, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + _apiKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<ResponseBodyOpenAI>(request.downloadHandler.text);
                string result = response.choices[0].message.content;
                callback(result);
            }
            else
            {
                Debug.LogError($"Chat Completion Error: {request.error} - {request.downloadHandler.text}");
                callback("Error: " + request.error);
            }
        }
    }

    private string GetModelString(ChatModel model)
    {
        switch (model)
        {
            case ChatModel.GPT_3_5_Turbo:
                return "gpt-3.5-turbo";
            case ChatModel.GPT_4:
                return "gpt-4";
            case ChatModel.GPT_4_Turbo:
                return "gpt-4-turbo";
            case ChatModel.GPT_4_1_nano:
                return "gpt-4.1-nano";
            case ChatModel.GPT_4_1_mini:
                return "gpt-4.1-mini";
            case ChatModel.GPT_4_1:
                return "gpt-4-1";
            case ChatModel.GPT_4o_mini:
                return "gpt-4o-mini";
            case ChatModel.GPT_4o:
                return "gpt-4o";
            default:
                return "gpt-3.5-turbo"; // Fallback to a default model
        }
        // return model.ToString().ToLower().Replace("gpt_", "gpt-").Replace("_", "-");
    }

    public string GetModelPriceInfo(ChatModel model)
    {
        switch (model)
        {
            case ChatModel.GPT_3_5_Turbo:
                return "Input: $0.50 / 1M tokens | Output: $1.50 / 1M tokens";
            case ChatModel.GPT_4:
                return "Input: $30.00 / 1M tokens | Output: $60.00 / 1M tokens";
            case ChatModel.GPT_4_Turbo:
                return "Input: $10.00 / 1M tokens | Output: $30.00 / 1M tokens";
            case ChatModel.GPT_4_1_nano:
                return "Input: $0.10 / 1M tokens | Output: $0.40 / 1M tokens";
            case ChatModel.GPT_4_1_mini:
                return "Input: $0.40 / 1M tokens | Output: $1.60 / 1M tokens";
            case ChatModel.GPT_4_1:
                return "Input: $2.00 / 1M tokens | Output: $8.00 / 1M tokens";
            case ChatModel.GPT_4o_mini:
                return "Input: $0.15 / 1M tokens | Output: $0.60 / 1M tokens";
            case ChatModel.GPT_4o:
                return "Input: $2.50 / 1M tokens | Output: $10.00 / 1M tokens";
            default:
                return "Price not available";
        }
    }
}

// --- Enums ---

public enum ChatModel
{
    // Oldest / Smallest
    GPT_3_5_Turbo,
    GPT_4,
    GPT_4_Turbo,
    GPT_4_1_nano,
    GPT_4_1_mini,
    GPT_4_1,
    GPT_4o_mini,
    GPT_4o
    // Newest / Biggest
}

// --- API Data Classes ---

[Serializable]
public class Message
{
    public string role;
    public string content;
}

[Serializable]
public class RequestBodyOpenAI
{
    public string model;
    public List<Message> messages;
    public float temperature;
    public int max_tokens;
}

[Serializable]
public class ResponseBodyOpenAI
{
    public string id;
    public string @object;
    public int created;
    public string model;
    public List<Choice> choices;
    public Usage usage;

    [Serializable]
    public class Choice
    {
        public int index;
        public Message message;
        public string finish_reason;
    }

    [Serializable]
    public class Usage
    {
        public int prompt_tokens;
        public int completion_tokens;
        public int total_tokens;
    }
}