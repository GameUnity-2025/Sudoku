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
        if (notesContainer != null)
        {
            // Tạo 9 note Text nhỏ cho container
            for (int i = 0; i < 9; i++)
            {
                GameObject noteObj = Instantiate(notePrefab, notesContainer.transform);
                Text noteText = noteObj.GetComponent<Text>();
                noteText.text = "";
                noteTexts.Add(noteText);
            }

            notesContainer.SetActive(false); // ẩn mặc định
        }
    }

    public void SetValues(int _row, int _col, int _value, string _id, Board _board)
    {
        row = _row;
        col = _col;
        id = _id;
        board = _board;

        cellImage = GetComponent<Image>();
        if (cellImage != null)
            originalColor = cellImage.color;

        if (_value != 0)
        {
            value = _value;
            t.text = value.ToString();
            notesContainer.SetActive(false);
            GetComponent<Button>().interactable = false;
        }
        else
        {
            value = 0;
            t.text = "";
            t.color = new Color32(0, 102, 187, 255);
        }
    }

    public void ButtonClicked()
    {
        if (NoteButton.isNoteMode && value == 0)
        {
            InputButton.instance.ActivateInputButton(this);
        }
        else
        {
            InputButton.instance.ActivateInputButton(this);
        }
    }

    public void UpdateValue(int newValue)
    {
        if (NoteButton.isNoteMode && value == 0)
        {
            ToggleNoteNumber(newValue);
            return;
        }

        // Nếu nhập giá trị thật thì xóa hết note
        ClearAllNotes();

        value = newValue;
        if (value != 0)
        {
            t.text = value.ToString();
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
    ToggleNoteNumber(number); // gọi đúng hàm bạn đã có
}

public void SetValue(int newValue)
{
    UpdateValue(newValue); // gọi đúng hàm UpdateValue gốc
}


// Khi người chơi click vào ô Sudoku
public void OnCellClicked()
{
    // Báo cho Board biết ô này đang được chọn
    board.SetSelectedCell(this);

    // (Tuỳ chọn) có thể highlight ô này để dễ thấy
    if (cellImage != null)
    {
        cellImage.color = new Color(0.8f, 0.9f, 1f); // xanh nhạt
        Invoke(nameof(ResetHighlight), 0.3f); // tự reset sau 0.3s
    }
}

private void ResetHighlight()
{
    if (!isError && cellImage != null)
        cellImage.color = originalColor;
}

}
