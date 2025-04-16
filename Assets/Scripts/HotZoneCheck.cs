using UnityEngine;

public class HotZoneCheck : MonoBehaviour
{
    private Enemy_Behaviour enemyParent;
    private bool inRange;
    private Animator anim;

    private void Awake()
    {
        enemyParent = GetComponentInParent<Enemy_Behaviour>();
        anim = GetComponentInParent<Animator>();
    }

    private void Update(){
        if(inRange && !anim.GetCurrentAnimatorStateInfo(0).IsName("EnemyAttack")){
            enemyParent.Flip();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.CompareTag("Player")){
            inRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.gameObject.CompareTag("Player")){
            inRange = false;
            gameObject.SetActive(false);
            enemyParent.TriggerArea.SetActive(true);
            enemyParent.inRange = false;
            enemyParent.SelectTarget();
        }
    }
}
