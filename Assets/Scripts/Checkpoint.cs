using UnityEngine;
using UnityEngine.SceneManagement;

public class Bonfire : MonoBehaviour
{
    [Header("Bonfire Settings")]
    [SerializeField] private Transform respawnPoint; // Vị trí hồi sinh
    [SerializeField] private GameObject interactPrompt; // UI nhắc nhở tương tác
    [SerializeField] private ParticleSystem bonfireEffect; // Hiệu ứng lửa
    [SerializeField] private AudioClip bonfireSound; // Âm thanh khi kích hoạt

    private bool isActivated = false; // Kiểm tra Bonfire đã được kích hoạt chưa
    private bool isInteracting = false; // Kiểm tra người chơi đang tương tác
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // Ẩn UI nhắc nhở ban đầu
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isActivated)
        {
            // Hiển thị UI nhắc nhở tương tác
            if (interactPrompt != null)
                interactPrompt.SetActive(true);

            // Bắt đầu theo dõi tương tác
            isInteracting = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Ẩn UI nhắc nhở tương tác
            if (interactPrompt != null)
                interactPrompt.SetActive(false);

            // Dừng theo dõi tương tác
            isInteracting = false;
        }
    }

    private void Update()
    {
        // Kiểm tra nếu người chơi đang trong vùng tương tác và nhấn phím E
        if (isInteracting && !isActivated && Input.GetKeyDown(KeyCode.E))
        {
            // Lấy component HealthSystem và PlayerRespawn từ nhân vật
            HealthSystem healthSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthSystem>();
            PlayerRespawn playerRespawn = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerRespawn>();

            if (healthSystem == null || playerRespawn == null)
            {
                Debug.LogError("HealthSystem or PlayerRespawn component not found on player!");
                return;
            }

            // Kích hoạt Bonfire
            ActivateBonfire(healthSystem, playerRespawn);
        }
    }

    private void ActivateBonfire(HealthSystem healthSystem, PlayerRespawn playerRespawn)
    {
        isActivated = true;

        // Kích hoạt hiệu ứng lửa
        if (bonfireEffect != null)
            bonfireEffect.Play();

        // Phát âm thanh
        if (bonfireSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(bonfireSound);
        }

        // Đặt vị trí hồi sinh cho nhân vật
        playerRespawn.SetRespawnPosition(respawnPoint.position);
        Debug.Log("Respawn position set to: " + respawnPoint.position);

        // Lưu vị trí checkpoint vào PlayerPrefs
        PlayerPrefs.SetFloat("CheckpointX", respawnPoint.position.x);
        PlayerPrefs.SetFloat("CheckpointY", respawnPoint.position.y);
        PlayerPrefs.Save(); // Lưu dữ liệu ngay lập tức

        // Lưu vị trí camera vào PlayerPrefs
        SmoothCamera smoothCamera = Camera.main.GetComponent<SmoothCamera>();
        if (smoothCamera != null)
        {
            smoothCamera.SaveCameraPosition();
        }

        // Hồi máu cho nhân vật
        healthSystem.HealToFull();
        Debug.Log("Player healed to full health!");

        // Reset lại scene (tùy chọn)
        ResetScene();

        Debug.Log("Bonfire activated!");
    }

    // Hàm reset lại scene
    private void ResetScene()
    {
        // Lấy tên scene hiện tại
        string currentSceneName = SceneManager.GetActiveScene().name;
        // Tải lại scene
        SceneManager.LoadScene(currentSceneName);
    }
}