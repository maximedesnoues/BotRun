using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoost : MonoBehaviour
{
    [SerializeField] private int scoreAmount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
            if (scoreManager != null)
            {
                scoreManager.AddScore(scoreAmount);
                Destroy(gameObject);
            }
        }
    }
}