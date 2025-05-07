using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class SaveSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Text UI")]
    [SerializeField] private TMP_Text folderNameText;
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text createdTimeText;

    [Header("Background UI")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = new Color(0.8f, 0.8f, 1f);
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip ClickSound;

    private string saveFolderPath;

    public void SetSaveFolder(string path)
    {
        saveFolderPath = path;

        // Hiển thị tên thư mục
        string folderName = Path.GetFileName(path);
        if (folderNameText != null)
            folderNameText.text = folderName;

        // Lấy thông tin từ database
        var info = SaveManager.GetSaveInfo(path);
        if (info.HasValue)
        {
            if (playerNameText != null)
                playerNameText.text = $"Name: {info.Value.playerName}";

            if (createdTimeText != null)
                createdTimeText.text = $"Created: {info.Value.createdAt:dd/MM/yyyy HH:mm}";
        }
        else
        {
            if (playerNameText != null)
                playerNameText.text = "Name: ???";

            if (createdTimeText != null)
                createdTimeText.text = "Created: none";
        }

        if (backgroundImage != null)
            backgroundImage.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (audioSource != null && ClickSound != null)
        {
            audioSource.PlayOneShot(ClickSound);
        }
        if (backgroundImage != null)
            backgroundImage.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (audioSource != null && ClickSound != null)
        {
            audioSource.PlayOneShot(ClickSound);
        }
        if (backgroundImage != null)
            backgroundImage.color = normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (string.IsNullOrEmpty(saveFolderPath)) return;
        if (audioSource != null && ClickSound != null)
        {
            audioSource.PlayOneShot(ClickSound);
        }

        Debug.Log($"📂 Loading game from: {saveFolderPath}");
        GameLoader.LoadGame(saveFolderPath);
    }

    public string GetSaveFolderPath()
    {
        return saveFolderPath;
    }
}
