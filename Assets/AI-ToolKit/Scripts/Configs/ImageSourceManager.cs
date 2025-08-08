using UnityEngine;

public class ImageSourceManager : MonoBehaviour
{
    public static ImageSourceManager Instance { get; private set; }

    [Header("---- Test Image ----")]
    [Tooltip("An image to use for analysis when not using the webcam.")]
    [SerializeField] private Texture2D _testImage;

    private WebCamTexture _webCamTexture;
    private bool _isWebcamReady = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializeWebcam(int width, int height)
    {
        if (WebCamTexture.devices.Length == 0)
        {
            Debug.LogError("[ImageSourceManager] No webcam found.");
            return;
        }

        WebCamDevice device = WebCamTexture.devices[0];
        _webCamTexture = new WebCamTexture(device.name, width, height);
        _webCamTexture.Play();
        _isWebcamReady = true;
        Debug.Log("[ImageSourceManager] Webcam initialized and playing.");
    }

    // Returns the live webcam feed as a generic Texture for UI display
    public Texture GetWebcamTexture()
    {
        if (!_isWebcamReady || !_webCamTexture.isPlaying)
        {
            Debug.LogError("[ImageSourceManager] Webcam is not initialized or not playing.");
            return null;
        }
        return _webCamTexture;
    }

    public Texture2D GetTestImage()
    {
        return _testImage;
    }

    // Returns a static snapshot of the current webcam frame as a Texture2D for analysis
    public Texture2D GetCurrentWebcamFrame()
    {
        if (!_isWebcamReady)
        {
            Debug.LogError("[ImageSourceManager] Cannot get frame, webcam not ready.");
            return null;
        }
        Texture2D photo = new Texture2D(_webCamTexture.width, _webCamTexture.height);
        photo.SetPixels(_webCamTexture.GetPixels());
        photo.Apply();
        return photo;
    }

    public void StopWebcam()
    {
        if (_webCamTexture != null && _webCamTexture.isPlaying)
        {
            _webCamTexture.Stop();
            _isWebcamReady = false;
            Debug.Log("[ImageSourceManager] Webcam stopped.");
        }
    }
}