using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class MyCoroutineManager : MonoBehaviour
{
    private static MyCoroutineManager _instance;

    public static MyCoroutineManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Try to find an existing instance in the scene
                _instance = FindObjectOfType<MyCoroutineManager>();

                // If no instance exists, create a new one
                if (_instance == null)
                {
                    var go = new GameObject("MyCoroutineManager");
                    _instance = go.AddComponent<MyCoroutineManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        // Singleton pattern implementation
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
#if UNITY_EDITOR
            // Use DestroyImmediate in the editor
            if (!Application.isPlaying) DestroyImmediate(gameObject);
            else Destroy(gameObject);
#else
            Destroy(gameObject);
#endif
            return;
        }

#if UNITY_EDITOR
        // This ensures the manager persists across scene loads in the editor
        // but not when entering play mode from an editor script.
        if (!Application.isPlaying)
        {
            hideFlags = HideFlags.HideAndDontSave;
        }
#endif

        // In a build, we want this to persist across scenes
#if !UNITY_EDITOR
        DontDestroyOnLoad(gameObject);
#endif
    }

#if UNITY_EDITOR
    // Static constructor for editor-specific setup
    static MyCoroutineManager()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        // Destroy the editor instance when entering play mode
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            if (_instance != null)
            {
                DestroyImmediate(_instance.gameObject);
                _instance = null;
            }
        }
    }
#endif

    // Overload StartCoroutine to be accessible from non-MonoBehaviour scripts
    public new Coroutine StartCoroutine(IEnumerator routine)
    {
        return base.StartCoroutine(routine);
    }
}