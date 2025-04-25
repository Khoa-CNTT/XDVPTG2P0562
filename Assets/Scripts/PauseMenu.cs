using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseUI;
    public Button saveButton;
    public Button returnButton;
    private bool isPaused = false;

    void Start()
    {
        pauseUI?.SetActive(false);
        saveButton?.onClick.AddListener(SaveGame);
        returnButton?.onClick.AddListener(ReturnToTitle);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        pauseUI?.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }

    void SaveGame()
    {
        string saveFolder = PlayerPrefs.GetString("CurrentSaveFolder", "");
        if (string.IsNullOrEmpty(saveFolder))
        {
            Debug.LogWarning("❌ Không tìm thấy CurrentSaveFolder trong PlayerPrefs.");
            return;
        }

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        var health = player.GetComponent<HealthSystem>();
        var potions = player.GetComponent<HealthPotionSystem>();
        var pos = player.transform.position;

        var playerData = new PlayerData
        {
            PositionX = pos.x,
            PositionY = pos.y,
            CurrentHP = health != null ? health.currentHealth : 100,
            PotionCount = potions != null ? potions.GetCurrentPotionCount() : 0
        };

        List<InventoryItemData> inventory = InventoryManager.Instance != null
            ? InventoryManager.Instance.GetAllAsSaveData()
            : new List<InventoryItemData>();

        int money = GameManager.Instance != null ? GameManager.Instance.CurrentMoney : 0;

        SaveManager.SaveGame(saveFolder, playerData, inventory, money);

        Debug.Log("✅ Game đã được lưu vào " + saveFolder);
        TogglePause();
    }

    void ReturnToTitle()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("TitleMenu");
    }
}
