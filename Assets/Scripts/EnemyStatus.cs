using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStatus : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hurtSound;
    public Animator animator;
    public int maxHealth;
    public Slider healthSlider;

    [Header("Enemy Settings")]
    [SerializeField] private int moneyDrop = 10;
    private int currentHealth;
    private bool isDead = false;

    public Enemy_Behaviour enemyBehaviour;

    [Header("Potion Drop Settings")]
    [SerializeField] private float potionDropRate = 0.3f; // Tỉ lệ rơi bình máu (0.3 = 30%)
    [SerializeField] private GameObject healthPotionPrefab; // Prefab bình máu

    void Start()
    {
        enemyBehaviour = GetComponent<Enemy_Behaviour>();
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
        else
        {
            Debug.LogError("Gán Slider vào Health System!");
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        animator.SetTrigger("Hurt");
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (audioSource != null && hurtSound != null)
        {
            audioSource.PlayOneShot(hurtSound);
        }

        Debug.Log("Enemy took " + damage + " damage!");

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }

        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    void Die()
    {
        isDead = true;

        if (enemyBehaviour != null)
        {
            enemyBehaviour.MoveSpeed = 0;
        }

        DisableAllColliders();
        StopPhysicsMovement();

        Debug.Log("Enemy died!");
        animator.SetBool("IsDead", true);

        GameManager.Instance.AddMoney(moneyDrop);

        TryDropPotion();

        Destroy(gameObject, 0.75f);
    }

    void TryDropPotion()
    {
        if (healthPotionPrefab == null)
        {
            Debug.LogWarning("⚠ Không gán prefab HealthPotion!");
            return;
        }

        float roll = UnityEngine.Random.value; // giá trị từ 0 → 1
        if (roll <= potionDropRate)
        {
            Instantiate(healthPotionPrefab, transform.position, Quaternion.identity);
            Debug.Log("💧 Rơi ra 1 bình máu!");
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

    public bool IsDead()
    {
        return isDead;
    }

    public void ResetEnemy()
    {
        currentHealth = maxHealth;
        isDead = false;
        enemyBehaviour.enabled = true;

        EnableAllColliders();
        EnablePhysicsMovement();
        gameObject.SetActive(true);
        Debug.Log("Enemy reset!");
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
}
