using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Item Settings")]
    public ItemData itemToPickup;
    public int quantity = 1;

    [Header("Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private GameObject interactPrompt;

    [Header("Sound")]
    [SerializeField] private AudioClip pickupSound;

    private bool isPlayerInRange = false;
    private GameObject player;

    private void Start()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(interactKey))
        {
            TryPickup();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            player = collision.gameObject;

            if (interactPrompt != null)
                interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            player = null;

            if (interactPrompt != null)
                interactPrompt.SetActive(false);
        }
    }

    private void TryPickup()
    {
        if (itemToPickup == null || player == null) return;

        if (itemToPickup.itemName == "HealthPotion")
        {
            var potionSystem = player.GetComponent<HealthPotionSystem>();
            if (potionSystem != null)
            {
                int before = potionSystem.GetCurrentPotionCount();
                potionSystem.AddPotion(quantity);
                int after = potionSystem.GetCurrentPotionCount();

                if (after > before)
                {
                    PlayPickupSound();
                    Debug.Log($"🧪 Nhặt {quantity} bình máu! (Tổng: {after})");
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            InventoryManager.Instance?.Add(itemToPickup, quantity);
            PlayPickupSound();
            Debug.Log($"📦 Nhặt item: {itemToPickup.itemName} x{quantity}");
            Destroy(gameObject);
        }
    }

    private void PlayPickupSound()
    {
        if (pickupSound == null) return;

        // 👉 Tạo AudioSource tạm thời để phát sound
        GameObject audioObj = new GameObject("PickupSound");
        AudioSource source = audioObj.AddComponent<AudioSource>();
        source.clip = pickupSound;
        source.Play();

        Destroy(audioObj, pickupSound.length);
    }
}
