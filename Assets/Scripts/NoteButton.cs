using UnityEngine;
using UnityEngine.UI;
using TMPro; // Bắt buộc nếu bạn dùng TextMeshPro

public class NoteButton : MonoBehaviour
{
    public static bool isNoteMode = false;

    [Header("Icon hiển thị trạng thái")]
    [SerializeField] private Image noteIcon;
    [SerializeField] private Color activeColor = Color.yellow;   // Màu icon khi ON
    [SerializeField] private Color inactiveColor = Color.white;  // Màu icon khi OFF

    [Header("Text hiển thị ON/OFF")]
    [SerializeField] private TextMeshProUGUI statusText; // Kéo đối tượng Text vào đây trong Inspector
    [SerializeField] private Color activeTextColor = Color.green;   // Màu chữ khi ON
    [SerializeField] private Color inactiveTextColor = Color.red;   // Màu chữ khi OFF

    void Start()
    {
        UpdateUI();
    }

    public void ToggleNoteMode()
    {
        isNoteMode = !isNoteMode;
        Debug.Log("Note mode: " + isNoteMode);
        UpdateUI();
    }

    void UpdateUI()
    {
        UpdateIcon();
        UpdateStatusText();
    }

    void UpdateIcon()
    {
        if (noteIcon != null)
            noteIcon.color = isNoteMode ? activeColor : inactiveColor;
    }

    void UpdateStatusText()
    {
        if (statusText != null)
        {
            statusText.text = isNoteMode ? "ON" : "OFF";
            statusText.color = isNoteMode ? activeTextColor : inactiveTextColor;
        }
    }

    public static bool IsNoteModeActive() => isNoteMode;
}
