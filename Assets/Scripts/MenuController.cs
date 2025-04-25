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

    private List<GameObject> activeSlots = new List<GameObject>();

    private void Awake()
    {
        newGamePanel.SetActive(false);
        loadGamePanel.SetActive(false);
    }

    public void NewGame()
    {
        newGamePanel.SetActive(true);
        nameInputField.text = "";
    }

    public void ConfirmNewGame()
    {
        string playerName = nameInputField.text.Trim();
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogError("Tên người chơi không được để trống!");
            return;
        }

        string saveFolder = SaveManager.CreateNewSaveFolder(playerName);
        if (!string.IsNullOrEmpty(saveFolder))
        {
            PlayerPrefs.SetString("CurrentSaveFolder", saveFolder);
            SceneManager.LoadScene("GameScene");
        }
        else
        {
            Debug.LogError("Không thể tạo thư mục lưu mới!");
        }
    }

    public void LoadGame()
    {
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
        PlayerPrefs.SetString("CurrentSaveFolder", folderPath);
        SceneManager.LoadScene("GameScene");
    }

    public void CancelNewGame()
    {
        newGamePanel.SetActive(false);
    }

    public void CancelLoadGame()
    {
        loadGamePanel.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
