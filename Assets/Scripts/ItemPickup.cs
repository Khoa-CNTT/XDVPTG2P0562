using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData itemToPickup;
    public int quantity = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || itemToPickup == null) return;

        if (itemToPickup.itemName == "HealthPotion")
        {
            var potionSystem = collision.GetComponent<HealthPotionSystem>();
            if (potionSystem != null)
            {
                potionSystem.AddPotion(quantity);
                Debug.Log($"🧪 Nhặt {quantity} bình máu! (Tổng: {potionSystem.GetCurrentPotionCount()})");

                Destroy(gameObject); // ✅ chỉ destroy nếu thêm thành công
            }
        }
        else
        {
            InventoryManager.Instance?.Add(itemToPickup, quantity);
            Debug.Log($"📦 Nhặt item: {itemToPickup.itemName} x{quantity}");
            Destroy(gameObject);
        }
    }


}
