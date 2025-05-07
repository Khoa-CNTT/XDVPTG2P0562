using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Bonfire : MonoBehaviour
{
    [Header("Bonfire Settings")]
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private GameObject interactPrompt;
    [SerializeField] private ParticleSystem bonfireEffect;
    [SerializeField] private AudioClip bonfireSound;
    [SerializeField] private UpgradeMenuUI upgradeMenuUI; // Tham chiếu tới UI nâng cấp

    private bool isActivated = false;
    private bool isInteracting = false;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (interactPrompt != null) interactPrompt.SetActive(true);
            isInteracting = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (interactPrompt != null) interactPrompt.SetActive(false);
            isInteracting = false;
        }
    }

    private void Update()
    {
        if (isInteracting && Input.GetKeyDown(KeyCode.E))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            var healthSystem = player.GetComponent<HealthSystem>();
            var potionSystem = player.GetComponent<HealthPotionSystem>();
            var respawnSystem = player.GetComponent<PlayerRespawn>();

            if (healthSystem == null || potionSystem == null || respawnSystem == null)
            {
                Debug.LogError("Không tìm thấy HealthSystem, PotionSystem hoặc RespawnSystem!");
                return;
            }

            ActivateBonfire(player.transform.position, healthSystem, potionSystem, respawnSystem);
        }
    }

    private void ActivateBonfire(Vector2 playerPos, HealthSystem health, HealthPotionSystem potions, PlayerRespawn respawn)
    {
        FindObjectOfType<SkillUpgradeManager>()?.OpenSkillPanel();

        if (isActivated) return;
        isActivated = true;

        // Hiệu ứng
        bonfireEffect?.Play();
        if (bonfireSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(bonfireSound);
        }

        // Gán điểm hồi sinh
        respawn.SetRespawnPosition(respawnPoint.position);

        // Hồi máu đầy
        health.HealToFull();

        // Lưu game
        string saveFolder = PlayerPrefs.GetString("CurrentSaveFolder", "");
        if (!string.IsNullOrEmpty(saveFolder))
        {
            PlayerData playerData = new PlayerData
            {
                PositionX = playerPos.x,
                PositionY = playerPos.y,
                CurrentHP = health.currentHealth,
                PotionCount = potions.GetCurrentPotionCount()
            };

            List<InventoryItemData> inventory = InventoryManager.Instance?.GetAllAsSaveData() ?? new();
            int money = GameManager.Instance?.CurrentMoney ?? 0;

            SaveManager.SaveGame(saveFolder, playerData, inventory, money);
            Debug.Log($"💾 Game saved at bonfire → {saveFolder}");
        }



        // Mở UI nâng cấp nếu có
        if (upgradeMenuUI != null)
        {
            upgradeMenuUI.Show();
        }
        else
        {
            Debug.LogWarning("⚠ Không có UI nâng cấp được gán! Reset scene luôn.");
            ResetScene(); // fallback
        }
    }
    public void ActivateFromUI()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        var healthSystem = player.GetComponent<HealthSystem>();
        var potionSystem = player.GetComponent<HealthPotionSystem>();
        var respawnSystem = player.GetComponent<PlayerRespawn>();

        if (healthSystem == null || potionSystem == null || respawnSystem == null)
        {
            Debug.LogError("Không tìm thấy HealthSystem, PotionSystem hoặc RespawnSystem!");
            return;
        }

        ActivateBonfire(player.transform.position, healthSystem, potionSystem, respawnSystem);
    }


    // Gọi thủ công từ UI khi người chơi thoát menu nâng cấp
    private void ResetScene()
    {
        string scene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(scene);
    }
}
