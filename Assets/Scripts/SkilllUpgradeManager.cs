using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillUpgradeManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject skillPanel;
    public Button upgradeHealthButton;
    public Button upgradeDamageButton;
    public Button completeUpgradeButton;
    public TextMeshProUGUI healthCostText;
    public TextMeshProUGUI damageCostText;

    [Header("Upgrade Settings")]
    public int baseHealthCost = 100;
    public int baseDamageCost = 100;

    private int healthLevel = 0;
    private int damageLevel = 0;

    private void Start()
    {
        LoadUpgradeLevels();
        UpdateCostTexts();

        upgradeHealthButton.onClick.AddListener(UpgradeHealth);
        upgradeDamageButton.onClick.AddListener(UpgradeDamage);
        completeUpgradeButton.onClick.AddListener(CompleteUpgrade);

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))  // Nhấn phím P để kiểm tra
        {
            CompleteUpgrade();
        }
    }


    public void OpenSkillPanel()
    {
        skillPanel.SetActive(true);
        UpdateCostTexts();
    }

    public void CloseSkillPanel()
    {
        skillPanel.SetActive(false);
    }

    void UpgradeHealth()
    {
        int cost = baseHealthCost + healthLevel * 100;
        if (GameManager.Instance.GetCurrentMoney() >= cost)
        {
            GameManager.Instance.AddMoney(-cost);
            healthLevel++;
            SaveUpgradeLevels();
            UpdateCostTexts();
        }
    }

    void UpgradeDamage()
    {
        int cost = baseDamageCost + damageLevel * 100;
        if (GameManager.Instance.GetCurrentMoney() >= cost)
        {
            GameManager.Instance.AddMoney(-cost);
            damageLevel++;
            SaveUpgradeLevels();
            UpdateCostTexts();
        }
    }
    void CompleteUpgrade()
    {
        Bonfire bonfire = FindObjectOfType<Bonfire>();
        if (bonfire != null)
        {
            bonfire.ActivateFromUI();
        }
        else
        {
            Debug.LogError("Không tìm thấy bonfire trong scene.");
        }
    }



    void UpdateCostTexts()
    {
        healthCostText.text = $"Upgrade HP ({baseHealthCost + healthLevel * 100}G)";
        damageCostText.text = $"Upgrade DMG ({baseDamageCost + damageLevel * 100}G)";
    }

    void SaveUpgradeLevels()
    {
        PlayerPrefs.SetInt("HP_Level", healthLevel);
        PlayerPrefs.SetInt("DMG_Level", damageLevel);
    }

    void LoadUpgradeLevels()
    {
        healthLevel = PlayerPrefs.GetInt("HP_Level", 0);
        damageLevel = PlayerPrefs.GetInt("DMG_Level", 0);
    }

    public int GetHealthLevel() => healthLevel;
    public int GetDamageLevel() => damageLevel;
}
