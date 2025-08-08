using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class TriviaTest : MonoBehaviour
{
    [Header("---- Brain ----")]
    [SerializeField] private BrainSO _brain;
    [Space]

    [Header("---- Trivia Display Setting ----")]
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _question;
    [SerializeField] private Button[] answerButtons;
    [Space]

    [Header("---- Trivia Topics About ----")]
    [SerializeField] private string _aboutTrivia;
    [Space]

    [Header("---- GPT Response ----")]
    [TextArea(3, 40)]
    [SerializeField] private string _triviaJsonData;

    private string title;
    private string question;
    private string[] answers;
    private int correctAnswerIndex;

    private void Start()
    {
        SetTrivia();
    }

    public void SetTrivia()
    {
        Action<string> callback = (response) =>
        {
            _triviaJsonData = response;
            LoadDataFromJSONString(_triviaJsonData);
        };

        _brain.SendMessage(_aboutTrivia, callback);
        _triviaJsonData = "Thinking...";
    }

    public void LoadDataFromJSONString(string jsonString)
    {
        TriviaData jsonData = JsonUtility.FromJson<TriviaData>(jsonString);

        this.title = jsonData.title;
        this.question = jsonData.question;
        this.answers = jsonData.answers;
        this.correctAnswerIndex = jsonData.correctAnswerIndex;
        
        DisplayTrivia();
    }

    public void DisplayTrivia()
    {
        _title.text = title;
        _question.text = question;

        for (int i = 0; i < answers.Length; i++)
        {
            int answerIndex = i;
            answerButtons[i].onClick.RemoveAllListeners(); 
            answerButtons[i].onClick.AddListener(() =>
            {
                CheckAnswer(answerIndex);
            });

            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = answers[i];
        }
    }
    
    public void CheckAnswer(int selectedAnswerIndex)
    {
        if (selectedAnswerIndex == correctAnswerIndex)
        {
            Debug.Log("¡Respuesta correcta!");
            StartCoroutine(NextQuestion());
        }
        else
        {
            Debug.Log("¡Incorrecto! Perdiste la trivia.");
        }
    }

    private IEnumerator NextQuestion()
    {
        yield return new WaitForSeconds(0.5f); 
        SetTrivia();
    }

    [System.Serializable]
    private class TriviaData
    {
        public string title;
        public string question;
        public string[] answers;
        public int correctAnswerIndex;
    }
}