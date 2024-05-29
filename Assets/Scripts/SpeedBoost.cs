using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.ActivateSpeedBoost(GameManager.Instance.speedBoostMultiplier, GameManager.Instance.boostDuration);
                Destroy(gameObject);
            }
        }
    }
}