using UnityEngine;
using UnityEngine.UI;

public class ImageGeneratorTest : MonoBehaviour
{
    [Header("---- Image Generation Settings ----")]
    [SerializeField] private OpenAIImageGeneratorSO _imageGeneratorBrain; // Drag your OpenAIImageGeneratorSO here
    [SerializeField] private Image _imageDisplay; // Drag the UI Image component here

    [TextArea(3, 10)]
    [SerializeField] private string _prompt = "A futuristic city at sunset, cyberpunk style";

    void Start()
    {
        if (_imageGeneratorBrain == null)
        {
            Debug.LogError("Image Generator Brain no asignado.");
            return;
        }

        if (_imageDisplay == null)
        {
            Debug.LogError("Image Display no asignado.");
            return;
        }

        // Subscribe to the event
        _imageGeneratorBrain.OnSpriteGenerated += HandleSpriteGenerated;

        // Generate the image at start for testing
        GenerateAndDisplayImage();
    }

    void OnDestroy()
    {
        // Unsubscribe from the event to prevent memory leaks
        if (_imageGeneratorBrain != null)
        {
            _imageGeneratorBrain.OnSpriteGenerated -= HandleSpriteGenerated;
        }
    }

    [ContextMenu("Generate and Display Image")]
    public void GenerateAndDisplayImage()
    {
        if (_imageGeneratorBrain != null && !string.IsNullOrEmpty(_prompt))
        {
            Debug.Log($"Generando imagen para: '{_prompt}'");
            _imageGeneratorBrain.SendMessage(_prompt, OnImagePathReceived);
        }
        else
        {
            Debug.LogError("Image Generator Brain o prompt no asignado/vacío.");
        }
    }

    private void HandleSpriteGenerated(Sprite sprite)
    {
        if (sprite != null && _imageDisplay != null)
        {
            _imageDisplay.sprite = sprite;
            Debug.Log("Sprite recibido y asignado al UI Image.");
        }
    }

    private void OnImagePathReceived(string imagePath)
    {
        if (!string.IsNullOrEmpty(imagePath) && !imagePath.StartsWith("Error"))
        {
            Debug.Log($"Imagen generada y guardada en: {imagePath}");
        }
        else
        {
            Debug.LogError($"Error en la generación de imagen: {imagePath}");
        }
    }
}