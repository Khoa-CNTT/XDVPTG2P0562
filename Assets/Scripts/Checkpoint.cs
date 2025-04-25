
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
        if (collision.CompareTag("Player") && !isActivated)
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
        if (isInteracting && !isActivated && Input.GetKeyDown(KeyCode.E))
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
        isActivated = true;

        if (bonfireEffect != null) bonfireEffect.Play();
        if (bonfireSound != null && audioSource != null) audioSource.PlayOneShot(bonfireSound);

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

        ResetScene();
    }

    private void ResetScene()
    {
        string scene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(scene);
    }
}
