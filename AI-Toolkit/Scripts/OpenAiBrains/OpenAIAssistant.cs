using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu(fileName = "Brain_OpenAIAssistant", menuName = "OpenAI/Assistant SO", order = 2)]
public class OpenAIAssistant : BrainSO
{
    [Header("---- OpenAI Assistant Settings ----")]
    [SerializeField] protected string _assistantId;

    private const string OpenAIBaseURL = "https://api.openai.com/v1";

    public override void SendMessage(string message, Action<string> callback)
    {
        MyCoroutineManager.Instance.StartCoroutine(ConversationFlow(message, callback));
    }

    private IEnumerator ConversationFlow(string userInput, Action<string> callback)
    {
        // Step 1: Create a Thread
        string threadId = null;
        yield return MyCoroutineManager.Instance.StartCoroutine(CreateThread((id) => threadId = id, callback));

        if (string.IsNullOrEmpty(threadId))
        {
            yield break; // Stop if thread creation failed
        }

        // Step 2: Add a Message to the Thread
        yield return MyCoroutineManager.Instance.StartCoroutine(AddMessageToThread(threadId, userInput, callback));

        // Step 3: Run the Assistant with Streaming
        yield return MyCoroutineManager.Instance.StartCoroutine(RunAssistantStream(threadId, callback));
    }

    private IEnumerator CreateThread(Action<string> onThreadCreated, Action<string> onError)
    {
        using (UnityWebRequest request = new UnityWebRequest($"{OpenAIBaseURL}/threads", "POST"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            SetCommonHeaders(request);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<ThreadResponse>(request.downloadHandler.text);
                onThreadCreated(response.id);
            }
            else
            {
                onError($"Error creating thread: {request.error} - {request.downloadHandler.text}");
                onThreadCreated(null);
            }
        }
    }

    private IEnumerator AddMessageToThread(string threadId, string message, Action<string> onError)
    {
        var messageData = new CreateMessageRequest { role = "user", content = message };
        string jsonData = JsonUtility.ToJson(messageData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest($"{OpenAIBaseURL}/threads/{threadId}/messages", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            SetCommonHeaders(request);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError($"Error adding message: {request.error} - {request.downloadHandler.text}");
            }
        }
    }

    private IEnumerator RunAssistantStream(string threadId, Action<string> callback)
    {
        var runData = new CreateRunRequest { assistant_id = _assistantId, stream = true };
        string jsonData = JsonUtility.ToJson(runData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest($"{OpenAIBaseURL}/threads/{threadId}/runs", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            var streamingHandler = new StreamingDownloadHandler(callback);
            request.downloadHandler = streamingHandler;
            SetCommonHeaders(request);

            request.SendWebRequest();

            while (!streamingHandler.isDone)
            {
                yield return null;
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                callback($"Error in streaming run: {request.error} - {request.downloadHandler.text}");
            }
        }
    }

    private void SetCommonHeaders(UnityWebRequest request)
    {
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + _apiKey);
        request.SetRequestHeader("OpenAI-Beta", "assistants=v2");
    }
}

public class StreamingDownloadHandler : DownloadHandlerScript
{
    public bool isDone = false;
    private Action<string> _onChunkReceived;

    public StreamingDownloadHandler(Action<string> onChunkReceived)
    {
        _onChunkReceived = onChunkReceived;
    }

    protected override bool ReceiveData(byte[] data, int dataLength)
    {
        if (data == null || dataLength == 0)
        {
            return false;
        }

        string chunk = Encoding.UTF8.GetString(data, 0, dataLength);
        string[] lines = chunk.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            if (line.StartsWith("data:"))
            {
                string jsonData = line.Substring(5).Trim();
                if (jsonData == "[DONE]")
                {
                    isDone = true;
                    return false; // End of stream
                }

                try
                {                   
                    var eventData = JsonUtility.FromJson<MessageDeltaEventData>(jsonData);
                    if (eventData != null && eventData.delta != null && eventData.delta.content != null)
                    {                       
                        foreach (var content in eventData.delta.content)
                        {
                            if (content.text != null && !string.IsNullOrEmpty(content.text.value))
                            {
                                _onChunkReceived(content.text.value);
                            }
                        }
                    }
                }
                catch (Exception e)
                {                   
                    Debug.LogWarning($"Failed to parse stream data: {jsonData} - Error: {e.Message}");
                }
            }
        }
        return true;
    }
}

// --- Data Classes for JSON Deserialization ---

[Serializable]
public class CreateMessageRequest
{
    public string role;
    public string content;
}

[Serializable]
public class CreateRunRequest
{
    public string assistant_id;
    public bool stream = true;
}

[Serializable]
public class ThreadResponse
{
    public string id;
}

[Serializable]
public class MessageDeltaEventData
{
    public string id;
    public Delta delta;
}

[Serializable]
public class Delta
{
    public System.Collections.Generic.List<ContentData> content;
}

[Serializable]
public class ContentData
{
    public int index;
    public string type;
    public TextData text;
}

[Serializable]
public class TextData
{
    public string value;
}

