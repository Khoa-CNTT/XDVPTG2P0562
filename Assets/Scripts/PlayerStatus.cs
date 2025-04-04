using Unity.VisualScripting;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public Animator animator;
    public int maxHealth = 100;
    int currentHealth;
    private bool isDead = false;
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage){
        if (isDead)
            return;
        currentHealth -= damage;

        animator.SetTrigger("Hurt");

        Debug.Log("Player took " + damage + " damage!");

        if(currentHealth <= 0){
            Die();
        }
    }


    void Die(){
        Debug.Log("Player died!");
        isDead = true;
        animator.SetBool("IsDead", true);
        this.enabled = false;
        Destroy(gameObject, 1.5f);
    }

    public bool IsDead()
    {
        return isDead;
    }
    
}
