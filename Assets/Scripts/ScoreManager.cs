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
        UpdateScore();
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
    }

    private void UpdateScore()
    {
        score = timeScore + trickScore;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score : " + score;
    }
}