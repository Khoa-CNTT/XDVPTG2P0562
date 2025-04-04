using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void NewGame()
    {
        SceneManager.LoadScene("GameScene"); // Đổi "GameScene" thành tên scene chính của bạn
    }

    public void LoadGame()
    {
        Debug.Log("Load Game Clicked"); 
        // Thêm logic load game từ dữ liệu lưu (PlayerPrefs, JSON, v.v.)
    }

    public void OpenSettings()
    {
        Debug.Log("Settings Clicked");
        // Chuyển đến Scene Settings hoặc mở Panel cài đặt
    }

    public void ExitGame()
    {
        Debug.Log("Exit Clicked");
        Application.Quit();
    }
}
