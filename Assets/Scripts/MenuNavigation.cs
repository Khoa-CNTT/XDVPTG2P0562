using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Data;
using Mono.Data.Sqlite;


public class MenuNavigation : MonoBehaviour
{
    public Button[] menuButtons; // Mảng chứa các nút menu
    private int currentIndex = 0; // Vị trí hiện tại của menu
    private Button currentButton; // Nút đang được chọn
    private GameObject currentBorder; // Viền sáng đang hiển thị

    void Start()
    {
        // Mặc định chọn nút đầu tiên
        currentButton = menuButtons[currentIndex];
        currentBorder = currentButton.transform.Find("SelectHover").gameObject;
        currentBorder.SetActive(true);
    }

    void Update()
    {
        HandleKeyboardNavigation();
        HandleMouseHover();

        // Nhấn E để kích hoạt nút
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentButton.onClick.Invoke();
        }
    }

    void HandleKeyboardNavigation()
    {
        int previousIndex = currentIndex;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentIndex = (currentIndex - 1 + menuButtons.Length) % menuButtons.Length;
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentIndex = (currentIndex + 1) % menuButtons.Length;
        }

        // Nếu có thay đổi, di chuyển border
        if (previousIndex != currentIndex)
        {
            MoveBorder(menuButtons[currentIndex]);
        }
    }

    void HandleMouseHover()
    {
        for (int i = 0; i < menuButtons.Length; i++)
        {
            if (EventSystem.current.currentSelectedGameObject == menuButtons[i].gameObject)
            {
                if (currentIndex != i)
                {
                    currentIndex = i;
                    MoveBorder(menuButtons[i]);
                }
                break;
            }
        }
    }

    void MoveBorder(Button newButton)
    {
        if (currentBorder != null)
        {
            currentBorder.SetActive(false);
        }

        currentButton = newButton;
        currentBorder = currentButton.transform.Find("SelectHover").gameObject;
        currentBorder.SetActive(true);
    }
    
}
