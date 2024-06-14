using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [HideInInspector] public float elapsedTime = 0f;

    [SerializeField] private Text timerText;

    private void Start()
    {
        UpdateTimerText();
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60F);
        int seconds = Mathf.FloorToInt(elapsedTime - minutes * 60);
        string text = string.Format("Timer: {0:0}:{1:00}", minutes, seconds);
        timerText.text = text;
    }

    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60F);
        int seconds = Mathf.FloorToInt(elapsedTime - minutes * 60);
        return string.Format("{0:0}:{1:00}", minutes, seconds);
    }
}