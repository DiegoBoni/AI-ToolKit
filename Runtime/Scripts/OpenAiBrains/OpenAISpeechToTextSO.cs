using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu(fileName = "Brain_OpenAISpeechToText", menuName = "OpenAI/Speech To Text SO", order = 4)]
public class OpenAISpeechToTextSO : BrainSO
{
    [Header("---- Speech-to-Text Settings ----")]
    [SerializeField] private TranscriptionModel _model = TranscriptionModel.Whisper_1;

    private const string OpenAIAudioAPIURL = "https://api.openai.com/v1/audio/transcriptions";

    // This method is not suitable for this brain, as it requires an AudioClip.
    public override void SendMessage(string message, Action<string> callback)
    {
        Debug.LogError("SendMessage(string, ...) is not applicable for OpenAISpeechToTextSO. Use Transcribe(AudioClip, ...).");
        callback?.Invoke(null);
    }

    public void Transcribe(AudioClip clip, Action<string> callback)
    {
        if (clip == null)
        {
            Debug.LogError("AudioClip is null. Cannot transcribe.");
            callback?.Invoke(null);
            return;
        }
        MyCoroutineManager.Instance.StartCoroutine(SendRequestAPI(clip, callback));
    }

    private IEnumerator SendRequestAPI(AudioClip clip, Action<string> callback)
    {
        // Convert AudioClip to WAV byte array
        byte[] wavData = ConvertAudioClipToWav(clip);
        string fileName = $"audio_{DateTime.Now.Ticks}.wav";

        // Create a form to send the data
        WWWForm form = new WWWForm();
        form.AddField("model", GetModelString(_model));
        form.AddBinaryData("file", wavData, fileName, "audio/wav");

        using (UnityWebRequest request = UnityWebRequest.Post(OpenAIAudioAPIURL, form))
        {
            request.SetRequestHeader("Authorization", "Bearer " + _apiKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<TranscriptionResponse>(request.downloadHandler.text);
                callback?.Invoke(response.text);
            }
            else
            {
                Debug.LogError($"Speech-to-Text Error: {request.error} - {request.downloadHandler.text}");
                callback?.Invoke(null);
            }
        }
    }

    private string GetModelString(TranscriptionModel model) => model switch
    {
        TranscriptionModel.GPT_4o_transcribe => "gpt-4o-transcribe",
        TranscriptionModel.GPT_4o_mini_transcribe => "gpt-4o-mini-transcribe",
        TranscriptionModel.Whisper_1 => "whisper-1",
        _ => "whisper-1"
    };

    // --- WAV Conversion Utility ---
    // Based on: https://gist.github.com/darktable/2317063
    private byte[] ConvertAudioClipToWav(AudioClip clip)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                // WAV header
                writer.Write(Encoding.ASCII.GetBytes("RIFF"));
                writer.Write(0); // Placeholder for file size
                writer.Write(Encoding.ASCII.GetBytes("WAVE"));
                writer.Write(Encoding.ASCII.GetBytes("fmt "));
                writer.Write(16); // PCM chunk size
                writer.Write((ushort)1); // Audio format (1 for PCM)
                ushort numChannels = (ushort)clip.channels;
                writer.Write(numChannels);
                uint sampleRate = (uint)clip.frequency;
                writer.Write(sampleRate);
                writer.Write(sampleRate * numChannels * 2); // Byte rate
                writer.Write((ushort)(numChannels * 2)); // Block align
                writer.Write((ushort)16); // Bits per sample

                writer.Write(Encoding.ASCII.GetBytes("data"));
                writer.Write(0); // Placeholder for data size

                // Audio data
                float[] samples = new float[clip.samples * clip.channels];
                clip.GetData(samples, 0);

                for (int i = 0; i < samples.Length; i++)
                {
                    short intSample = (short)(samples[i] * short.MaxValue);
                    writer.Write(intSample);
                }

                // Fill in placeholders
                long fileSize = memoryStream.Length;
                writer.Seek(4, SeekOrigin.Begin);
                writer.Write((int)(fileSize - 8));
                writer.Seek(40, SeekOrigin.Begin);
                writer.Write((int)(fileSize - 44));
            }
            return memoryStream.ToArray();
        }
    }
}

public enum TranscriptionModel
{
    GPT_4o_transcribe,
    GPT_4o_mini_transcribe,
    Whisper_1
}

[Serializable]
public class TranscriptionResponse
{
    public string text;
}
