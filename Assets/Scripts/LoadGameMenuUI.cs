// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections.Generic;
// using TMPro;

// public class LoadGameMenuUI : MonoBehaviour
// {
//     public GameObject saveButtonPrefab;
//     public Transform saveListParent;

//     private void Start()
//     {
//         ShowLoadGameMenu();
//     }

//     void ShowLoadGameMenu()
//     {
//         var folders = SaveManager.Instance.GetAllSaveFolders();

//         foreach (var folder in folders)
//         {
//             string summary = SaveManager.Instance.GetSaveSummary(folder);
//             GameObject button = Instantiate(saveButtonPrefab, saveListParent);

//             button.GetComponentInChildren<TMP_Text>().text = Path.GetFileName(folder) + "\n" + summary;

//             button.GetComponent<Button>().onClick.AddListener(() =>
//             {
//                 SaveManager.Instance.LoadGame(folder, out var playerData, out var inventory, out var money);
//                 ApplyLoadedData(playerData, inventory, money);
//             });
//         }
//     }

//     void ApplyLoadedData(PlayerData data, List<InventoryItemData> inventory, int money)
//     {
//         // Đây là BƯỚC 5 nè!
//         var player = GameObject.FindWithTag("Player"); // hoặc tham chiếu trực tiếp
//         player.transform.position = new Vector2(data.PositionX, data.PositionY);
//         player.GetComponent<Health>().SetHP(data.CurrentHP);

//         InventoryManager.Instance.SetInventory(inventory);
//         MoneyManager.Instance.SetMoney(money);
//     }
// }
