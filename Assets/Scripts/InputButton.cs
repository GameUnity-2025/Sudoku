using UnityEngine;
using UnityEngine.UI;

public class InputButton : MonoBehaviour
{
    public static InputButton instance;
    private SudokuCell lastCell;
    [SerializeField] private GameObject wrongText;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void ActivateInputButton(SudokuCell cell)
    {
        gameObject.SetActive(true);
        lastCell = cell;
    }

    public void ClickedButton(int num)
    {
        if (lastCell != null)
        {
            if (NoteButton.IsNoteModeActive())
            {
                // Không ghi Undo khi ở Note Mode
                lastCell.UpdateValue(num);
            }
            else
            {
                UndoManager.EnsureAndRecord(lastCell, num);
            }
        }

        if (wrongText != null)
            wrongText.SetActive(false);

        gameObject.SetActive(false);
    }

    public void ClickedUndo()
    {
        UndoManager.EnsureAndUndo();
    }
}
