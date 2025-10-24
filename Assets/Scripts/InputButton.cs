using UnityEngine;
using UnityEngine.UI;

public class InputButton : MonoBehaviour
{
    public static InputButton instance;

    private SudokuCell lastCell;
    private int[,] solutionGrid; // Lưới lời giải chính xác (copy từ Board)
    private Board board;         // Tham chiếu đến Board để cập nhật puzzle

    [SerializeField] private GameObject wrongText;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        gameObject.SetActive(false);

        // Tự tìm Board nếu chưa được set
        if (board == null)
            board = FindObjectOfType<Board>();

        // Lấy grid chuẩn (solution) để so sánh
        if (board != null)
            solutionGrid = board.GetSolutionGrid();
    }

    /// <summary>
    /// Kích hoạt khi chọn một ô Sudoku
    /// </summary>
    public void ActivateInputButton(SudokuCell cell)
    {
        // Nếu game đang pause hoặc đã xong thì không mở bàn phím
        if (!GameStatsManager.instance.CanInput()) return;

        gameObject.SetActive(true);
        lastCell = cell;
    }

    /// <summary>
    /// Khi bấm số
    /// </summary>
    public void ClickedButton(int num)
    {
        // ✅ Ngăn nhập khi game pause hoặc kết thúc
        if (!GameStatsManager.instance.CanInput()) return;
        if (lastCell == null) return;

        int row = lastCell.GetRow();
        int col = lastCell.GetCol();

        if (NoteButton.IsNoteModeActive())
        {
            lastCell.UpdateValue(num);
        }
        else
        {
            UndoManager.EnsureAndRecord(lastCell, num);

            // ✅ Kiểm tra đúng sai
            if (solutionGrid != null && solutionGrid[row, col] != num)
            {
                if (wrongText != null)
                    wrongText.SetActive(true);

                // ❗ Không cộng lỗi ở đây — Board sẽ xử lý
            }
            else
            {
                if (wrongText != null)
                    wrongText.SetActive(false);
            }

            // ✅ Gửi về Board để cập nhật và tính logic
            board?.UpdatePuzzle(row, col, num);
        }

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Khi bấm nút Undo
    /// </summary>
    public void ClickedUndo()
    {
        // ✅ Cũng không undo khi game đang pause
        if (!GameStatsManager.instance.CanInput()) return;

        UndoManager.EnsureAndUndo();
        if (wrongText != null)
            wrongText.SetActive(false);
    }

    /// <summary>
    /// Cho phép cập nhật grid chuẩn từ bên ngoài (Board gửi vào)
    /// </summary>
    public void SetSolutionGrid(int[,] grid)
    {
        solutionGrid = grid;
    }
}
