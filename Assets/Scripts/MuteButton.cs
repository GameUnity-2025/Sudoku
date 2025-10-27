using TMPro;

using UnityEngine;

using UnityEngine.UI;

public class MuteButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText;


    void Start()
    {
        UpdateButtonText();
    }

    public void ToggleSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ToggleMute();
            UpdateButtonText();
        }
    }

    private void UpdateButtonText()
    {
        if (AudioManager.Instance != null)
        {
            bool isMuted = AudioManager.Instance.IsMuted();
            buttonText.text = isMuted ? "🔇" : "🔊";
        }
    }
}
