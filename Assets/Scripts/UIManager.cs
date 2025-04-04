using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI moneyText;

    private void Start()
    {
        UpdateMoneyDisplay(GameManager.Instance.CurrentMoney);
        GameManager.Instance.onMoneyChanged.AddListener(UpdateMoneyDisplay);
    }

    private void UpdateMoneyDisplay(int amount)
    {
        moneyText.text = $"$ {amount}";
    }
}