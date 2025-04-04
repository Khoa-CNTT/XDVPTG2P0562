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
    public float comboTimeWindow = 0.5f; // Thời gian cửa sổ combo

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
        // Reset combo nếu quá thời gian cửa sổ combo
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
             // Reset combo step sau đòn tấn công thứ hai
        }else if (comboStep == 3)
        {
            StartCoroutine(stopMovement());
            StartCoroutine(PerformAttack("Attack"));
            nextAttackTime = Time.time + 1f / attackRate;
            comboStep = 1; // Reset combo step sau đòn tấn công thứ hai
        }
    }

    IEnumerator PerformAttack(string attackTrigger)
    {
        isAttacking = true; // Bắt đầu tấn công
        animator.SetTrigger(attackTrigger); // Kích hoạt animation

        // Chờ animation hoàn thành
        float animationLength = GetAnimationLength(attackTrigger);
        yield return new WaitForSeconds(animationLength);

        isAttacking = false; // Kết thúc tấn công
    }

    IEnumerator stopMovement(){
        playerController.canMove = false;
        yield return playerController.LockMovementDuringAttack();
        playerController.canMove = true;

    }
    

    // Hàm lấy độ dài của animation
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
        return 0f; // Trả về 0 nếu không tìm thấy animation
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

    
}