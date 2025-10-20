using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SudokuCell : MonoBehaviour
{
    Board board;

    int row;
    int col;
    int value;

    string id;

    public Text t;
    private Image cellImage;
    private Color originalColor;
    private Color errorColor = new Color(1f, 0.7f, 0.7f); // Light red for errors
    private bool isError = false;

    public void SetValues(int _row, int _col, int value, string _id, Board _board)
    {
        row = _row;
        col = _col; 
        id = _id;
        board = _board;

        // Get the Image component for highlighting
        cellImage = GetComponent<Image>();
        if (cellImage != null)
        {
            originalColor = cellImage.color;
        }

        Debug.Log(t.text);

        if (value != 0)
        {
            t.text = value.ToString();
        }
        else
        {
            t.text = " ";
        }

        if (value != 0)
        {
            GetComponentInParent<Button>().enabled = false;
        }
        else
        {
            t.color = new Color32(0, 102,187,255);
        }
    }

    public void ButtonClicked()
    {
        InputButton.instance.ActivateInputButton(this);
    }

    public void UpdateValue(int newValue)
    {
        value = newValue;

        if (value != 0)
        {
            t.text = value.ToString();
        }
        else
        {
            t.text = "";
        }
        board.UpdatePuzzle(row, col, value);
    }

    // Public getter for the current value so UndoManager can inspect previous state
    public int GetValue()
    {
        return value;
    }

    // Highlight this cell as an error
    public void HighlightError()
    {
        if (cellImage != null)
        {
            cellImage.color = errorColor;
            isError = true;
        }
    }

    // Clear error highlighting
    public void ClearError()
    {
        if (cellImage != null)
        {
            cellImage.color = originalColor;
            isError = false;
        }
    }

    // Get position info
    public int GetRow() { return row; }
    public int GetCol() { return col; }
}
