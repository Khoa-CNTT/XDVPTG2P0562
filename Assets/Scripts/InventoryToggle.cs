using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryUI;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip toggleSound;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (audioSource != null && toggleSound != null)
            {
                audioSource.PlayOneShot(toggleSound);
            }
            bool isActive = inventoryUI.activeSelf;
            inventoryUI.SetActive(!isActive);

            if (!isActive)
            {
                inventoryUI.GetComponent<InventoryUI>().Refresh();
            }
        }
    }
}
