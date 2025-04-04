using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{

    public Slider healthBarSlider;
    public TextMeshProUGUI healthBarValueText;

    public int maxHealth;
    public int currentHealth;
    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        healthBarValueText.text = currentHealth.ToString() + "/" + maxHealth.ToString();

        healthBarSlider.value = currentHealth;
        healthBarSlider.maxValue = maxHealth;
    }



    
}
