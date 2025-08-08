using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

[CreateAssetMenu(fileName = "Brain_OpenAIImageGenerator", menuName = "OpenAI/Image Generator SO", order = 3)]
public class OpenAIImageGeneratorSO : BrainSO
{
    [Header("---- Image Generation Settings ----")]
    [SerializeField] public ImageModel _model = ImageModel.DALL_E_3;
    [SerializeField] private ImageSize _size = ImageSize.S1024x1024;
    [SerializeField] private ImageQuality _quality = ImageQuality.Standard;
    [SerializeField] private ImageStyle _style = ImageStyle.Vivid;

    public event Action<Sprite> OnSpriteGenerated;

    private const string OpenAIImageAPIURL = "https://api.openai.com/v1/images/generations";

    public override void SendMessage(string prompt, Action<string> callback)
    {
        MyCoroutineManager.Instance.StartCoroutine(GenerateImage(prompt, callback));
    }

    private IEnumerator GenerateImage(string prompt, Action<string> callback)
    {
        ImageGenerationRequest requestBody = new ImageGenerationRequest
        {
            prompt = prompt,
            model = GetModelString(_model),
            n = 1,
            quality = GetImageQualityString(_quality),
            response_format = "url",
            size = GetImageSizeString(_size),
            style = GetImageStyleString(_style)
        };

        string jsonData = JsonUtility.ToJson(requestBody);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(OpenAIImageAPIURL, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + _apiKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                ImageGenerationResponse response = JsonUtility.FromJson<ImageGenerationResponse>(request.downloadHandler.text);
                if (response.data != null && response.data.Length > 0)
                {
                    string imageUrl = response.data[0].url;
                    yield return MyCoroutineManager.Instance.StartCoroutine(DownloadAndProcessImage(imageUrl, callback));
                }
                else
                {
                    callback?.Invoke("Error: No image URL received.");
                }
            }
            else
            {
                callback?.Invoke($"Error generating image: {request.error} - {request.downloadHandler.text}");
            }
        }
    }

    private IEnumerator DownloadAndProcessImage(string imageUrl, Action<string> callback)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                OnSpriteGenerated?.Invoke(sprite);

                byte[] bytes = texture.EncodeToPNG();
                string folderPath = "Assets/AI-Toolkit/Resources/GeneratedImages";
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                string fileName = $"GeneratedImage_{DateTime.Now.ToString("yyyyMMddHHmmss")}.png";
                string filePath = Path.Combine(folderPath, fileName);

                File.WriteAllBytes(filePath, bytes);
                callback?.Invoke(filePath);
            }
            else
            {
                callback?.Invoke($"Error downloading image: {request.error}");
            }
        }
    }

    // --- Helper methods ---
    public string GetModelString(ImageModel model) => model switch
    {
        ImageModel.DALL_E_2 => "dall-e-2",
        ImageModel.DALL_E_3 => "dall-e-3",
        ImageModel.GPT_Image_1 => "gpt-image-1",
        _ => "dall-e-3"
    };

    public string GetImageSizeString(ImageSize size) => size.ToString().Replace("S", "").ToLower();

    public string GetImageQualityString(ImageQuality quality) => quality.ToString().ToLower();

    public string GetImageStyleString(ImageStyle style) => style.ToString().ToLower();

    public string GetModelPriceInfo(ImageModel model)
    {
        return model switch
        {
            ImageModel.GPT_Image_1 => "Input: $10.00 / 1M tokens | Output: $40.00 / 1M tokens",
            _ => "Pricing varies by size and quality. See OpenAI docs."
        };
    }
}

// --- Enums for Image Generation Settings ---
public enum ImageModel
{
    DALL_E_3,
    DALL_E_2,
    GPT_Image_1
}

public enum ImageSize
{
    S1024x1024,
    S1792x1024, // DALL-E 3 only
    S1024x1792, // DALL-E 3 only
    S512x512,   // DALL-E 2 only
    S256x256    // DALL-E 2 only
}

public enum ImageQuality
{
    Standard,
    HD
}

public enum ImageStyle
{
    Vivid,
    Natural
}

// --- Data Classes for JSON Serialization/Deserialization ---

[Serializable]
public class ImageGenerationRequest
{
    public string prompt;
    public string model;
    public int n;
    public string quality;
    public string response_format;
    public string size;
    public string style;
}

[Serializable]
public class ImageGenerationResponse
{
    public long created;
    public ImageData[] data;
}

[Serializable]
public class ImageData
{
    public string url;
    public string b64_json;
    public string revised_prompt;
}