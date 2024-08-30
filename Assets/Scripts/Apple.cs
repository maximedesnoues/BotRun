using UnityEngine;

public class Apple : MonoBehaviour
{
    // Amount of health to restore when the apple is collected
    [SerializeField] private int healthAmount;

    // Trigger detection when player enters the apple's collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Player"))
        {
            // Get the PlayerHealth component from the player and heal them if possible
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.Heal(healthAmount);

                // Increase the player's score using the ScoreManager
                ScoreManager scoreManager = FindObjectOfType<ScoreManager>();

                if (scoreManager != null)
                {
                    scoreManager.AddCollectable(0); // Assuming 0 points for collecting this item
                }

                // Destroy the apple after collection
                Destroy(gameObject);
            }
        }
    }
}