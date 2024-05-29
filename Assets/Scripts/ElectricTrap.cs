using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricTrap : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float shockInterval;
    
    private bool isShocking = false;

    private void Start()
    {
        InvokeRepeating(nameof(ToggleShock), shockInterval, shockInterval);
    }

    private void ToggleShock()
    {
        isShocking = !isShocking;
        // Ajout d'effets visuels ou sonores pour indiquer l'état de choc
        // Par exemple, changer la couleur ou jouer un son
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isShocking && other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}