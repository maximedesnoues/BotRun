using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarAI : MonoBehaviour
{
    // List of waypoints the car will follow
    [SerializeField] private List<Transform> waypoints;

    // Reference to the NavMeshAgent component
    private NavMeshAgent agent;

    // Index of the current waypoint the car is heading towards
    private int currentWaypointIndex;

    private void Start()
    {
        // Initialize the NavMeshAgent component
        agent = GetComponent<NavMeshAgent>();

        // Find the closest waypoint in front of the car to start the path
        currentWaypointIndex = GetClosestWaypointIndexInFront();

        // Set the destination to the first waypoint if there are waypoints assigned
        if (waypoints.Count > 0)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    private void Update()
    {
        // Check if the car has reached the current waypoint
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            // Move to the next waypoint in the list, looping back to the start if necessary
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    // Method to find the closest waypoint that is in front of the car
    private int GetClosestWaypointIndexInFront()
    {
        int closestIndex = 0;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < waypoints.Count; i++)
        {
            // Calculate the direction from the car to the waypoint
            Vector3 directionToWaypoint = waypoints[i].position - transform.position;
            float distance = directionToWaypoint.sqrMagnitude; // Use squared magnitude for efficiency

            // Check if the waypoint is in front of the car and closer than the previously found waypoints
            if (Vector3.Dot(transform.forward, directionToWaypoint) > 0 && distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        // Return the index of the closest waypoint in front of the car
        return closestIndex;
    }
}