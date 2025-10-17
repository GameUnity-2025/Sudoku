using System.Collections.Generic;
using UnityEngine;

// Simple undo manager for cell value changes
public class UndoManager : MonoBehaviour
{
    public static UndoManager instance;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    struct CellAction
    {
        public SudokuCell cell;
        public int prevValue;
        public int newValue;
    }

    Stack<CellAction> history = new Stack<CellAction>();

    // Record the change and apply it to the cell
    public void RecordAndApply(SudokuCell cell, int newValue)
    {
        if (cell == null) return;
        CellAction a = new CellAction();
        a.cell = cell;
        a.prevValue = cell.GetValue();
        a.newValue = newValue;
        history.Push(a);

        Debug.Log($"UndoManager: Recorded action for cell (prev={a.prevValue} -> new={a.newValue}). History size={history.Count}");

        cell.UpdateValue(newValue);
    }

    // Undo the last action
    public void UndoLast()
    {
        if (history.Count == 0) return;
        CellAction a = history.Pop();
        Debug.Log($"UndoManager: Undoing action for cell (reverting {a.newValue} -> {a.prevValue}). History size={history.Count}");
        if (a.cell != null)
        {
            a.cell.UpdateValue(a.prevValue);
        }
    }

    // Optional: clear history
    public void ClearHistory()
    {
        history.Clear();
    }

    // Static helper: ensure an instance exists (create if missing) and record the action
    public static void EnsureAndRecord(SudokuCell cell, int newValue)
    {
        if (instance == null)
        {
            GameObject mgr = new GameObject("Managers");
            instance = mgr.AddComponent<UndoManager>();
            Debug.Log("UndoManager: Created Managers GameObject and UndoManager via EnsureAndRecord.");
        }
        instance.RecordAndApply(cell, newValue);
    }

    // Static helper to ensure instance and perform undo
    public static void EnsureAndUndo()
    {
        if (instance == null)
        {
            Debug.Log("UndoManager: No instance found when calling EnsureAndUndo(). Nothing to undo.");
            return;
        }
        instance.UndoLast();
    }

    // Quick test: allow undo via keyboard 'U' when running in Editor or builds
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("UndoManager: Keyboard Undo (U) pressed.");
            UndoLast();
        }
    }

    // Utility: bind a Button by GameObject name to call InputButton.ClickedUndo
    public void BindButtonByName(string goName)
    {
        var btn = GameObject.Find(goName)?.GetComponent<UnityEngine.UI.Button>();
        if (btn == null)
        {
            Debug.LogWarning($"UndoManager: BindButtonByName could not find Button named '{goName}'");
            return;
        }
        if (btn.GetComponent<UndoButtonBinder>() == null)
        {
            btn.onClick.AddListener(() => { if (InputButton.instance != null) InputButton.instance.ClickedUndo(); });
            btn.gameObject.AddComponent<UndoButtonBinder>();
            Debug.Log($"UndoManager: Bound Button '{goName}' to ClickedUndo via BindButtonByName.");
        }
    }
}
