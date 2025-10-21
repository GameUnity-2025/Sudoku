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

        // 🟨 Bỏ qua ghi Undo nếu đang ở chế độ ghi chú
        if (NoteButton.IsNoteModeActive())
        {
            Debug.Log("UndoManager: Skipped recording (Note mode active).");
            cell.UpdateValue(newValue); // Vẫn cho phép cell xử lý ghi chú
            return;
        }

        CellAction a = new CellAction();
        a.cell = cell;
        a.prevValue = cell.GetValue();
        a.newValue = newValue;
        history.Push(a);

        Debug.Log($"UndoManager: Recorded action for cell (prev={a.prevValue} -> new={a.newValue}). History size={history.Count}");

        // 🟨 Chỉ áp dụng giá trị thật (không dùng cho note)
        cell.UpdateValue(newValue);
    }

    // Undo the last action
    public void UndoLast()
    {
        if (history.Count == 0)
        {
            Debug.Log("UndoManager: Nothing to undo.");
            return;
        }

        CellAction a = history.Pop();

        // 🟨 Kiểm tra cell hợp lệ
        if (a.cell == null)
        {
            Debug.LogWarning("UndoManager: Skipped undo because cell reference is null.");
            return;
        }

        Debug.Log($"UndoManager: Undoing action for cell (reverting {a.newValue} -> {a.prevValue}). History size={history.Count}");

        // 🟩 Gọi trực tiếp cell.UpdateValue để khôi phục
        a.cell.UpdateValue(a.prevValue);
    }

    // Optional: clear history
    public void ClearHistory()
    {
        history.Clear();
        Debug.Log("UndoManager: Cleared history.");
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

        // 🟨 Đảm bảo ghi hoặc bỏ qua đúng theo chế độ
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

        // 🟨 Giữ nguyên hành vi gốc, chỉ thêm check null
        if (btn.GetComponent<UndoButtonBinder>() == null)
        {
            btn.onClick.AddListener(() =>
            {
                if (InputButton.instance != null)
                    InputButton.instance.ClickedUndo();
                else
                    Debug.LogWarning("UndoManager: InputButton.instance is null while binding undo button.");
            });
            btn.gameObject.AddComponent<UndoButtonBinder>();
            Debug.Log($"UndoManager: Bound Button '{goName}' to ClickedUndo via BindButtonByName.");
        }
    }
}
