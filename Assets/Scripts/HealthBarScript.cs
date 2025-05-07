using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    [Header("UI References")]
    public Slider healthBarSlider;
    public TextMeshProUGUI healthBarValueText;

    [Header("Reference to HealthSystem")]
    [SerializeField] private HealthSystem healthSystem;

    private void Start()
    {
        if (healthSystem == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                healthSystem = player.GetComponent<HealthSystem>();
            }
        }

        if (healthSystem != null)
        {
            healthSystem.onHealthChanged.AddListener(UpdateHealthBar);
            UpdateHealthBar(); // Gọi lần đầu
        }
        else
        {
            Debug.LogError("HealthSystem not assigned to HealthBarScript!");
        }
    }

    private void UpdateHealthBar()
    {
        int current = healthSystem.currentHealth;
        int max = healthSystem.maxHealth;

        healthBarSlider.maxValue = max;
        healthBarSlider.value = current;
        healthBarValueText.text = $"{current}/{max}";
    }
}
