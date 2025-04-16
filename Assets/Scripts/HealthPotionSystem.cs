using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HealthPotionSystem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int maxPotions = 3; // Số bình tối đa
    [SerializeField] private int healAmount = 50; // Lượng máu hồi mỗi bình
    [SerializeField] private KeyCode useKey = KeyCode.E; // Phím sử dụng bình máu

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI potionCountText; // Hiển thị số bình máu
    [SerializeField] private Image potionIconPrefab; // Icon bình máu
    [SerializeField] private Transform potionUIContainer; // Panel chứa icon bình máu

    [Header("Animation")]
    [SerializeField] private Animator animator; // Animator của Player

    private int currentPotions;
    private bool isUsingPotion = false;
    private HealthSystem healthSystem;
    private PlayerController1 playerController; // Tham chiếu đến script di chuyển
    private Rigidbody2D rb; // Tham chiếu đến Rigidbody2D

    private void Start()
    {
        healthSystem = GetComponent<HealthSystem>(); // Lấy tham chiếu HealthSystem
        playerController = GetComponent<PlayerController1>(); // Lấy tham chiếu PlayerController1
        rb = GetComponent<Rigidbody2D>(); // Lấy tham chiếu Rigidbody2D
        currentPotions = maxPotions;
        InitializePotionUI();
    }

    private void Update()
    {
        // Chỉ cho phép sử dụng bình máu nếu không đang tấn công
        if (Input.GetKeyDown(useKey)) // Kiểm tra nhấn phím E
        {
            UsePotion();
        }
    }

    // Khởi tạo UI bình máu
    private void InitializePotionUI()
    {
        if (potionUIContainer != null && potionIconPrefab != null)
        {
            for (int i = 0; i < maxPotions; i++)
            {
                Image icon = Instantiate(potionIconPrefab, potionUIContainer);
                icon.gameObject.SetActive(i < currentPotions); // Hiển thị số bình hiện có
            }
        }
        UpdatePotionUI();
    }

    // Sử dụng bình máu
    private void UsePotion()
    {
        if (CanUsePotion())
        {
            isUsingPotion = true;
            currentPotions--;
            UpdatePotionUI();

            // Dừng di chuyển ngay lập tức
            if (playerController != null)
            {
                playerController.canMove = false;
            }

            // Đặt vận tốc về 0 để dừng nhân vật
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            // Kích hoạt animation uống bình máu
            animator.SetBool("IsUsingPotion", true); // Sử dụng bool thay vì trigger

            // Hồi máu cho nhân vật
            healthSystem.TakeDamage(-healAmount); // Truyền số âm để hồi máu

            // Reset trạng thái sau khi sử dụng
            Invoke(nameof(ResetPotionUse), 1f); // Thời gian tùy chỉnh
        }
    }

    // Kiểm tra có thể sử dụng bình máu không
    private bool CanUsePotion()
    {
        // Kiểm tra các điều kiện:
        // 1. Còn bình máu
        // 2. Không đang sử dụng bình
        // 3. Máu chưa đầy
        // 4. Đang đứng trên mặt đất
        // 5. Không đang tấn công
        return currentPotions > 0 
            && !isUsingPotion 
            && healthSystem.currentHealth < healthSystem.maxHealth
            && playerController.IsGrounded() // Kiểm tra đang đứng trên mặt đất
            && !playerController.IsAttacking(); // Kiểm tra không đang tấn công
    }

    // Reset trạng thái sử dụng bình máu
    private void ResetPotionUse()
    {
        isUsingPotion = false;

        // Kết thúc animation
        animator.SetBool("IsUsingPotion", false);

        // Bật lại di chuyển
        if (playerController != null)
        {
            playerController.canMove = true;
        }
    }

    // Cập nhật UI bình máu
    private void UpdatePotionUI()
    {
        if (potionCountText != null)
        {
            potionCountText.text = $"{currentPotions}";
        }

        if (potionUIContainer != null)
        {
            for (int i = 0; i < potionUIContainer.childCount; i++)
            {
                potionUIContainer.GetChild(i).gameObject.SetActive(i < currentPotions);
            }
        }
    }

    // Thêm bình máu (có thể gọi từ script khác)
    public void AddPotion(int amount)
    {
        currentPotions = Mathf.Clamp(currentPotions + amount, 0, maxPotions);
        UpdatePotionUI();
    }
    public bool IsUsingPotion()
    {
        // Replace with actual logic to determine if a potion is being used
        return false;
    }
}