using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

[CreateAssetMenu(fileName = "Brain_OpenAIVision", menuName = "OpenAI/Vision SO", order = 5)]
public class OpenAIVisionSO : BrainSO
{
    [Header("---- Vision Model Settings ----")]
    [SerializeField] private VisionModel _model = VisionModel.GPT_4o;
    [SerializeField] private int _maxTokens = 1024;

    private const string OpenAIUrlAPI = "https://api.openai.com/v1/chat/completions";

    // --- Nested classes for the specific Vision API request structure ---

    [Serializable]
    private class VisionRequestBody
    {
        public string model;
        public List<Message> messages;
        public int max_tokens;
    }

    [Serializable]
    private class Message
    {
        public string role;
        public List<ContentPart> content;
    }

    [Serializable]
    private abstract class ContentPart
    {
        public string type;
    }

    [Serializable]
    private class TextContent : ContentPart
    {
        public string text;
        public TextContent(string text) { this.type = "text"; this.text = text; }
    }

    [Serializable]
    private class ImageUrlContent : ContentPart
    {
        public ImageUrl image_url;
        public ImageUrlContent(ImageUrl imageUrl) { this.type = "image_url"; this.image_url = imageUrl; }
    }

    [Serializable]
    private class ImageUrl
    {
        public string url;
    }

    // --- Response classes (simplified for vision) ---
    [Serializable]
    private class ResponseBody
    {
        public List<Choice> choices;
    }

    [Serializable]
    private class Choice
    {
        public ResponseMessage message;
    }

    [Serializable]
    private class ResponseMessage
    {
        public string content;
    }

    // This base method is not suitable for vision tasks.
    public override void SendMessage(string message, Action<string> callback)
    {
        Debug.LogError("SendMessage(string,...) is not applicable for OpenAIVisionSO. Use AnalyzeImage(Texture2D, string, ...).");
        callback?.Invoke(null);
    }

    public void AnalyzeImage(Texture2D image, string prompt, Action<string> callback)
    {
        if (image == null)
        {
            Debug.LogError("Image cannot be null.");
            callback?.Invoke(null);
            return;
        }
        MyCoroutineManager.Instance.StartCoroutine(SendRequestAPI(image, prompt, callback));
    }

    private IEnumerator SendRequestAPI(Texture2D image, string prompt, Action<string> callback)
    {
        // 1. Encode the image to JPG and then to Base64
        byte[] imageData = image.EncodeToJPG();
        string base64Image = Convert.ToBase64String(imageData);
        string imageUrlData = $"data:image/jpeg;base64,{base64Image}";

        // 2. Construct the request body
        var requestBody = new VisionRequestBody
        {
            model = GetModelString(_model),
            max_tokens = _maxTokens,
            messages = new List<Message>
            {
                new Message
                {
                    role = "user",
                    content = new List<ContentPart>
                    {
                        new TextContent(prompt),
                        new ImageUrlContent(new ImageUrl { url = imageUrlData })
                    }
                }
            }
        };

        // We need a custom serializer because JsonUtility doesn't support abstract classes.
        string jsonBody = SerializeRequest(requestBody);
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
                var response = JsonUtility.FromJson<ResponseBody>(request.downloadHandler.text);
                callback?.Invoke(response.choices[0].message.content);
            }
            else
            {
                Debug.LogError($"Vision API Error: {request.error} - {request.downloadHandler.text}");
                callback?.Invoke(null);
            }
        }
    }

    private string GetModelString(VisionModel model) => model switch
    {
        VisionModel.GPT_4o => "gpt-4o",
        VisionModel.GPT_4_Turbo => "gpt-4-turbo",
        _ => "gpt-4o"
    };

    // Custom serializer to handle the polymorphic 'content' array.
    private string SerializeRequest(VisionRequestBody requestBody)
    {
        var sb = new StringBuilder();
        sb.Append("{");
        sb.Append($"\"model\": \"{requestBody.model}\",");
        sb.Append($"\"max_tokens\": {requestBody.max_tokens},");
        sb.Append("\"messages\": [");
        for (int i = 0; i < requestBody.messages.Count; i++)
        {
            var msg = requestBody.messages[i];
            sb.Append("{");
            sb.Append($"\"role\": \"{msg.role}\",");
            sb.Append("\"content\": [");
            for (int j = 0; j < msg.content.Count; j++)
            {
                var part = msg.content[j];
                if (part is TextContent textPart)
                {
                    sb.Append($"{{\"type\": \"text\", \"text\": \"{textPart.text}\"}}");
                }
                else if (part is ImageUrlContent imagePart)
                {
                    sb.Append($"{{\"type\": \"image_url\", \"image_url\": {{\"url\": \"{imagePart.image_url.url}\"}}}}");
                }
                if (j < msg.content.Count - 1) sb.Append(",");
            }
            sb.Append("]}");
            if (i < requestBody.messages.Count - 1) sb.Append(",");
        }
        sb.Append("]}");
        return sb.ToString();
    }
}

public enum VisionModel
{
    GPT_4o,
    GPT_4_Turbo
}
