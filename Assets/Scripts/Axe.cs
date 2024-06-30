using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private int damage;
    [SerializeField] private float pushBackForce;

    private void Update()
    {
        transform.Rotate(Vector3.up * -rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            Rigidbody playerRb = other.GetComponent<Rigidbody>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            if (playerRb != null)
            {
                Vector3 pushBackDirection = (other.transform.position - transform.position).normalized;
                pushBackDirection.y = 0;
                playerRb.velocity = Vector3.zero;
                playerRb.AddForce(pushBackDirection * pushBackForce, ForceMode.Impulse);
            }
        }
    }
}