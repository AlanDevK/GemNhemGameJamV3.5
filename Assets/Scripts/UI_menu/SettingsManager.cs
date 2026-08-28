using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FMODUnity;

public class SettingsManager : MonoBehaviour
{
    [Header("UI References")]
    public Slider VolumeSlider;
    public Toggle FullscreenToggle;
    public TMP_Dropdown FpsDropdown;
    public TMP_Dropdown LanguageDropdown;

    [Header("FMOD Settings")]
    [ParamRef]
    public string MasterBusPath = "bus:/";
    private FMOD.Studio.Bus masterBus;

    void Start()
    {
        // Khởi tạo FMOD Bus nếu có dùng âm thanh
        //if (!string.IsNullOrEmpty(MasterBusPath))
        //{
          //  masterBus = RuntimeManager.GetBus(MasterBusPath);
        //}

        // Thiết lập giá trị mặc định cho UI
        if (FullscreenToggle != null)
            FullscreenToggle.isOn = Screen.fullScreen;
            
        if (FpsDropdown != null)
            FpsDropdown.value = Application.targetFrameRate == 120 ? 1 : 0;
    }

    // 1. Chỉnh âm lượng qua FMOD
    public void SetVolume(float volume)
    {
        if (masterBus.isValid())
        {
            masterBus.setVolume(volume);
        }
    }

    // 2. Bật/Tắt Fullscreen
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    // 3. Thay đổi FPS (60 hoặc 120)
    public void SetFPS(int index)
    {
        if (index == 0)
        {
            Application.targetFrameRate = 60;
        }
        else if (index == 1)
        {
            Application.targetFrameRate = 120;
        }
    }

    // 4. Thay đổi ngôn ngữ (Ví dụ tùy chọn: 0 - Tiếng Việt, 1 - English, 2 - Deutsch)
    public void SetLanguage(int index)
    {
        switch (index)
        {
            case 0:
                Debug.Log("Đã chuyển sang Tiếng Việt");
                break;
            case 1:
                Debug.Log("Switched to English");
                break;
            case 2:
                Debug.Log("Zu Deutsch gewechselt");
                break;
        }
    }
}