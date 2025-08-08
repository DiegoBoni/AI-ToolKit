using System;
using UnityEngine;

public abstract class BrainSO : ScriptableObject
{
    [Header("---- General Settings ----")]
    [Password]
    [SerializeField] protected string _apiKey;

    public abstract void SendMessage(string message, Action<string> callback);
}
