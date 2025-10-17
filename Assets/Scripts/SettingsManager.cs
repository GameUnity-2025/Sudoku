using UnityEngine;
using UnityEngine.UI; // Rất quan trọng, cần để dùng Slider
using UnityEngine.Audio; // Có thể cần nếu dùng Audio Mixer

public class SettingsManager : MonoBehaviour
{
    // Kéo Audio Source chứa nhạc nền vào đây trong Inspector
    [SerializeField] private AudioSource musicAudioSource;

    // Kéo Slider Âm lượng Nhạc vào đây trong Inspector
    [SerializeField] private Slider musicVolumeSlider;

    // Hàm này được gọi khi game bắt đầu
    void Start()
    {
        // Khi game vừa mở, tải cài đặt âm lượng đã lưu và áp dụng
        LoadVolumeSettings();
    }

    // Hàm này sẽ được gọi MỖI KHI người chơi kéo thanh trượt
    public void OnMusicSliderChanged()
    {
        // Lấy giá trị từ slider (từ 0.0 đến 1.0)
        float volume = musicVolumeSlider.value;

        // Gán giá trị đó cho âm lượng của Audio Source
        musicAudioSource.volume = volume;

        // Lưu giá trị này lại để lần sau mở game vẫn còn
        PlayerPrefs.SetFloat("MusicVolume", volume);
        Debug.Log("Đã lưu âm lượng nhạc: " + volume);
    }

    // Hàm để tải cài đặt đã lưu
    private void LoadVolumeSettings()
    {
        // Tải giá trị đã lưu trong PlayerPrefs. Nếu chưa từng lưu, giá trị mặc định sẽ là 1 (to nhất)
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);

        // Đặt giá trị cho slider
        musicVolumeSlider.value = savedVolume;

        // Đồng thời, đặt luôn âm lượng cho nhạc
        musicAudioSource.volume = savedVolume;
    }
}