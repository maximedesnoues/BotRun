using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    // Public variable to track elapsed time
    [HideInInspector] public float elapsedTime = 0f;

    // UI element to display the timer
    [SerializeField] private Text timerText;

    private void Start()
    {
        // Initialize the timer display
        UpdateTimerText();
    }

    private void Update()
    {
        // Increment elapsed time and update the timer display
        elapsedTime += Time.deltaTime;
        UpdateTimerText();
    }

    // Updates the timer text with the current elapsed time
    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60F);
        int seconds = Mathf.FloorToInt(elapsedTime - minutes * 60);
        string text = string.Format("Timer: {0:0}:{1:00}", minutes, seconds);
        timerText.text = text;
    }

    // Returns the formatted elapsed time as a string (mm:ss)
    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60F);
        int seconds = Mathf.FloorToInt(elapsedTime - minutes * 60);
        return string.Format("{0:0}:{1:00}", minutes, seconds);
    }
}