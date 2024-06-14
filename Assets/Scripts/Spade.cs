using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spade : MonoBehaviour
{
    [SerializeField] private int damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                if (playerController != null)
                {
                    playerController.Stumble();
                }
            }
        }
    }
}