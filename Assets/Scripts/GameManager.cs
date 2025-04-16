using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [SerializeField] private int currentMoney;
    public int CurrentMoney => currentMoney;
    // Thêm sự kiện khi tiền được nhặt
        public UnityEvent onMoneyPickupStart;
    
    // Thêm các biến quản lý tiền rơi
    private Vector3? deathPosition = null;
    private int droppedMoney = 0;
    private GameObject moneyDropInstance = null;
    private bool isDroppingMoney = false; // Ngăn chặn gọi nhiều lần
    
    public UnityEvent<int> onMoneyChanged;
    public UnityEvent<int> onMoneyPickedUp;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadMoney();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        SaveMoney();
        onMoneyChanged?.Invoke(currentMoney);
    }

    // Hàm xử lý khi player chết
    public void OnPlayerDeath(Vector3 deathPos)
{
    // Nếu còn MoneyDrop cũ (chưa nhặt), hủy nó ngay lập tức
    if (moneyDropInstance != null)
    {
        Destroy(moneyDropInstance);
        moneyDropInstance = null;
        droppedMoney = 0; // Đặt lại số tiền rơi để tránh cộng dồn
        deathPosition = null;
    }

    // Nếu player có tiền, tạo túi tiền mới
    if (currentMoney > 0)
    {
        droppedMoney = currentMoney;
        deathPosition = deathPos;
        currentMoney = 0;
        CreateMoneyDrop();
        SaveMoney();
        onMoneyChanged?.Invoke(currentMoney);
    }
}



    

   private void CreateMoneyDrop()
{
    // Nếu đã tồn tại một MoneyDrop, không tạo thêm
    if (moneyDropInstance != null) return;

    if (deathPosition.HasValue && droppedMoney > 0)
    {
        GameObject moneyDropPrefab = Resources.Load<GameObject>("MoneyDrop");
        if (moneyDropPrefab != null)
        {
            moneyDropInstance = Instantiate(moneyDropPrefab, deathPosition.Value, Quaternion.identity);
            MoneyDrop dropScript = moneyDropInstance.GetComponent<MoneyDrop>();
            dropScript.amount = droppedMoney;
        }
    }
}


    // Hàm gọi khi player hồi sinh
    public void OnPlayerRespawn()
    {
        // Chỉ tạo lại nếu thực sự có tiền chưa nhặt
    if (droppedMoney > 0 && deathPosition.HasValue)
    {
        CreateMoneyDrop();
    }
    }

    public void ReclaimDroppedMoney(int amount)
{
    if (droppedMoney != amount) return; // Chỉ nhặt đúng số tiền rơi ra

    onMoneyPickupStart?.Invoke();
    currentMoney += amount;
    
    // Reset dữ liệu rơi tiền
    droppedMoney = 0;
    deathPosition = null;

    // Hủy vật phẩm tiền rơi cũ
    if (moneyDropInstance != null)
    {
        Destroy(moneyDropInstance);
        moneyDropInstance = null;
    }

    SaveMoney();
    onMoneyChanged?.Invoke(currentMoney);
    onMoneyPickedUp?.Invoke(amount);
}


    private void SaveMoney()
    {
        PlayerPrefs.SetInt("PlayerMoney", currentMoney);
    }

    private void LoadMoney()
    {
        currentMoney = PlayerPrefs.GetInt("PlayerMoney", 0);
    }

    public void ResetMoney()
    {
        currentMoney = 0;
        SaveMoney();
        onMoneyChanged?.Invoke(currentMoney);
    }

    public int GetCurrentMoney()
    {
        return currentMoney;
    }
}