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

    private Timer timer;
    private ScoreManager scoreManager;
    private Defeat defeat;

    private bool isInvincible = false;

    private void Start()
    {
        healthBar.SetMaxHealth(maxHealth);
        currentHealth = maxHealth;
        UpdateHealthBar();

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
            ShowDefeatScreen();
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

    private void UpdateHealthBar()
    {
        healthBar.SetHealth(currentHealth);
    }

    private void ShowDefeatScreen()
    {
        int totalScore = scoreManager.GetTotalScore();
        float elapsedTime = timer.elapsedTime;
        int tricks = scoreManager.GetTricksPerformed();
        string objectsCollected = scoreManager.GetCollectablesCollected().ToString();

        defeat.ShowDefeatScreen(totalScore, elapsedTime, tricks, objectsCollected);
    }
}