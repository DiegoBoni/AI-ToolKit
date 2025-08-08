using UnityEngine;
using System;

public class TTSPlayerTest : MonoBehaviour
{
    [Header("---- TTS Settings ----")]
    [SerializeField] private OpenAITextToSpeech _ttsBrain; // Drag your OpenAITextToSpeech SO here
    [SerializeField] private AudioSource _audioSource; // Drag your AudioSource component here

    [TextArea(3, 10)]
    [SerializeField] private string _textToSpeak = "Hola Diego, ¿cómo andamos? ¡Probando el nuevo Text-to-Speech!";

    void Start()
    {
        if (_ttsBrain == null)
        {
            Debug.LogError("TTS Brain no asignado. Por favor, arrastrá un OpenAITextToSpeech Scriptable Object al campo '_ttsBrain' en el Inspector.");
            return;
        }

        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
                Debug.LogWarning("No se encontró un AudioSource. Se agregó uno automáticamente al GameObject.");
            }
        }

        // Generar el audio al inicio para probar
        GenerateAndPlaySpeech();
    }

    [ContextMenu("Generate and Play Speech")]
    public void GenerateAndPlaySpeech()
    {
        if (_ttsBrain != null && !string.IsNullOrEmpty(_textToSpeak))
        {
            Debug.Log($"Generando audio para: '{_textToSpeak}'");
            _ttsBrain.GenerateSpeech(_textToSpeak, OnSpeechGenerated);
        }
        else
        {
            Debug.LogError("TTS Brain o texto a hablar no asignado/vacío.");
        }
    }

    private void OnSpeechGenerated(AudioClip audioClip)
    {
        if (audioClip != null)
        {
            _audioSource.clip = audioClip;
            _audioSource.Play();
            Debug.Log("Audio generado y reproduciéndose.");
        }
        else
        {
            Debug.LogError("No se pudo generar el audio. Revisá los logs de errores de la API de OpenAI.");
        }
    }
}
