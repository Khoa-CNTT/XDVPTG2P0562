using Unity.VisualScripting;
using UnityEngine;

public class TriggerAreaCheck : MonoBehaviour
{
   private Enemy_Behaviour enemyParent;

    private void Awake()
    {
        enemyParent = GetComponentInParent<Enemy_Behaviour>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.CompareTag("Player")){
            gameObject.SetActive(false);
            enemyParent.target = collider.transform;
            enemyParent.inRange = true;
            enemyParent.HotZone.SetActive(true);
        }
    }
}
