using UnityEngine;

public class MovingPlatformBasic : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector2 moveDirection = Vector2.right; // Hướng di chuyển
    public float moveDistance = 3f;               // Khoảng cách cần di chuyển
    public float moveSpeed = 2f;                   // Tốc độ di chuyển

    private Vector2 startPos;
    private Vector2 targetPos;
    private bool movingToTarget = true;
    private Rigidbody2D rb;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + moveDirection.normalized * moveDistance;
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (movingToTarget)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, targetPos, moveSpeed * Time.fixedDeltaTime));
            if (Vector2.Distance(rb.position, targetPos) < 0.05f)
                movingToTarget = false;
        }
        else
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, startPos, moveSpeed * Time.fixedDeltaTime));
            if (Vector2.Distance(rb.position, startPos) < 0.05f)
                movingToTarget = true;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }

}
