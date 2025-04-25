// using UnityEngine;
// using System.Collections.Generic;

// public class AutoSaveManager : MonoBehaviour
// {
//     public Transform playerTransform;
//     public int currentHP = 100;
//     public float saveInterval = 30f;

//     private float timer;
//     private string gameID;

//     private void Start()
//     {
//         gameID = PlayerPrefs.GetString("CurrentGameID", "");
//         if (string.IsNullOrEmpty(gameID))
//         {
//             Debug.LogError("Không tìm thấy GameID hiện tại!");
//             enabled = false;
//             return;
//         }

//         // Load Player Position + HP
//         var data = SaveManager.Instance.LoadPlayerData(gameID);
//         if (data != null)
//         {
//             playerTransform.position = new Vector2(data.PositionX, data.PositionY);
//             currentHP = data.CurrentHP;
//         }

//         // Load Inventory
//         var loadedItems = SaveManager.Instance.LoadInventory(gameID);
//         InventoryManager.Instance.playerInventory.items.Clear();

//         foreach (var itemData in loadedItems)
//         {
//             var item = ItemDatabase.Instance.GetItemByID(itemData.ItemID);
//             if (item != null)
//             {
//                 InventoryManager.Instance.playerInventory.items.Add(new ItemStack(item, itemData.Quantity));
//             }
//         }

//         InventoryUI ui = FindObjectOfType<InventoryUI>();
//         if (ui != null) ui.Refresh();
//     }

//     private void Update()
//     {
//         timer += Time.deltaTime;
//         if (timer >= saveInterval)
//         {
//             AutoSave();
//             timer = 0f;
//         }
//     }

//     private void AutoSave()
//     {
//         Vector2 pos = playerTransform.position;

//         int currentMoney = 0; // Replace with the actual value or variable for money
//         SaveManager.Instance.SavePlayerData(gameID, pos, currentHP, currentMoney);

//         var inventory = InventoryManager.Instance.playerInventory.items;
//         var dataList = new List<InventoryItemData>();
//         foreach (var stack in inventory)
//         {
//             dataList.Add(new InventoryItemData
//             {
//                 ItemID = stack.item.name, // Hoặc .ID nếu có
//                 Quantity = stack.quantity
//             });
//         }

//         SaveManager.Instance.SaveInventory(gameID, dataList);

//         Debug.Log("✅ Auto-save hoàn tất.");
//     }

//     private void OnApplicationQuit() => AutoSave();
//     private void OnApplicationPause(bool pause) { if (pause) AutoSave(); }
// }
