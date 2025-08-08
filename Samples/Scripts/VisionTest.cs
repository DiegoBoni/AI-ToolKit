using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VisionTest : MonoBehaviour
{
    [Header("---- AI Brain ----")]
    [SerializeField] private OpenAIVisionSO _visionBrain;

    [Header("---- UI Components ----")]
    [SerializeField] private RawImage _imageDisplay;
    [SerializeField] private TMP_InputField _promptInputField;
    [SerializeField] private Button _takePhotoButton;
    [SerializeField] private Button _analyzeButton;
    [SerializeField] private Button _retakeButton;
    [SerializeField] private TextMeshProUGUI _resultText;

    private Texture2D _capturedPhoto;

    private void Start()
    {
        Debug.Log("[VisionTest] Starting... Validating components.");
        if (!ValidateComponents()) return;

        // Subscribe to button clicks via code for better readability and robustness
        _takePhotoButton.onClick.AddListener(TakePicture);
        _analyzeButton.onClick.AddListener(AnalyzeCapturedImage);
        _retakeButton.onClick.AddListener(RetakePicture);

        Debug.Log("[VisionTest] Initializing Webcam...");
        ImageSourceManager.Instance.InitializeWebcam((int)_imageDisplay.rectTransform.rect.width, (int)_imageDisplay.rectTransform.rect.height);
        _imageDisplay.texture = ImageSourceManager.Instance.GetWebcamTexture();

        // Initial UI State
        SetInitialUIState();
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        _takePhotoButton.onClick.RemoveListener(TakePicture);
        _analyzeButton.onClick.RemoveListener(AnalyzeCapturedImage);
        _retakeButton.onClick.RemoveListener(RetakePicture);

        if (ImageSourceManager.Instance != null)
        {
            ImageSourceManager.Instance.StopWebcam();
        }
    }

    private void SetInitialUIState()
    {
        _takePhotoButton.gameObject.SetActive(true);
        _analyzeButton.gameObject.SetActive(false);
        _retakeButton.gameObject.SetActive(false);
        _resultText.text = "Apuntá con la cámara y tomá una foto.";
    }

    private void TakePicture()
    {
        Debug.Log("[VisionTest] TakePicture button clicked.");
        _capturedPhoto = ImageSourceManager.Instance.GetCurrentWebcamFrame();
        if (_capturedPhoto == null) return;

        _imageDisplay.texture = _capturedPhoto;

        // Update UI State
        _takePhotoButton.gameObject.SetActive(false);
        _analyzeButton.gameObject.SetActive(true);
        _retakeButton.gameObject.SetActive(true);
        _resultText.text = "Foto capturada. Escribí un prompt y analizá.";
    }

    private void AnalyzeCapturedImage()
    {
        if (_capturedPhoto == null) return;

        string prompt = _promptInputField.text;
        if (string.IsNullOrEmpty(prompt)) prompt = "What do you see in this image? Be descriptive.";

        _resultText.text = "Analizando imagen...";
        _analyzeButton.interactable = false;
        _retakeButton.interactable = false;

        _visionBrain.AnalyzeImage(_capturedPhoto, prompt, OnAnalysisComplete);
    }

    private void RetakePicture()
    {
        _capturedPhoto = null;
        _imageDisplay.texture = ImageSourceManager.Instance.GetWebcamTexture();
        SetInitialUIState();
        _analyzeButton.interactable = true;
        _retakeButton.interactable = true;
    }

    private void OnAnalysisComplete(string result)
    {
        _resultText.text = result ?? "Error en el análisis. Revisa la consola.";
        _analyzeButton.interactable = true;
        _retakeButton.interactable = true;
    }

    private bool ValidateComponents()
    {
        // Simplified validation, you can add more detailed logs if needed
        if (_visionBrain == null || _imageDisplay == null || _promptInputField == null || 
            _takePhotoButton == null || _analyzeButton == null || _retakeButton == null || 
            _resultText == null || ImageSourceManager.Instance == null)
        {
            Debug.LogError("[VisionTest] A component is not assigned in the Inspector! Please check all fields.");
            return false;
        }
        return true;
    }
}
