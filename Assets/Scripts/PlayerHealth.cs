using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Range(0, 100)] public int maxHealth; // Définir la santé maximale du joueur
    [Range(0, 100)] public int currentHealth; // Définir la santé actuelle du joueur
    
    public PlayerHealthBar healthBar; // Référence à la barre de santé pour les mises à jour visuelles
    
    [SerializeField] private float invincibilityDuration; // Durée de l'invincibilité après avoir pris des dégâts
    [SerializeField] private Renderer playerRenderer; // Renderer du joueur pour le clignotement

    private bool isInvincible = false;

    private void Start()
    {
        healthBar.SetMaxHealth(maxHealth); // Initialise la valeur maximale de la barre de santé
        currentHealth = maxHealth; // Assigne la santé maximale au démarrage
        UpdateHealthBar(); // Met à jour visuellement la barre de santé
    }

    private void OnValidate()
    {
        if (healthBar != null && Application.isPlaying)
        {
            UpdateHealthBar(); // Met à jour la barre de santé lors des modifications dans l'inspecteur, si l'application est en cours d'exécution
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible)
        {
            return;
        }

        currentHealth -= damage; // Applique les dommages à la santé
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Assure que la santé ne tombe pas en dessous de zéro
        UpdateHealthBar(); // Met à jour la barre de santé pour refléter le changement

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
        healthBar.SetHealth(currentHealth); // Met à jour la barre de santé avec la valeur actuelle
    }
}