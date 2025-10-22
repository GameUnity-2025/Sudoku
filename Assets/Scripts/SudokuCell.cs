using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SudokuCell : MonoBehaviour
{
    private Board board;
    private int row;
    private int col;
    private int value;
    private string id;

    [Header("UI References")]
    public Text t;
    public GameObject notesContainer;
    public GameObject notePrefab;

    private List<Text> noteTexts = new List<Text>();

    private Image cellImage;
    private Color originalColor;
    private Color errorColor = new Color(1f, 0.7f, 0.7f);
    private bool isError = false;

    private void Awake()
    {
        cellImage = GetComponent<Image>();

        if (cellImage != null)
        {
            originalColor = Color.white;     // Màu trắng làm nền mặc định
            cellImage.color = originalColor; // Áp dụng ngay
        }

        if (notesContainer != null)
        {
            for (int i = 0; i < 9; i++)
            {
                GameObject noteObj = Instantiate(notePrefab, notesContainer.transform);
                Text noteText = noteObj.GetComponent<Text>();
                noteText.text = "";
                noteTexts.Add(noteText);
            }
            notesContainer.SetActive(false);
        }
    }

    public void SetValues(int _row, int _col, int _value, string _id, Board _board)
    {
        row = _row;
        col = _col;
        id = _id;
        board = _board;

        if (cellImage == null) cellImage = GetComponent<Image>();

        // Luôn đặt màu trắng cho tất cả các ô
        originalColor = Color.white;
        cellImage.color = originalColor;

        if (_value != 0)
        {
            value = _value;
            t.text = value.ToString();
            t.color = Color.black;
            notesContainer.SetActive(false);

            // Cho ô cố định (có số sẵn) vẫn có thể click highlight nhưng không đổi số
            GetComponent<Button>().interactable = true;
        }
        else
        {
            value = 0;
            t.text = "";
            t.color = new Color32(0, 102, 187, 255); // Màu chữ ô nhập
        }
    }

    public void ButtonClicked()
    {
        InputButton.instance.ActivateInputButton(this);
    }

    public void UpdateValue(int newValue)
    {
        if (NoteButton.isNoteMode && value == 0)
        {
            ToggleNoteNumber(newValue);
            return;
        }

        ClearAllNotes();
        value = newValue;

        if (value != 0)
        {
            t.text = value.ToString();
            t.color = Color.black;
            notesContainer.SetActive(false);
        }
        else
        {
            t.text = "";
        }

        board.UpdatePuzzle(row, col, value);
    }

    private void ToggleNoteNumber(int number)
    {
        if (number < 1 || number > 9) return;

        notesContainer.SetActive(true);
        Text noteText = noteTexts[number - 1];

        if (noteText.text == number.ToString())
        {
            noteText.text = "";
        }
        else
        {
            noteText.text = number.ToString();
        }
    }

    public void ClearAllNotes()
    {
        if (notesContainer != null)
        {
            foreach (Text note in noteTexts)
                note.text = "";
            notesContainer.SetActive(false);
        }
    }

    public int GetValue() => value;
    public int GetRow() => row;
    public int GetCol() => col;

    public void HighlightError()
    {
        if (cellImage != null)
        {
            cellImage.color = errorColor;
            isError = true;
        }
    }

    public void ClearError()
    {
        if (cellImage != null)
        {
            cellImage.color = originalColor;
            isError = false;
        }
    }

    // ==== Compatibility wrapper methods for Board.cs ====
    public void ToggleNote(int number)
    {
        ToggleNoteNumber(number);
    }

    public void SetValue(int newValue)
    {
        UpdateValue(newValue);
    }

    public void OnCellClicked()
    {
        board.SetSelectedCell(this);

        if (cellImage != null)
        {
            cellImage.color = new Color(0.8f, 0.9f, 1f); // xanh nhạt khi chọn
            Invoke(nameof(ResetHighlight), 0.3f);
        }
    }

    private void ResetHighlight()
    {
        if (!isError && cellImage != null)
            cellImage.color = originalColor;
    }
}
