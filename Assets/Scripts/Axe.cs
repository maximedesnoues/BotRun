using UnityEngine;

public class Axe : MonoBehaviour
{
    // Variables for axe rotation, damage, and pushback force
    [SerializeField] private float rotationSpeed;
    [SerializeField] private int damage;
    [SerializeField] private float pushBackForce;

    // Rotate the axe continuously around its Y-axis
    private void Update()
    {
        transform.Rotate(Vector3.up * -rotationSpeed * Time.deltaTime);
    }

    // Trigger detection when player enters the axe's collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Player"))
        {
            // Apply damage to the player if possible
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            Rigidbody playerRb = other.GetComponent<Rigidbody>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // Apply pushback force to the player if they have a Rigidbody
            if (playerRb != null)
            {
                Vector3 pushBackDirection = (other.transform.position - transform.position).normalized;
                pushBackDirection.y = 0; // Prevent vertical pushback
                playerRb.velocity = Vector3.zero; // Reset velocity before applying force
                playerRb.AddForce(pushBackDirection * pushBackForce, ForceMode.Impulse);
            }
        }
    }
}