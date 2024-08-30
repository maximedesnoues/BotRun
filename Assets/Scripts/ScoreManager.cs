using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    // UI element to display the score
    [SerializeField] private Text scoreText;

    // Score components
    private int score = 0;
    private int timeScore = 0;
    private int trickScore = 0;
    private int tricksPerformed = 0;
    private int collectablesCollected = 0;

    private void Start()
    {
        // Initialize the score text at the start of the game
        UpdateScoreText();
    }

    // Method to add points to the time score
    public void AddTimeScore(int points)
    {
        timeScore += points;
        UpdateScore();
    }

    // Method to add points to the trick score and increment the tricks counter
    public void AddTrickScore(int points)
    {
        trickScore += points;
        tricksPerformed++;
        UpdateScore();
    }

    // Method to add points for collectables and increment the collectables counter
    public void AddCollectable(int points)
    {
        collectablesCollected++;
        score += points;
        UpdateScoreText();
    }

    // Method to update the total score by summing all score components
    private void UpdateScore()
    {
        score = timeScore + trickScore + collectablesCollected * 20;
        UpdateScoreText();
    }

    // Method to update the score text on the UI
    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }

    // Getter for the total score
    public int GetTotalScore()
    {
        return score;
    }

    // Getter for the trick score
    public int GetTrickScore()
    {
        return trickScore;
    }

    // Getter for the number of tricks performed
    public int GetTricksPerformed()
    {
        return tricksPerformed;
    }

    // Getter for the number of collectables collected
    public int GetCollectablesCollected()
    {
        return collectablesCollected;
    }
}