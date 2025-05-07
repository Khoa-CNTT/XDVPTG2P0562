using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    void Start()
    {
        string saveFolder = PlayerPrefs.GetString("CurrentSaveFolder", "");
        if (string.IsNullOrEmpty(saveFolder))
        {
            Debug.LogError("Không tìm thấy thư mục save!");
            return;
        }

        SaveManager.LoadGame(saveFolder, out PlayerData playerData, out var inventory, out int money);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Không tìm thấy Player!");
            return;
        }

        // Đặt vị trí
        player.transform.position = new Vector2(playerData.PositionX, playerData.PositionY);

        // Đặt máu
        var health = player.GetComponent<HealthSystem>();
        if (health != null)
            health.SetHP(playerData.CurrentHP);

        // Đặt bình máu
        if (player != null)
        {
            HealthPotionSystem potionSystem = player.GetComponent<HealthPotionSystem>();
            if (potionSystem != null)
            {
                potionSystem.SetPotionCount(playerData.PotionCount); // ⬅️ Gán đúng ở đây!
            }
        }

        // Load inventory
        // InventoryManager.Instance?.LoadFromSaveData(inventory);

        // // Load tiền
        // if (GameManager.Instance != null)
        //     GameManager.Instance.CurrentMoney = money;

        Debug.Log("✅ Dữ liệu game đã được load từ " + saveFolder);
    }

    public static void LoadGame(string saveFolder)
    {
        PlayerPrefs.SetString("CurrentSaveFolder", saveFolder);
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameScene");
    }
}
