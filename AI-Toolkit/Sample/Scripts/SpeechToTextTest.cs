using UnityEngine;
using TMPro;

public class SpeechToTextTest : MonoBehaviour
{
    [Header("---- AI Brain ----")]
    [SerializeField] private OpenAISpeechToTextSO _speechToTextBrain;

    [Header("---- UI Components ----")]
    [SerializeField] private PushToTalkButton _talkButton;
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private TextMeshProUGUI _resultText;

    private void Start()
    {
        if (!ValidateComponents())
        {
            return;
        }

        // Subscribe to the button events in code
        _talkButton.OnButtonPressed += StartRecording;
        _talkButton.OnButtonReleased += StopRecordingAndTranscribe;

        SetStatus("Listo para hablar");
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (_talkButton != null)
        {
            _talkButton.OnButtonPressed -= StartRecording;
            _talkButton.OnButtonReleased -= StopRecordingAndTranscribe;
        }
    }

    private void StartRecording()
    {
        Debug.Log("[SpeechToTextTest] Event received: StartRecording");
        MicrophoneManager.Instance.StartRecording();
        SetStatus("Escuchando...");
        if(_resultText) _resultText.text = "...";
    }

    private void StopRecordingAndTranscribe()
    {
        Debug.Log("[SpeechToTextTest] Event received: StopRecordingAndTranscribe");
        SetStatus("Procesando...");
        AudioClip recordedClip = MicrophoneManager.Instance.StopRecording();

        if (recordedClip != null)
        {
            Debug.Log("[SpeechToTextTest] AudioClip received, sending to brain for transcription.");
            _speechToTextBrain.Transcribe(recordedClip, OnTranscriptionComplete);
        }
        else
        {
            Debug.LogError("[SpeechToTextTest] Received null AudioClip from MicrophoneManager.");
            SetStatus("Error al grabar");
        }
    }

    private void OnTranscriptionComplete(string transcription)
    {
        if (!string.IsNullOrEmpty(transcription))
        {
            _resultText.text = transcription;
            SetStatus("Listo para hablar");
        }
        else
        {
            _resultText.text = "<No se pudo transcribir el audio>";
            SetStatus("Error en la transcripción");
        }
    }

    private bool ValidateComponents()
    {
        if (_speechToTextBrain == null)
        {
            Debug.LogError("[SpeechToTextTest] Speech-to-Text Brain no asignado.");
            if(_talkButton) _talkButton.gameObject.SetActive(false);
            SetStatus("Error: Brain no asignado");
            return false;
        }

        if (_talkButton == null)
        {
            Debug.LogError("[SpeechToTextTest] PushToTalkButton no asignado.");
            SetStatus("Error: Botón no asignado");
            return false;
        }

        if (MicrophoneManager.Instance == null)
        {
            Debug.LogError("[SpeechToTextTest] MicrophoneManager no se encuentra en la escena.");
            _talkButton.gameObject.SetActive(false);
            SetStatus("Error: MicrophoneManager no encontrado");
            return false;
        }
        return true;
    }

    private void SetStatus(string status)
    {
        if (_statusText != null)
        {
            _statusText.text = status;
        }
    }
}