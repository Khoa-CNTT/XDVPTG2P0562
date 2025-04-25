using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public float damage = 10f;
    public float damageInterval = 0.5f;

    private bool isPlayerInTrap = false;
    private float timer = 0f;

    private void Update()
    {
        if (isPlayerInTrap)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                DealDamage();
                timer = damageInterval;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInTrap = true;
            timer = 0f; // Gây sát thương ngay lập tức
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInTrap = false;
        }
    }

    private void DealDamage()
    {
        // Giả sử player có component HealthSystem gắn trên GameObject
        HealthSystem health = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthSystem>();
        if (health != null)
        {
            health.TakeDamage((int)damage); // Ép kiểu float sang int
        }
    }
}
