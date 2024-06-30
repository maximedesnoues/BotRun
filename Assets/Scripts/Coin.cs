using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private Vector3 direction;
    [SerializeField] private AudioClip pickupSoundClip;
    [SerializeField] private GameObject pickupParticles;

    private void Update()
    {
        transform.Rotate(direction * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ScoreManager scoreManager = FindObjectOfType<ScoreManager>();

            if (scoreManager != null)
            {
                scoreManager.AddCollectable(20);
            }

            AudioManager audioManager = other.GetComponent<AudioManager>();
            if (audioManager != null && pickupSoundClip != null)
            {
                audioManager.PlaySound(pickupSoundClip);
            }

            if (pickupParticles != null)
            {
                GameObject particles = Instantiate(pickupParticles, transform.position, Quaternion.identity);
                Destroy(particles, 0.5f);
            }

            Destroy(gameObject);
        }
    }
}