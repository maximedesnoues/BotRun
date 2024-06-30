using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainSaw : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector3 pointA;
    [SerializeField] private Vector3 pointB;
    [SerializeField] private int damage;

    private Vector3 targetPoint;

    private void Start()
    {
        targetPoint = pointB;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, pointA) < 0.1f)
        {
            targetPoint = pointB;
        }
        else if (Vector3.Distance(transform.position, pointB) < 0.1f)
        {
            targetPoint = pointA;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}