using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    // Vị trí respawn (có thể đặt trong Inspector)
    public Vector2 respawnPosition;

    // Độ cao tối thiểu, nếu nhân vật rơi xuống dưới độ cao này sẽ respawn
    public float fallThreshold = -10f;

    // Tham chiếu đến Rigidbody2D của nhân vật (nếu có)
    private Rigidbody2D rb;

    void Start()
    {
        // Lấy component Rigidbody2D (nếu nhân vật có Rigidbody2D)
        rb = GetComponent<Rigidbody2D>();

        // Đọc vị trí checkpoint từ PlayerPrefs (nếu có)
        if (PlayerPrefs.HasKey("CheckpointX") && PlayerPrefs.HasKey("CheckpointY"))
        {
            float checkpointX = PlayerPrefs.GetFloat("CheckpointX");
            float checkpointY = PlayerPrefs.GetFloat("CheckpointY");
            respawnPosition = new Vector2(checkpointX, checkpointY);
        }
        else
        {
            // Đặt vị trí respawn mặc định là vị trí ban đầu của nhân vật
            respawnPosition = transform.position;
        }

        // Đặt lại vị trí của nhân vật khi scene được tải lại
        transform.position = respawnPosition;
    }

    void Update()
    {
        // Kiểm tra nếu nhân vật rơi xuống dưới fallThreshold
        if (transform.position.y < fallThreshold)
        {
            Respawn();
        }
    }

    // Hàm respawn nhân vật
    public void Respawn()
    {
        // Đặt lại vị trí của nhân vật
        transform.position = respawnPosition;

        // Nếu nhân vật có Rigidbody2D, đặt lại vận tốc để tránh lỗi vật lý
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        Debug.Log("Player respawned at: " + respawnPosition);
    }

    // Hàm này có thể được gọi từ script khác để đặt vị trí respawn mới
    public void SetRespawnPosition(Vector2 newPosition)
    {
        respawnPosition = newPosition;
        Debug.Log("Respawn position set to: " + respawnPosition);
    }
}