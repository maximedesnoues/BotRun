using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{
    [SerializeField] private int healthAmount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.Heal(healthAmount);

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