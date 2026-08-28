using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject StartPanel;
    public GameObject SettingsMenuPanel;
    public GameObject GameLogoPanel; // Thêm biến quản lý Logo

    // Hàm gọi khi bấm nút New Game
    public void StartGame()
    {
        if (GameLogoPanel != null) GameLogoPanel.SetActive(false); // Tắt logo khi vào game
        SceneManager.LoadScene("Map1");
    }

    // Hàm mở bảng Setting
    public void OpenSettings()
    {
        if (StartPanel != null) StartPanel.SetActive(false);
        if (SettingsMenuPanel != null) SettingsMenuPanel.SetActive(true);
        if (GameLogoPanel != null) GameLogoPanel.SetActive(false); // Tắt logo khi vào Setting
    }

    // Hàm đóng bảng Setting (Nút Back)
    public void CloseSettings()
    {
        if (SettingsMenuPanel != null) SettingsMenuPanel.SetActive(false);
        if (StartPanel != null) StartPanel.SetActive(true);
        if (GameLogoPanel != null) GameLogoPanel.SetActive(true); // Bật lại logo khi quay về màn hình chính
    }

    // Hàm thoát game
    public void QuitGame()
    {
        Debug.Log("Game is exiting...");
        Application.Quit();
    }
}