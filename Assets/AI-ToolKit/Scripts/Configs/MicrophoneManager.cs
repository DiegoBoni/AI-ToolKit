using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MicrophoneManager : MonoBehaviour
{
    public static MicrophoneManager Instance { get; private set; }

    private AudioSource _audioSource;
    private string _microphoneDevice;
    private const int RECORDING_FREQUENCY = 44100;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _audioSource = GetComponent<AudioSource>();

            if (Microphone.devices.Length > 0)
            {
                _microphoneDevice = Microphone.devices[0];
                Debug.Log($"[MicrophoneManager] Microphone found: {_microphoneDevice}");
            }
            else
            {
                Debug.LogError("[MicrophoneManager] No microphone devices found!");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartRecording()
    {
        if (string.IsNullOrEmpty(_microphoneDevice))
        {
            Debug.LogError("[MicrophoneManager] Cannot start recording, no microphone device found.");
            return;
        }

        _audioSource.clip = Microphone.Start(_microphoneDevice, true, 10, RECORDING_FREQUENCY);
        // Add a small delay to ensure the microphone has started
        Invoke(nameof(CheckRecordingStarted), 0.1f);
    }

    private void CheckRecordingStarted()
    {
        if (Microphone.IsRecording(_microphoneDevice))
        {
            Debug.Log("[MicrophoneManager] Recording started successfully.");
        }
        else
        {
            Debug.LogError("[MicrophoneManager] Microphone.Start() was called but is not recording.");
        }
    }

    public AudioClip StopRecording()
    {
        if (string.IsNullOrEmpty(_microphoneDevice))
        {
            Debug.LogWarning("[MicrophoneManager] StopRecording called but no microphone device.");
            return null;
        }

        if (!Microphone.IsRecording(_microphoneDevice))
        {
            Debug.LogWarning("[MicrophoneManager] StopRecording called but was not recording.");
            // This can happen if the recording fails to start.
            return null;
        }

        int lastSample = Microphone.GetPosition(null);
        Microphone.End(_microphoneDevice);
        Debug.Log($"[MicrophoneManager] Recording stopped. Last sample position: {lastSample}");

        if (lastSample <= 0)
        {
            Debug.LogError("[MicrophoneManager] No audio was recorded. The clip is empty.");
            return null;
        }

        // Create a new clip with the exact length of the recording
        AudioClip recordedClip = AudioClip.Create("RecordedAudio", lastSample, _audioSource.clip.channels, _audioSource.clip.frequency, false);
        float[] data = new float[lastSample * _audioSource.clip.channels];
        _audioSource.clip.GetData(data, 0);
        recordedClip.SetData(data, 0);

        Debug.Log($"[MicrophoneManager] AudioClip created with length: {recordedClip.length} seconds.");
        return recordedClip;
    }
}