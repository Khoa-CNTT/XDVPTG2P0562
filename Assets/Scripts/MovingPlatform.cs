using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 2f;
    public float waitTime = 0.5f;

    private int currentIndex = 0;
    private float waitCounter = 0f;
    private bool isWaiting = false;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (waypoints.Length > 0) transform.position = waypoints[0].position;
    }

    void FixedUpdate()
    {
        if (waypoints.Length < 2) return;

        if (isWaiting)
        {
            waitCounter += Time.deltaTime;
            if (waitCounter >= waitTime) isWaiting = false;
            return;
        }

        Vector2 newPos = Vector2.MoveTowards(rb.position, waypoints[currentIndex].position, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        if (Vector2.Distance(rb.position, waypoints[currentIndex].position) < 0.01f)
        {
            currentIndex = (currentIndex + 1) % waypoints.Length;
            isWaiting = true;
            waitCounter = 0f;
        }
    }

    void OnDrawGizmos()
    {
        if (waypoints != null && waypoints.Length > 1)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < waypoints.Length - 1; i++)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                Gizmos.DrawSphere(waypoints[i].position, 0.1f);
            }
            Gizmos.DrawSphere(waypoints[waypoints.Length - 1].position, 0.1f);
        }
    }
}