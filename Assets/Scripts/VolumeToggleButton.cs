using UnityEngine;
using UnityEngine.UI;

public class VolumeToggleButton : MonoBehaviour
{
    public Sprite volumeOnIcon;   // icon loa bật
    public Sprite muteIcon;       // icon loa tắt
    public Image buttonImage;     // ảnh hiện tại của nút

    private void Start()
    {
        UpdateIcon();
    }

    public void ToggleVolume()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ToggleMute();
            UpdateIcon();
        }
    }

    private void UpdateIcon()
    {
        if (buttonImage != null && AudioManager.Instance != null)
        {
            bool isMuted = AudioManager.Instance.IsMuted();
            buttonImage.sprite = isMuted ? muteIcon : volumeOnIcon;
        }
    }
}