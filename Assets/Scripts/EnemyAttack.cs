using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public Animator animator;
    public Transform attackPoint;
    public LayerMask playerLayers;
    public float attackRange = 0.5f;
    public int attackDamage = 40;
    public float attackCooldown = 2f;
    public float detectionRange = 3f;

    private float nextAttackTime = 0f;
    private bool isPlayerDead = false;

    void Update()
    {
        if (isPlayerDead) return;

        // Phát hiện player bằng OverlapCircle
        Collider2D player = Physics2D.OverlapCircle(
            transform.position, 
            detectionRange, 
            playerLayers
        );

        // Xử lý tấn công nếu đủ điều kiện
        if (player != null && Time.time >= nextAttackTime)
        {
            HealthSystem healthSystem = player.GetComponent<HealthSystem>(); // Sửa dòng 31
            if (healthSystem != null && !healthSystem.IsDead())
            {
                Attack();
                nextAttackTime = Time.time + attackCooldown;
            }
            else
            {
                isPlayerDead = true;
            }
        }
    }

    void Attack()
    {
        animator.SetTrigger("Attack");
    }

    // Hàm này gọi từ Animation Event
    public void DealDamage()
    {
        if (isPlayerDead) return;

        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(
            attackPoint.position, 
            attackRange, 
            playerLayers
        );

        foreach (Collider2D player in hitPlayers)
        {
            HealthSystem healthSystem = player.GetComponent<HealthSystem>(); // Sửa dòng 62
            if (healthSystem != null && !healthSystem.IsDead())
            {
                // Gọi hàm TakeDamage từ HealthSystem
                healthSystem.TakeDamage(attackDamage);
            }
            else
            {
                isPlayerDead = true;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}