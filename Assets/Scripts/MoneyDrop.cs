using UnityEngine;
using UnityEngine.Events;

public class MoneyDrop : MonoBehaviour
{
    public int amount;
    public GameObject interactPrompt; // Kéo thả UI prompt vào đây
    public KeyCode interactKey = KeyCode.E;
    
    private bool isInRange = false;
    private bool isCollected = false;

    private void Update()
    {
        if (isInRange && !isCollected && Input.GetKeyDown(interactKey))
        {
            CollectMoney();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isCollected)
        {
            isInRange = true;
            if (interactPrompt != null)
                interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = false;
            if (interactPrompt != null)
                interactPrompt.SetActive(false);
        }
    }

    private void CollectMoney()
    {
        isCollected = true;
        GameManager.Instance.ReclaimDroppedMoney(amount);
        
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
            
        Destroy(gameObject);
    }
    private void OnDestroy()
{
    SpriteRenderer sprite = GetComponent<SpriteRenderer>();
    if (sprite != null)
    {
        sprite.enabled = false; // Ẩn SpriteRenderer khi object bị hủy
    }
}

}