using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // Player health values
    [Range(0, 100)] public int maxHealth;
    [Range(0, 100)] public int currentHealth;

    // Reference to the health bar UI component
    public PlayerHealthBar healthBar;

    // Invincibility settings
    [SerializeField] private float invincibilityDuration;
    [SerializeField] private Renderer playerRenderer;

    // References to other components
    private PlayerController playerController;
    private Animator animator;
    private Timer timer;
    private ScoreManager scoreManager;
    private Defeat defeat;

    // Flag to check if the player is invincible
    private bool isInvincible = false;

    private void Start()
    {
        // Initialize health and update health bar
        healthBar.SetMaxHealth(maxHealth);
        currentHealth = maxHealth;
        UpdateHealthBar();

        // Get necessary components
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        timer = FindObjectOfType<Timer>();
        scoreManager = FindObjectOfType<ScoreManager>();
        defeat = FindObjectOfType<Defeat>();
    }

    private void OnValidate()
    {
        // Update health bar in editor if health changes
        if (healthBar != null && Application.isPlaying)
        {
            UpdateHealthBar();
        }
    }

    // Method to apply damage to the player
    public void TakeDamage(int damage)
    {
        if (isInvincible)
        {
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();

        if (currentHealth > 0)
        {
            StartCoroutine(InvincibilityCoroutine()); // Start invincibility if still alive
        }
        else
        {
            StartCoroutine(HandleDeath()); // Handle death if health is 0
        }
    }

    // Method to heal the player
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    // Coroutine to handle invincibility after taking damage
    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        float endTime = Time.time + invincibilityDuration;

        while (Time.time < endTime)
        {
            playerRenderer.enabled = !playerRenderer.enabled; // Toggle visibility
            yield return new WaitForSeconds(0.1f);
        }

        playerRenderer.enabled = true;
        isInvincible = false;
    }

    // Coroutine to handle player death
    private IEnumerator HandleDeath()
    {
        playerController.DisableControls();

        animator.SetTrigger("Death");

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); // Wait for death animation to finish

        defeat.ShowDefeatScreen(scoreManager.GetTotalScore(), timer.elapsedTime, scoreManager.GetTricksPerformed(), scoreManager.GetCollectablesCollected().ToString());
    }

    // Update the health bar UI with the current health
    private void UpdateHealthBar()
    {
        healthBar.SetHealth(currentHealth);
    }
}