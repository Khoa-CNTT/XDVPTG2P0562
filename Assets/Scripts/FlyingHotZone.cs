using UnityEngine;

public class FlyingHotZone : MonoBehaviour
{
    private FlyingEnemy enemyParent;

    private void Awake()
    {
        enemyParent = GetComponentInParent<FlyingEnemy>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            enemyParent.TriggerChase();
        }
    }

private void OnTriggerExit2D(Collider2D collider)
{
    if (collider.CompareTag("Player") && enemyParent.gameObject.activeInHierarchy)
    {
        enemyParent.ResetToPatrol();
    }
}
}
