using UnityEngine;

public class ChainSaw : MonoBehaviour
{
    // Variables for chainsaw movement and damage
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector3 pointA;
    [SerializeField] private Vector3 pointB;
    [SerializeField] private int damage;

    private Vector3 targetPoint;

    // Set initial target point for chainsaw movement
    private void Start()
    {
        targetPoint = pointB;
    }

    // Move the chainsaw between pointA and pointB
    private void Update()
    {
        // Move the chainsaw towards the target point
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, moveSpeed * Time.deltaTime);

        // Switch target point when the chainsaw reaches either pointA or pointB
        if (Vector3.Distance(transform.position, pointA) < 0.1f)
        {
            targetPoint = pointB;
        }
        else if (Vector3.Distance(transform.position, pointB) < 0.1f)
        {
            targetPoint = pointA;
        }
    }

    // Trigger detection when player enters the chainsaw's collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Player"))
        {
            // Apply damage to the player if possible
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}