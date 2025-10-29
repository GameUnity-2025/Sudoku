using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MuteButton : MonoBehaviour
{
    [Header("Text hiển thị icon 🔊 / 🔇")]
    public TextMeshProUGUI buttonText;

    void Start()
    {
        if (buttonText == null)
        {
            Debug.LogError("⚠ Chưa gán TextMeshProUGUI cho MuteButton!");
            return;
        }

        UpdateButtonText();
    }

    public void ToggleSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ToggleMute();
            UpdateButtonText();
        }
        else
        {
            Debug.LogWarning("⚠ AudioManager.Instance chưa tồn tại!");
        }
    }

    private void UpdateButtonText()
    {
        if (AudioManager.Instance == null || buttonText == null) return;

        bool isMuted = AudioManager.Instance.IsMuted();
        buttonText.text = isMuted ? "🔇" : "🔊";
    }
}
