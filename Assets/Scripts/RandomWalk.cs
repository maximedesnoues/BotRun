using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomWalk : MonoBehaviour
{
    [SerializeField] private float range;

    private NavMeshAgent agent;
    private Animator animator;

    private int sidewalkAreaMask;

    private bool destinationReached = true;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        sidewalkAreaMask = 1 << NavMesh.GetAreaFromName("Sidewalk");

        StartCoroutine(UpdateDestination());
    }

    private void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            destinationReached = true;
            animator.SetBool("IsWalking", false);
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

                if (NavMesh.SamplePosition(randomDirection, out hit, range, sidewalkAreaMask))
                {
                    agent.SetDestination(hit.position);
                    destinationReached = false;
                    animator.SetBool("IsWalking", true);
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}