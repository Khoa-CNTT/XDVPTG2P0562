using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeMenuUI : MonoBehaviour
{
    public Button upgradeHealthButton;
    public Button upgradeDamageButton;
    public GameObject panel;
    public TextMeshProUGUI moneyText;

    private void Start()
    {
        panel.SetActive(false); // Ẩn panel khi bắt đầu

        upgradeHealthButton.onClick.AddListener(UpgradeHealth);
        upgradeDamageButton.onClick.AddListener(UpgradeDamage);
    }

    public void Show()
    {
        panel.SetActive(true);
        UpdateMoneyDisplay();
        Time.timeScale = 0f; // Dừng game nếu cần
    }

    public void Hide()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
    }

    void UpgradeHealth()
    {
        GameManager.Instance.TryUpgradeHealth(FindObjectOfType<HealthSystem>());
        UpdateMoneyDisplay();
    }

    void UpgradeDamage()
    {
        GameManager.Instance.TryUpgradeDamage(FindObjectOfType<PlayerAttack>());
        UpdateMoneyDisplay();
    }

    void UpdateMoneyDisplay()
    {
        moneyText.text = "Tiền: " + GameManager.Instance.GetCurrentMoney();
    }
}
