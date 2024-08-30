using UnityEngine;

public class SodaCan : MonoBehaviour
{
    // Amount of boost to provide when the soda can is collected
    [SerializeField] private float boostAmount;

    // Trigger detection when player enters the soda can's collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Player"))
        {
            // Get the BoostManager component to apply the boost
            BoostManager boostManager = FindObjectOfType<BoostManager>();

            if (boostManager != null)
            {
                boostManager.AddBoost(boostAmount);

                // Increase the player's score using the ScoreManager
                ScoreManager scoreManager = FindObjectOfType<ScoreManager>();

                if (scoreManager != null)
                {
                    scoreManager.AddCollectable(0); // Assuming 0 points for collecting this item
                }

                // Destroy the soda can after collection
                Destroy(gameObject);
            }
        }
    }
}