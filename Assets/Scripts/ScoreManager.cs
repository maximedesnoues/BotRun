using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private Text scoreText;

    private int score = 0;
    private int timeScore = 0;
    private int trickScore = 0;
    private int tricksPerformed = 0;
    private int collectablesCollected = 0;

    private void Start()
    {
        UpdateScoreText();
    }

    public void AddTimeScore(int points)
    {
        timeScore += points;
        UpdateScore();
    }

    public void AddTrickScore(int points)
    {
        trickScore += points;
        tricksPerformed++;
        UpdateScore();
    }

    public void AddCollectable(int points)
    {
        collectablesCollected++;
        score += points;
        UpdateScoreText();
    }

    private void UpdateScore()
    {
        score = timeScore + trickScore + collectablesCollected * 20;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }

    public int GetTotalScore()
    {
        return score;
    }

    public int GetTrickScore()
    {
        return trickScore;
    }

    public int GetTricksPerformed()
    {
        return tricksPerformed;
    }

    public int GetCollectablesCollected()
    {
        return collectablesCollected;
    }
}