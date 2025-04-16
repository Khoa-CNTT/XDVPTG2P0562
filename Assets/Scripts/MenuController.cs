using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject newGamePanel;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private GameObject loadGamePanel;
    [SerializeField] private Transform saveListContent;
    [SerializeField] private GameObject saveSlotPrefab;
    private List<SaveSlotUI> activeSlots = new List<SaveSlotUI>();


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

        // Tạo save mới và chuyển scene
        int saveId = SaveManager.Instance.CreateSave(playerName);
        if (saveId != -1)
        {
            PlayerPrefs.SetInt("CurrentSaveId", saveId);
            SceneManager.LoadScene("GameScene");
        }
    }

    public void LoadGame()
    {
        loadGamePanel.SetActive(true);
        RefreshSaveList();
    }


      public void RefreshSaveList()
    {
        // Xóa slot cũ an toàn
        foreach (var slot in activeSlots)
        {
            if (slot != null) Destroy(slot.gameObject);
        }
        activeSlots.Clear();

        // Tải danh sách mới
        var saves = SaveManager.Instance.GetAllSaves();
        
        foreach (var save in saves)
        {
            var slotObj = Instantiate(saveSlotPrefab, saveListContent);
            var slotUI = slotObj.GetComponent<SaveSlotUI>();
            
            if (slotUI == null)
            {
                Debug.LogError("Prefab thiếu component!", slotObj);
                continue;
            }

            slotUI.Initialize(save);
            activeSlots.Add(slotUI);
            
            Debug.Log($"Đã tạo slot cho: {save.PlayerName} (ID: {save.Id})");
        }
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