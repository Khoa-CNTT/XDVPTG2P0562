using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Reset mọi dữ liệu runtime khi New Game hoặc cần reset game.
/// (Không ảnh hưởng đến Save trong Database trừ khi tự gọi Save lại)
/// </summary>
public static class ClearGameState
{
    public static void ResetGame()
    {
        // Reset Máu
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var health = player.GetComponent<HealthSystem>();
            if (health != null)
            {
                health.SetHP(health.maxHealth); // Full máu
            }

            var potion = player.GetComponent<HealthPotionSystem>();
            if (potion != null)
            {
                potion.SetPotionCount(3); // Reset 3 bình máu
            }

            var moneySystem = GameManager.Instance;
            if (moneySystem != null)
            {
                moneySystem.currentMoney = 0; // Reset tiền về 0
            }
        }

        // // Reset Inventory
        // if (InventoryManager.Instance != null)
        // {
        //     InventoryManager.Instance.ClearInventory();
        // }

        // // Reset Equipped Items nếu có
        // if (EquipManager.Instance != null)
        // {
        //     EquipManager.Instance.ClearAllEquippedItems();
        // }

        Debug.Log("🧹 Game State đã được Reset!");
    }
}
