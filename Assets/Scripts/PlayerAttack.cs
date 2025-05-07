using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public Animator animator;
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public float attackRange = 0.5f;
    public int attackDamage = 40;
    public float attackRate = 2f;
    public float comboTimeWindow = 0.5f;

    [Header("Audio")]
    [SerializeField] private AudioClip attackSFX;         // Âm thanh khi tấn công
    [SerializeField] private AudioSource audioSource;     // Nguồn phát âm thanh

    private float nextAttackTime = 0f;
    private bool isEnemyDead = false;
    private bool isAttacking = false;
    private int comboStep = 0;
    private float lastAttackTime;
    private PlayerController1 playerController;
    private HealthPotionSystem healthPotionSystem;

    private void Awake()
    {
        playerController = GetComponent<PlayerController1>();
        healthPotionSystem = GetComponent<HealthPotionSystem>();
        int dmgLevel = PlayerPrefs.GetInt("DMG_Level", 0);
        attackDamage += dmgLevel * 5;


        // Tự động lấy AudioSource nếu chưa gán
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0)
                && !playerController.IsDashing()
                && !isAttacking
                && !healthPotionSystem.IsUsingPotion())
            {
                HandleComboAttack();
            }
        }
    }

    public void HandleComboAttack()
    {
        if (Time.time - lastAttackTime > comboTimeWindow)
        {
            comboStep = 0;
        }

        lastAttackTime = Time.time;
        comboStep++;

        if (comboStep == 1)
        {
            StartCoroutine(stopMovement());
            StartCoroutine(PerformAttack("Attack"));
        }
        else if (comboStep == 2)
        {
            StartCoroutine(stopMovement());
            StartCoroutine(PerformAttack("Attack1"));
        }
        else if (comboStep == 3)
        {
            StartCoroutine(stopMovement());
            StartCoroutine(PerformAttack("Attack"));
            nextAttackTime = Time.time + 1f / attackRate;
            comboStep = 1;
        }
    }

    IEnumerator PerformAttack(string attackTrigger)
    {
        isAttacking = true;
        animator.SetTrigger(attackTrigger);

        // 🔊 Phát âm thanh khi bắt đầu đánh
        if (attackSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(attackSFX);
        }

        float animationLength = GetAnimationLength(attackTrigger);
        yield return new WaitForSeconds(animationLength);

        isAttacking = false;
    }

    IEnumerator stopMovement()
    {
        playerController.canMove = false;
        yield return playerController.LockMovementDuringAttack();
        playerController.canMove = true;
    }

    float GetAnimationLength(string animationName)
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        foreach (AnimationClip clip in ac.animationClips)
        {
            if (clip.name == animationName)
            {
                return clip.length;
            }
        }
        return 0f;
    }

    public void DealDamage()
    {
        if (isEnemyDead) return;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyStatus enemyStatus = enemy.GetComponent<EnemyStatus>();
            if (enemyStatus != null && !enemyStatus.IsDead())
            {
                enemyStatus.TakeDamage(attackDamage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }

    public bool IsAttacking()
    {
        return isAttacking || animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack");
    }
    // PlayerAttack.cs
    public void IncreaseDamage(int amount)
    {
        attackDamage += amount;  // Tăng damage
        Debug.Log("Attack damage increased by " + amount + ". New attack damage: " + attackDamage);
    }

}
