using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.IO;
using UnityEditor;

public class Cody : MonoBehaviour
{
    private const string _QUESTION_CONTEXTS = "Este asistente está diseñado para ayudar a los desarrolladores ofreciendo soluciones, prácticas recomendadas y optimización para mejorar la eficiencia del desarrollo dentro del entorno de Unity y C#.";
    private const string _CODE_CONTEXTS = "Solo vas a crear la clase en C# para Unity para la tarea que se solicita, basándose en los principios de Clean Code para mejorar la legibilidad del código, verifica que no haya textos que no correspondan a la clase, no agregues ningun comments explicativos en los metodos ni en las variables, todo en ingles";

    [SerializeField] private BrainSO _brainSO;
    
    public RequestType RequestType;

    [TextArea(3, 10)]
    [SerializeField] private string _prompt;
    [TextArea(3, 40)]
    [SerializeField] private string _result;

    private string scriptFolder = "Assets/AI-Toolkit/Resources/GeneratedScripts";

    public void SendRequest()
    {
        string prompt = string.Empty;

        switch (RequestType)
        {
            case RequestType.CreateScript:
                prompt = _CODE_CONTEXTS + _prompt;
                break;
            case RequestType.MakeQuery:
                prompt = _QUESTION_CONTEXTS + _prompt;
                break;
            default:
                Debug.LogError("Invalid Request Type");
                break;
        }

        _result = "Thinking...";
        bool isFirstChunk = true;

        Action<string> callback = (chunk) =>
        {
            if (isFirstChunk)
            {
                _result = string.Empty;
                isFirstChunk = false;
            }
            _result += chunk;
        };

        _brainSO.SendMessage(prompt, callback);
    }

    public void OnResponseReceived(string response)
    {
        _result = response;
    }

    public void Clear()
    {
        _prompt = string.Empty;
        _result = string.Empty;
    }

    public void SaveScript()
    {
        string fullFolderPath = "Assets/AI-Toolkit/Resources/GeneratedScripts";
        if (!Directory.Exists(fullFolderPath))
        {
            Directory.CreateDirectory(fullFolderPath);
        }

        string className = ParseClassName(_result);
        string scriptPath = Path.Combine(fullFolderPath, className + ".cs");
        using (FileStream fs = new FileStream(scriptPath, FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.Write(_result);
            }
        }

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    public string ParseClassName(string result)
    {
        int indexClass = result.IndexOf("class");
        int indexDots = result.IndexOf(":");

        string className = result.Substring(indexClass + 6, indexDots - indexClass - 6 - 1);
        return className;
    }
}

public enum RequestType
{
    CreateScript,
    MakeQuery
}
