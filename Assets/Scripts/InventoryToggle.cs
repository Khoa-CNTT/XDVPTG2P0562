using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            bool isActive = inventoryUI.activeSelf;
            inventoryUI.SetActive(!isActive);

            if (!isActive)
            {
                inventoryUI.GetComponent<InventoryUI>().Refresh();
            }
        }
    }
}
