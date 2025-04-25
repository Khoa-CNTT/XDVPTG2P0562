using UnityEngine;
using System.Collections.Generic;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;
    public List<ItemData> allItems;
    public enum ItemType { Default, HealthPotion, Weapon, KeyItem }
    public ItemType itemType;

    private Dictionary<string, ItemData> lookup = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        foreach (var item in allItems)
        {
            lookup[item.name] = item; // Hoặc dùng item.ID nếu bạn thêm thuộc tính đó
        }
    }

    public ItemData GetItemByID(string id)
    {
        return lookup.TryGetValue(id, out var item) ? item : null;
    }
}
