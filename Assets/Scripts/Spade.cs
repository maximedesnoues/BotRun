using UnityEngine;

public class Spade : MonoBehaviour
{
    // Damage dealt by the spade
    [SerializeField] private int damage;

    // Trigger detection when player enters the spade's collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Player"))
        {
            // Apply damage to the player
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // Make the player stumble if possible
            if (playerController != null)
            {
                playerController.Stumble();
            }
        }
    }
}