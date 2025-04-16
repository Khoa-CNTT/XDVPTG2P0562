using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HealthSystem : MonoBehaviour
{
    [Header("Settings")]
    public int maxHealth = 100;
    public Animator animator;
    public Slider healthSlider;
    public TextMeshProUGUI healthBarValueText;

    [Header("Respawn Settings")]
    public float respawnDelay = 1.5f;

    public int currentHealth;
    private bool isDead = false;
    private PlayerRespawn playerRespawn;
    private PlayerController1 playerController1;



    void Awake()
    {
        playerController1 = GetComponent<PlayerController1>();
        playerRespawn = GetComponent<PlayerRespawn>();

        if (playerRespawn == null)
        {
            Debug.LogError("PlayerRespawn component not found!");
        }

        currentHealth = maxHealth;
        InitializeHealthBar();
    }

    void InitializeHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        if (healthBarValueText != null)
        {
            healthBarValueText.text = currentHealth + "/" + maxHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        UpdateHealthBar();
        animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
        if (healthBarValueText != null)
        {
            healthBarValueText.text = currentHealth + "/" + maxHealth;
        }
    }

    public void Die()
    {
        GameManager.Instance.OnPlayerDeath(transform.position);
        Debug.Log("Player died!");
        if (isDead) return; // Tránh gọi nhiều lần
        isDead = true;
        animator.SetBool("IsDead", true);
        playerController1.enabled = false;
        DisableAllColliders();
        StopPhysicsMovement();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetMoney();
        }

        if (playerRespawn != null)
        {
            Invoke(nameof(CallRespawn), respawnDelay);
        }
    }

    void DisableAllColliders()
    {
        Collider2D[] colliders2D = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders2D)
        {
            col.enabled = false;
        }
    }

    void StopPhysicsMovement()
    {
        Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
        if (rb2D != null)
        {
            rb2D.linearVelocity = Vector2.zero;
            rb2D.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    private void CallRespawn()
    {
        playerRespawn.Respawn();
        OnRespawn();
    }

    private void OnRespawn()
    {
        GameManager.Instance.OnPlayerRespawn();
        Debug.Log("Player respawned!");

        currentHealth = maxHealth;
        isDead = false;
        animator.SetBool("IsDead", false);
        animator.Rebind();
        animator.Update(0f);
        playerController1.enabled = true;
        EnableAllColliders();
        EnablePhysicsMovement();
        
        gameObject.SetActive(true);
        UpdateHealthBar();
        
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void Heal(int amount)
    {
        if (isDead) return;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UpdateHealthBar();
        Debug.Log("Player healed by " + amount);
    }

    void EnableAllColliders()
    {
        Collider2D[] colliders2D = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders2D)
        {
            col.enabled = true;
        }
    }

    void EnablePhysicsMovement()
    {
        Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
        if (rb2D != null)
        {
            rb2D.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    public void HealToFull()
    {
        currentHealth = maxHealth;
        Debug.Log("Health restored to full.");
    }
}