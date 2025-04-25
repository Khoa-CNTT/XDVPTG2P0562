using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public Transform slotContainer;

    private List<ItemSlotUI> slotList = new List<ItemSlotUI>();

    void Awake()
    {
        Instance = this;

        inventoryPanel.SetActive(false);

        // Tạo 20 ô inventory
        for (int i = 0; i < 20; i++)
        {
            GameObject obj = Instantiate(slotPrefab, slotContainer);
            ItemSlotUI slot = obj.GetComponent<ItemSlotUI>();
            slotList.Add(slot);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            Refresh();
        }
    }

    public void Refresh()
    {
        var items = InventoryManager.Instance.GetInventory();

        for (int i = 0; i < slotList.Count; i++)
        {
            if (i < items.Count)
            {
                slotList[i].SetItem(items[i].item, items[i].quantity);
            }
            else
            {
                slotList[i].Clear();
            }
        }
    }
}
