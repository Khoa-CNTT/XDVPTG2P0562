using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public Inventory playerInventory = new Inventory();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Thêm item vào inventory
    /// </summary>
    public void Add(ItemData item, int quantity = 1)
    {
        if (item == null) return;

        playerInventory.AddItem(item, quantity);
        RefreshUI();
        Debug.Log($"✅ Added {quantity}x {item.name} to inventory.");
    }

    /// <summary>
    /// Xuất danh sách item đang có
    /// </summary>
    public List<ItemStack> GetInventory()
    {
        return playerInventory.items;
    }

    /// <summary>
    /// Xuất dữ liệu lưu trữ để ghi vào database
    /// </summary>
    public List<InventoryItemData> GetAllAsSaveData()
    {
        List<InventoryItemData> data = new List<InventoryItemData>();
        foreach (var stack in playerInventory.items)
        {
            data.Add(new InventoryItemData
            {
                ItemID = stack.item.name,
                Quantity = stack.quantity
            });
        }
        return data;
    }

    /// <summary>
    /// Cập nhật lại UI
    /// </summary>
    public void RefreshUI()
    {
        if (InventoryUI.Instance != null)
            InventoryUI.Instance.Refresh();
    }

}
