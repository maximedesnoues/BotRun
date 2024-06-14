using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SodaCan : MonoBehaviour
{
    [SerializeField] private float boostAmount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BoostManager boostManager = FindObjectOfType<BoostManager>();

            if (boostManager != null)
            {
                boostManager.AddBoost(boostAmount);

                Destroy(gameObject);
            }
        }
    }
}