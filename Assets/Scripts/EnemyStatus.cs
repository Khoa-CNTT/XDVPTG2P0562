using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStatus : MonoBehaviour
{
    public Animator animator;
    public int maxHealth;
    public Slider healthSlider;

    [Header("Enemy Settings")]
    [SerializeField] private int moneyDrop = 10; // Tiền thưởng khi giết địch
    private int currentHealth;
    private bool isDead = false;
    
    public Enemy_Behaviour enemyBehaviour; // Tham chiếu tới Enemy_Behaviour

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
        if (isDead) return; // Chặn nhận damage nếu đã chết

        currentHealth -= damage;
        animator.SetTrigger("Hurt");
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

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
        isDead = true; // Đánh dấu enemy đã chết
        
        
        // ✅ Đặt tốc độ di chuyển về 0
        if (enemyBehaviour != null)
        {
            enemyBehaviour.MoveSpeed = 0;
            
        }

        // Tắt tất cả collider
        DisableAllColliders();

        // Dừng di chuyển vật lý
        StopPhysicsMovement();

        Debug.Log("Enemy died!");
        animator.SetBool("IsDead", true);

        // Cộng tiền cho người chơi
        GameManager.Instance.AddMoney(moneyDrop);

        // Hủy enemy sau 1.5 giây
        Destroy(gameObject, 0.75f);
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
        enemyBehaviour.enabled = true; // Kích hoạt lại AI
    

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
