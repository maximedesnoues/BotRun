using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Range(0, 100)] public int maxHealth; // D�finir la sant� maximale du joueur
    [Range(0, 100)] public int currentHealth; // D�finir la sant� actuelle du joueur
    
    public PlayerHealthBar healthBar; // R�f�rence � la barre de sant� pour les mises � jour visuelles
    
    [SerializeField] private float invincibilityDuration; // Dur�e de l'invincibilit� apr�s avoir pris des d�g�ts
    [SerializeField] private Renderer playerRenderer; // Renderer du joueur pour le clignotement

    private bool isInvincible = false;

    private void Start()
    {
        healthBar.SetMaxHealth(maxHealth); // Initialise la valeur maximale de la barre de sant�
        currentHealth = maxHealth; // Assigne la sant� maximale au d�marrage
        UpdateHealthBar(); // Met � jour visuellement la barre de sant�
    }

    private void OnValidate()
    {
        if (healthBar != null && Application.isPlaying)
        {
            UpdateHealthBar(); // Met � jour la barre de sant� lors des modifications dans l'inspecteur, si l'application est en cours d'ex�cution
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible)
        {
            return;
        }

        currentHealth -= damage; // Applique les dommages � la sant�
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Assure que la sant� ne tombe pas en dessous de z�ro
        UpdateHealthBar(); // Met � jour la barre de sant� pour refl�ter le changement

        if (currentHealth > 0)
        {
            StartCoroutine(InvincibilityCoroutine());
        }
        else
        {
            // Ajout de la logique de mort du joueur
        }
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        float endTime = Time.time + invincibilityDuration;

        while (Time.time < endTime)
        {
            playerRenderer.enabled = !playerRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
        }

        playerRenderer.enabled = true;
        isInvincible = false;
    }

    private void UpdateHealthBar()
    {
        healthBar.SetHealth(currentHealth); // Met � jour la barre de sant� avec la valeur actuelle
    }
}