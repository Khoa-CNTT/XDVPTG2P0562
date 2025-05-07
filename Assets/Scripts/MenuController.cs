using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;

public class MenuController : MonoBehaviour
{
    [Header("New Game Settings")]
    [SerializeField] private GameObject newGamePanel;
    [SerializeField] private TMP_InputField nameInputField;

    [Header("Load Game Settings")]
    [SerializeField] private GameObject loadGamePanel;
    [SerializeField] private Transform saveListContent;
    [SerializeField] private GameObject saveSlotPrefab;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip ClickSound;

    private List<GameObject> activeSlots = new List<GameObject>();

    private void Awake()
    {
        newGamePanel.SetActive(false);
        loadGamePanel.SetActive(false);
    }

    public void NewGame()
    {
        PlayClickSound();
        newGamePanel.SetActive(true);
        nameInputField.text = "";
    }

    public void ConfirmNewGame()
    {
        PlayClickSound();
        string playerName = nameInputField.text.Trim();
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogError("Tên người chơi không được để trống!");
            return;
        }

        string saveFolder = SaveManager.CreateNewSaveFolder(playerName);
        if (!string.IsNullOrEmpty(saveFolder))
        {
            // 👉 Reset dữ liệu mặc định cho NewGame
            PlayerData playerData = new PlayerData
            {
                PositionX = 0f,
                PositionY = 0f,
                CurrentHP = 100,         // Full máu mặc định
                PotionCount = 3           // 3 bình máu
            };

            SaveManager.SaveGame(saveFolder, playerData, new List<InventoryItemData>(), 0); // inventory trống, tiền = 0

            PlayerPrefs.SetString("CurrentSaveFolder", saveFolder);
            PlayerPrefs.Save();

            SceneManager.LoadScene("GameScene");
        }
        else
        {
            Debug.LogError("Không thể tạo thư mục lưu mới!");
        }
    }

    public void LoadGame()
    {
        PlayClickSound();
        loadGamePanel.SetActive(true);
        RefreshSaveList();
    }

    public void RefreshSaveList()
    {
        foreach (var obj in activeSlots)
        {
            Destroy(obj);
        }
        activeSlots.Clear();

        var saves = SaveManager.GetAllSaveFolders();

        foreach (var path in saves)
        {
            GameObject slot = Instantiate(saveSlotPrefab, saveListContent);
            SaveSlotUI ui = slot.GetComponent<SaveSlotUI>();

            if (ui != null)
                ui.SetSaveFolder(path);

            Button btn = slot.GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(() => LoadSelectedGame(path));

            activeSlots.Add(slot);
        }
    }

    private void LoadSelectedGame(string folderPath)
    {
        PlayClickSound();
        PlayerPrefs.SetString("CurrentSaveFolder", folderPath);
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameScene");
    }

    public void CancelNewGame()
    {
        PlayClickSound();
        newGamePanel.SetActive(false);
    }

    public void CancelLoadGame()
    {
        PlayClickSound();
        loadGamePanel.SetActive(false);
    }

    public void ExitGame()
    {
        PlayClickSound();
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void PlayClickSound()
    {
        if (audioSource != null && ClickSound != null)
        {
            audioSource.PlayOneShot(ClickSound);
        }
    }
}
