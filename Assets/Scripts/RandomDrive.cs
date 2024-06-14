using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomDrive : MonoBehaviour
{
    [SerializeField] private float range;

    private NavMeshAgent agent;

    private int roadAreaMask;

    private bool destinationReached = true;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        roadAreaMask = 1 << NavMesh.GetAreaFromName("Road");

        StartCoroutine(UpdateDestination());
    }

    private void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            destinationReached = true;
        }
    }

    private IEnumerator UpdateDestination()
    {
        while (true)
        {
            if (destinationReached)
            {
                Vector3 randomDirection = Random.insideUnitSphere * range;
                randomDirection += transform.position;
                NavMeshHit hit;

                if (NavMesh.SamplePosition(randomDirection, out hit, range, roadAreaMask))
                {
                    agent.SetDestination(hit.position);
                    destinationReached = false;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}