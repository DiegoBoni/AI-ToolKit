#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
public class MyCoroutineManagerEditorInitializer
{
    static MyCoroutineManagerEditorInitializer()
    {
        // This ensures the MyCoroutineManager instance is created when the editor loads.
        MyCoroutineManager.Instance.name = "MyCoroutineManager (Editor)";
    }
}
#endif
