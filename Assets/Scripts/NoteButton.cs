using UnityEngine;
using UnityEngine.UI;

public class NoteButton : MonoBehaviour
{
    public static bool isNoteMode = false;
    [SerializeField] private Image noteIcon;
    [SerializeField] private Color activeColor = Color.yellow;
    [SerializeField] private Color inactiveColor = Color.white;

    void Start()
    {
        UpdateIcon();
    }

    public void ToggleNoteMode()
    {
        isNoteMode = !isNoteMode;
        Debug.Log("Note mode: " + isNoteMode);
        UpdateIcon();
    }

    void UpdateIcon()
    {
        if (noteIcon != null)
            noteIcon.color = isNoteMode ? activeColor : inactiveColor;
    }

    public static bool IsNoteModeActive()
    {
        return isNoteMode;
    }
}
