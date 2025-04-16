using UnityEngine;
using TMPro;
using System;

public class SaveSlotUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text createdAtText;
    [SerializeField] private TMP_Text playTimeText;

    public void Initialize(SaveData save)
    {
        // Kiểm tra null
        if (playerNameText == null || createdAtText == null || playTimeText == null)
        {
            Debug.LogError("UI References chưa được gán!", this);
            return;
        }

        // Hiển thị thông tin
        playerNameText.text = string.IsNullOrEmpty(save.PlayerName) ? "No Name" : save.PlayerName;
        
        if (DateTime.TryParse(save.CreatedAt, out DateTime date))
        {
            createdAtText.text = date.ToLocalTime().ToString("HH:mm dd/MM/yyyy");
        }
        else
        {
            createdAtText.text = "Invalid Date";
        }

        TimeSpan time = TimeSpan.FromSeconds(save.PlayTime);
        playTimeText.text = $"{time.Hours:D2}h {time.Minutes:D2}m";
    }
}