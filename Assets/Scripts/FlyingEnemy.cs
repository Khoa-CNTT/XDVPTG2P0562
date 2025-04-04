using UnityEngine;
using System.Collections;

public class FlyingEnemy : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;
    public float idleTime = 2f;

    [Header("Chase Settings")]
    public float detectionRange = 5f;
    public float chaseSpeed = 4f;
    public Transform player;

    [Header("Attack Settings")]
    public float attackDistance = 1.5f; // Khoảng cách tấn công
    public float attackCooldown = 2f;   // Thời gian hồi chiêu
    public int attackDamage = 20;       // Sát thương đòn đánh
    
    private int currentPointIndex = 0;
    private bool isChasing = false;
    private bool isIdle = false;
    private bool isAttacking = false; // Đang tấn công không?
    private SpriteRenderer spriteRenderer;
    private Coroutine patrolRoutine;
    private Animator anim;
    private HealthSystem playerHealth; // Script quản lý máu của Player

    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (patrolPoints.Length < 2)
        {
            Debug.LogError("Không có đủ điểm tuần tra!");
            return;
        }

        patrolRoutine = StartCoroutine(PatrolRoutine());

        if (player != null)
        {
            playerHealth = player.GetComponent<HealthSystem>(); // Lấy script máu của Player
        }
    }

    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackDistance && !isAttacking)
            {
                StartCoroutine(AttackRoutine());
            }
            else if (distanceToPlayer <= detectionRange && !isAttacking)
            {
                TriggerChase();
            }
            else if (isChasing && distanceToPlayer > detectionRange)
            {
                ResetToPatrol();
            }
        }
    }

    IEnumerator PatrolRoutine()
    {
        while (true)
        {
            if (!isIdle)
            {
                Transform targetPoint = patrolPoints[currentPointIndex];
                anim.SetBool("IsMoving", true);
                UpdateDirection(targetPoint.position);

                while (Vector2.Distance(transform.position, targetPoint.position) > 0.1f)
                {
                    transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, patrolSpeed * Time.deltaTime);
                    yield return null;
                }

                anim.SetBool("IsMoving", false);
                isIdle = true;
                yield return new WaitForSeconds(idleTime);
                isIdle = false;

                currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
            }
            yield return null;
        }
    }

    public void TriggerChase()
    {
        if (!isChasing && !isAttacking)
        {
            isChasing = true;
            if (patrolRoutine != null) StopCoroutine(patrolRoutine);

            anim.SetBool("IsMoving", true);
            StartCoroutine(ChaseRoutine());
        }
    }

    IEnumerator ChaseRoutine()
    {
        while (isChasing && player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer > attackDistance) // Chỉ di chuyển khi chưa tới gần
            {
                UpdateDirection(player.position);
                transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
            }
            yield return null;
        }
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        isChasing = false;
        anim.SetTrigger("IsAttacking"); // Kích hoạt animation Attack

        yield return new WaitForSeconds(0.5f); // Chờ animation Attack bắt đầu trước khi gây sát thương
        DealDamage(); // Gây sát thương cho Player

        yield return new WaitForSeconds(attackCooldown - 0.5f); // Chờ hồi chiêu trừ thời gian attack anim

        isAttacking = false;
        TriggerChase(); // Tiếp tục đuổi theo nếu Player vẫn ở trong tầm
    }

    void DealDamage()
    {
        if (playerHealth != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer <= attackDistance)
            {
                playerHealth.TakeDamage(attackDamage);
                Debug.Log("Kẻ địch tấn công! Gây " + attackDamage + " sát thương.");
            }
        }
    }

    public void ResetToPatrol()
    {
        if (!gameObject.activeInHierarchy) return; // Kiểm tra nếu gameObject bị vô hiệu hóa

        if (isChasing)
        {
            isChasing = false;
            anim.SetBool("IsMoving", false);

            if (patrolRoutine != null) StopCoroutine(patrolRoutine);
            patrolRoutine = StartCoroutine(PatrolRoutine());
        }
    }


    void UpdateDirection(Vector3 targetPosition)
    {
        float direction = targetPosition.x - transform.position.x;
        spriteRenderer.flipX = direction < 0;
    }
}
