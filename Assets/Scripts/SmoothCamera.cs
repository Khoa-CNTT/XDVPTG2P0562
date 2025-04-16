using UnityEngine;

public class SmoothCamera : MonoBehaviour
{
    private Vector3 offset = new Vector3(0f, 0f, -10f);
    private float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private Transform target;

    void Start()
    {
        // Đọc vị trí camera từ PlayerPrefs (nếu có)
        if (PlayerPrefs.HasKey("CameraX") && PlayerPrefs.HasKey("CameraY") && PlayerPrefs.HasKey("CameraZ"))
        {
            float cameraX = PlayerPrefs.GetFloat("CameraX");
            float cameraY = PlayerPrefs.GetFloat("CameraY");
            float cameraZ = PlayerPrefs.GetFloat("CameraZ");
            transform.position = new Vector3(cameraX, cameraY, cameraZ);
        }
    }

    void Update()
    {
        // Kiểm tra nếu target bị null (đã bị hủy)
        if (target == null)
        {
            // Nếu target bị null, không làm gì cả hoặc có thể thêm logic xử lý khác
            Debug.LogWarning("Target is null. Camera will not follow.");
            return;
        }

        // Di chuyển camera mượt mà đến vị trí của target
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    // Hàm để lưu vị trí camera vào PlayerPrefs
    public void SaveCameraPosition()
    {
        PlayerPrefs.SetFloat("CameraX", transform.position.x);
        PlayerPrefs.SetFloat("CameraY", transform.position.y);
        PlayerPrefs.SetFloat("CameraZ", transform.position.z);
        PlayerPrefs.Save(); // Lưu dữ liệu ngay lập tức
        Debug.Log("Camera position saved: " + transform.position);
    }
}