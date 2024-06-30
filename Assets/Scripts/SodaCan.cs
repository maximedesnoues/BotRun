using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SodaCan : MonoBehaviour
{
    [SerializeField] private float boostAmount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BoostManager boostManager = FindObjectOfType<BoostManager>();

            if (boostManager != null)
            {
                boostManager.AddBoost(boostAmount);

                ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
                
                if (scoreManager != null)
                {
                    scoreManager.AddCollectable(0);
                }

                Destroy(gameObject);
            }
        }
    }
}