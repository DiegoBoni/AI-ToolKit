using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

[CreateAssetMenu(fileName = "Brain_OpenAITextToSpeech", menuName = "OpenAI/Text To Speech SO", order = 1)]
public class OpenAITextToSpeech : BrainSO
{
    [Header("---- Text-to-Speech Settings ----")]
    [SerializeField] public TTSModel _model = TTSModel.TTS_1;
    [SerializeField] public TTSVoice _voice = TTSVoice.Alloy;
    [Tooltip("The speed of the generated audio. Select a value from 0.25 to 4.0. 1.0 is the default.")]
    [Range(0.25f, 4.0f)]
    [SerializeField] public float _speed = 1.0f;
    [SerializeField] public TTSResponseFormat _responseFormat = TTSResponseFormat.Mp3;
    [Tooltip("Control the voice of your generated audio with additional instructions. Does not work with tts-1 or tts-1-hd.")]
    [TextArea(3, 30)]
    [SerializeField] public string _instructions = "";
    [Space]

    private const string OpenAIUrlAPI = "https://api.openai.com/v1/audio/speech";

    public override void SendMessage(string message, Action<string> callback)
    {
        Debug.LogError("OpenAITextToSpeech.SendMessage is not intended for audio generation. Use GenerateSpeech instead.");
        callback?.Invoke("Error: Use GenerateSpeech for audio output.");
    }

    public void GenerateSpeech(string text, Action<AudioClip> callback)
    {
        MyCoroutineManager.Instance.StartCoroutine(SendRequestAPI(text, callback));
    }

    private IEnumerator SendRequestAPI(string text, Action<AudioClip> callback)
    {
        RequestBodyTTS requestBody = new RequestBodyTTS
        {
            model = GetModelString(_model),
            input = text,
            voice = GetVoiceString(_voice),
            speed = _speed,
            response_format = GetResponseFormatString(_responseFormat)
        };

        if (_model != TTSModel.TTS_1 && _model != TTSModel.TTS_1_HD)
        {
            requestBody.instructions = _instructions;
        }

        string jsonBody = JsonUtility.ToJson(requestBody);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest request = new UnityWebRequest(OpenAIUrlAPI, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerAudioClip(request.url, AudioType.MPEG);
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + _apiKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"TTS Error: {request.error}");
                callback(null);
            }
            else
            {
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(request);
                callback(audioClip);
            }
        }
    }

    [Serializable]
    public class RequestBodyTTS
    {
        public string model;
        public string input;
        public string voice;
        public float speed;
        public string response_format;
        public string instructions;
    }

    // --- Enum to String Converters ---

    private string GetModelString(TTSModel model) => model switch
    {
        TTSModel.TTS_1 => "tts-1",
        TTSModel.TTS_1_HD => "tts-1-hd",
        TTSModel.GPT_4o_mini_TTS => "gpt-4o-mini-tts",
        _ => "tts-1"
    };

    private string GetVoiceString(TTSVoice voice) => voice.ToString().ToLower();

    private string GetResponseFormatString(TTSResponseFormat format) => format.ToString().ToLower();
}

// --- Enums for TTS Settings ---

public enum TTSModel
{
    TTS_1,
    TTS_1_HD,
    GPT_4o_mini_TTS
}

public enum TTSVoice
{
    Alloy,
    Ash,
    Coral,
    Echo,
    Fable,
    Onyx,
    Nova,
    Shimmer
}

public enum TTSResponseFormat
{
    Mp3,
    Opus,
    Aac,
    Flac,
    Wav,
    Pcm
}