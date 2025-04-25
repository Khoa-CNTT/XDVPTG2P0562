using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("Thông tin cơ bản")]
    public string itemName;          // Tên hiển thị
    public Sprite icon;              // Icon trong inventory
    public bool isStackable = true;  // Có thể cộng dồn không
    public string description;       // Mô tả (tuỳ chọn)

    // ID có thể dùng chính itemName hoặc tạo GUID riêng nếu muốn
    public string ID => itemName;
}
