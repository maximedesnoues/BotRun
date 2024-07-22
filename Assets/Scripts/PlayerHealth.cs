using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Range(0, 100)] public int maxHealth;
    [Range(0, 100)] public int currentHealth;
    
    public PlayerHealthBar healthBar;

    [SerializeField] private float invincibilityDuration;
    [SerializeField] private Renderer playerRenderer;

    private PlayerController playerController;
    private Animator animator;
    private Timer timer;
    private ScoreManager scoreManager;
    private Defeat defeat;

    private bool isInvincible = false;

    private void Start()
    {
        healthBar.SetMaxHealth(maxHealth);
        currentHealth = maxHealth;
        UpdateHealthBar();

        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        timer = FindObjectOfType<Timer>();
        scoreManager = FindObjectOfType<ScoreManager>();
        defeat = FindObjectOfType<Defeat>();
    }

    private void OnValidate()
    {
        if (healthBar != null && Application.isPlaying)
        {
            UpdateHealthBar();
        }
    }

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
            StartCoroutine(InvincibilityCoroutine());
        }
        else
        {
            StartCoroutine(HandleDeath());
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
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

    private IEnumerator HandleDeath()
    {
        playerController.DisableControls();

        animator.SetTrigger("Death");

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        defeat.ShowDefeatScreen(scoreManager.GetTotalScore(), timer.elapsedTime, scoreManager.GetTricksPerformed(), scoreManager.GetCollectablesCollected().ToString());
    }

    private void UpdateHealthBar()
    {
        healthBar.SetHealth(currentHealth);
    }
}