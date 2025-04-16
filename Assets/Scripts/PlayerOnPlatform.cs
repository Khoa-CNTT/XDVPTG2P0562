using UnityEngine;

public class PlayerOnPlatform : MonoBehaviour
{
    private Transform currentPlatform;
    private Vector3 lastPlatformPosition;

    void Update()
    {
        if (currentPlatform != null)
        {
            // Tính toán chuyển động tương đối
            Vector3 platformMovement = currentPlatform.position - lastPlatformPosition;
            transform.position += platformMovement;
            lastPlatformPosition = currentPlatform.position;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            currentPlatform = collision.transform;
            lastPlatformPosition = collision.transform.position;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            currentPlatform = null;
        }
    }
}