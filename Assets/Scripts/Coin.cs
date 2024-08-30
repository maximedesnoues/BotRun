using UnityEngine;

public class Coin : MonoBehaviour
{
    // Variables to control coin rotation, sound, and particle effects
    [SerializeField] private Vector3 direction;
    [SerializeField] private AudioClip pickupSoundClip;
    [SerializeField] private GameObject pickupParticles;

    // Rotate the coin continuously in the specified direction
    private void Update()
    {
        transform.Rotate(direction * Time.deltaTime);
    }

    // Trigger detection when player enters the coin's collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Player"))
        {
            // Increase the player's score using the ScoreManager
            ScoreManager scoreManager = FindObjectOfType<ScoreManager>();

            if (scoreManager != null)
            {
                scoreManager.AddCollectable(20); // Add 20 points to the score for collecting the coin
            }

            // Play pickup sound if available
            AudioManager audioManager = other.GetComponent<AudioManager>();
            if (audioManager != null && pickupSoundClip != null)
            {
                audioManager.PlaySound(pickupSoundClip);
            }

            // Instantiate and destroy pickup particles
            if (pickupParticles != null)
            {
                GameObject particles = Instantiate(pickupParticles, transform.position, Quaternion.identity);
                Destroy(particles, 0.5f); // Destroy particles after 0.5 seconds
            }

            // Destroy the coin after collection
            Destroy(gameObject);
        }
    }
}