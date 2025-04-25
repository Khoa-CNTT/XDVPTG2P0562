using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI quantityText;

    public void SetItem(ItemData item, int quantity)
    {
        icon.sprite = item.icon;
        icon.enabled = true;
        quantityText.text = quantity.ToString();

    }

    public void Clear()
    {
        icon.sprite = null;
        icon.enabled = false;
        quantityText.text = "";
    }
}
// Compare this snippet from ItemData.cs: