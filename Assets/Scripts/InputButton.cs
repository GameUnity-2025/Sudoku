using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputButton : MonoBehaviour
{
    public static InputButton instance;
    SudokuCell lastCell;
    [SerializeField] GameObject wrongText;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void ActivateInputButton(SudokuCell cell)
    {
        this.gameObject.SetActive(true);
        lastCell= cell;
    }

    public void ClickedButton(int num)
    {
        // Record the change so it can be undone later. If UndoManager isn't present,
        // fall back to applying the value directly to avoid a NullReferenceException.
        if (lastCell != null)
        {
            Debug.Log($"InputButton: ClickedButton({num}) for lastCell.");
            // Use helper that ensures UndoManager exists and records
            UndoManager.EnsureAndRecord(lastCell, num);
        }

        if (wrongText != null)
            wrongText.SetActive(false);

        this.gameObject.SetActive(false);
    }

    // Called by the UI "undo/back" button
    public void ClickedUndo()
    {
        // Perform an undo of the last recorded action
        Debug.Log("InputButton: ClickedUndo()");
        UndoManager.EnsureAndUndo();
    }



}
