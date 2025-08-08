using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Cody))]
public class ChatGPTEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Cody script = (Cody)target;

        GUILayout.Space(15);

        if(script.RequestType == RequestType.CreateScript)
        {
            if(GUILayout.Button("Generate Script"))
            {
                script.SendRequest();
            }
        }
        else
        {
            if(GUILayout.Button("Ask"))
            {
                script.SendRequest();
            }
        }    
        
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();

        if(script.RequestType == RequestType.CreateScript)
        {
            if(GUILayout.Button("Save Script"))
            {
                script.SaveScript();
            }
        }

        if(GUILayout.Button("Clear"))
        {
            script.Clear();
        }

        GUILayout.EndHorizontal();
    }
}
