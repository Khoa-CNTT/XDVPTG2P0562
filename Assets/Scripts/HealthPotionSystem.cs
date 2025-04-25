using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HealthPotionSystem : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip HealingSound;
    [Header("Settings")]
    [SerializeField] private int maxPotions = 3;
    [SerializeField] private int startingPotion;
    [SerializeField] private int healAmount = 50;
    [SerializeField] private KeyCode useKey = KeyCode.E;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI potionCountText; // Không phải prefab, đã gán object trong scene
    [SerializeField] private Transform potionUIContainer;     // Chứa các icon bình máu (đã có sẵn trong scene)

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private int currentPotions;
    private bool isUsingPotion = false;
    private HealthSystem healthSystem;
    private PlayerController1 playerController;
    private Rigidbody2D rb;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        playerController = GetComponent<PlayerController1>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        currentPotions = startingPotion;
        UpdatePotionUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(useKey))
        {
            UsePotion();
        }

        // // DEBUG: Nhấn P để thêm bình
        // if (Input.GetKeyDown(KeyCode.P))
        // {
        //     AddPotion(1);
        // }
    }

    private void UpdatePotionUI()
    {
        // Cập nhật số đếm text
        if (potionCountText != null)
        {
            potionCountText.gameObject.SetActive(true);
            potionCountText.text = currentPotions.ToString();
        }

        // Cập nhật icon hiển thị (giả sử có sẵn icon trong scene - không xoá/gán lại)
        if (potionUIContainer != null)
        {
            for (int i = 0; i < potionUIContainer.childCount; i++)
            {
                bool show = i < currentPotions;
                potionUIContainer.GetChild(i).gameObject.SetActive(show);
            }
        }
    }

    private bool CanUsePotion()
    {
        return currentPotions > 0 &&
               !isUsingPotion &&
               healthSystem.currentHealth < healthSystem.maxHealth &&
               playerController != null &&
               playerController.IsGrounded() &&
               !playerController.IsAttacking();
    }

    private void UsePotion()
    {
        if (!CanUsePotion()) return;

        isUsingPotion = true;
        currentPotions--;
        UpdatePotionUI();

        if (playerController != null) playerController.canMove = false;
        if (rb != null) rb.linearVelocity = Vector2.zero;

        animator.SetBool("IsUsingPotion", true);
        if (audioSource != null && HealingSound != null)
        {
            audioSource.PlayOneShot(HealingSound);
        }
        healthSystem.TakeDamage(-healAmount); // Hồi máu

        Invoke(nameof(ResetPotionUse), 1f);
    }

    private void ResetPotionUse()
    {
        isUsingPotion = false;
        animator.SetBool("IsUsingPotion", false);
        if (playerController != null) playerController.canMove = true;
    }

    public void AddPotion(int amount)
    {
        currentPotions = Mathf.Clamp(currentPotions + amount, 0, maxPotions);
        UpdatePotionUI();
    }

    public int GetCurrentPotionCount() => currentPotions;

    public void SetPotionCount(int count)
    {
        currentPotions = Mathf.Clamp(count, 0, maxPotions);
        UpdatePotionUI();
    }

    public bool IsUsingPotion() => isUsingPotion;
}
