using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarController : MonoBehaviour
{
    [HideInInspector] public List<Transform> waypoints;
    
    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (waypoints.Count > 0)
        {
            SetDestinationToNearestWaypoint();
        }
    }

    private void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            SetDestinationToNearestWaypoint();
        }
    }

    private void SetDestinationToNearestWaypoint()
    {
        int nearestWaypointIndex = FindNearestWaypointIndex();
        if (nearestWaypointIndex != -1)
        {
            agent.SetDestination(waypoints[nearestWaypointIndex].position);
        }
    }

    private int FindNearestWaypointIndex()
    {
        int nearestIndex = -1;
        float minDistance = Mathf.Infinity;

        for (int i = 0; i < waypoints.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, waypoints[i].position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestIndex = i;
            }
        }

        return nearestIndex;
    }
}