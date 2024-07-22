using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDetector : MonoBehaviour
{
    private Timer timer;
    private ScoreManager scoreManager;
    private Defeat defeat;

    private void Start()
    {
        timer = FindObjectOfType<Timer>();
        scoreManager = FindObjectOfType<ScoreManager>();
        defeat = FindObjectOfType<Defeat>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            defeat.ShowDefeatScreen(scoreManager.GetTotalScore(), timer.elapsedTime, scoreManager.GetTricksPerformed(), scoreManager.GetCollectablesCollected().ToString());
        }
    }
}