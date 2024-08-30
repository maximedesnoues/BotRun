using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PedestrianAI : MonoBehaviour
{
    // Range within which the pedestrian will move
    [SerializeField] private float walkRange;

    // References to components
    private NavMeshAgent agent;
    private Animator animator;

    // Navigation mask to restrict movement to sidewalk areas
    private int sidewalkAreaMask;

    // Flag to track if the pedestrian has reached the destination
    private bool destinationReached = true;


    private void Start()
    {
        // Initialize components
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Set the navigation mask to the "Sidewalk" area
        sidewalkAreaMask = 1 << NavMesh.GetAreaFromName("Sidewalk");

        // Start the coroutine to continuously update the pedestrian's destination
        StartCoroutine(UpdateDestination());
    }


    private void Update()
    {
        // Check if the pedestrian has reached the current destination
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            destinationReached = true;
            animator.SetBool("IsWalking", false); // Stop walking animation
        }
    }


    // Coroutine to update the pedestrian's destination at regular intervals
    private IEnumerator UpdateDestination()
    {
        while (true)
        {
            if (destinationReached)
            {
                // Generate a random point within the specified range around the pedestrian
                Vector3 randomDirection = Random.insideUnitSphere * walkRange;
                randomDirection += transform.position;
                NavMeshHit hit;

                // Check if the point is on the sidewalk and set it as the new destination
                if (NavMesh.SamplePosition(randomDirection, out hit, walkRange, sidewalkAreaMask))
                {
                    agent.SetDestination(hit.position);
                    destinationReached = false;
                    animator.SetBool("IsWalking", true); // Start walking animation
                }
            }

            yield return new WaitForSeconds(0.1f); // Wait before checking for a new destination
        }
    }
}