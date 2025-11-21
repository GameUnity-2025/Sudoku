using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Board : MonoBehaviour
{
    // Create the intital Sudoku Grid
    int[,] grid = new int[9,9];
    int[,] puzzle = new int[9, 9];
    private bool gameOver = false;

    // default numbers removed
    [SerializeField]
    int difficulty = 15;

    public Transform square00, square01, square02, 
                     square10, square11, square12, 
                     square20, square21, square22;
    public GameObject SudokuCell_Prefab;
    public GameObject winMenu;
    [SerializeField] GameObject loseText;

    // Store all cells for error checking
    private SudokuCell[,] allCells = new SudokuCell[9, 9];
    private bool autoCheckErrors = true; // Can be toggled by ShowMistakesButton
    [Header("Hover Highlight Settings")]
    [SerializeField] private Color hoverLineColor = new Color(0.92f, 0.96f, 1f);
    [SerializeField] private Color hoverCenterColor = new Color(0.8f, 0.88f, 1f);
    private readonly List<SudokuCell> hoverHighlightedCells = new List<SudokuCell>();
    private SudokuCell currentHoveredCell;


    // === Added for Note Mode ===
    private bool isNoteMode = false; // true = đang ghi chú
    private SudokuCell selectedCell = null;
    // ============================

    // Start is called before the first frame update
    void Start()
    {
        winMenu.SetActive(false);
        // If a difficulty was selected via UI (PlayerSettings), use it. Otherwise use the inspector/default.
        if (PlayerSettings.difficulty > 0)
        {
            difficulty = PlayerSettings.difficulty;
        }

        // If the player explicitly selected a new difficulty recently, force a new game
        if (PlayerSettings.forceNewGame)
        {
            Debug.Log("Board: forceNewGame is true — creating new puzzle for difficulty " + difficulty);
            CreateGrid();
            CreatePuzzle();
            PlayerSettings.forceNewGame = false; // reset flag
            // Save initial state immediately
            SaveSystem.SaveBoard(grid, puzzle, difficulty);
        }
        else
        {
            // Try to load saved game first
            var save = SaveSystem.LoadBoard();
            if (save != null)
            {
                Debug.Log("Board: Loading saved game.");
                // restore grid and puzzle
                for (int i = 0, idx = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++, idx++)
                    {
                        grid[i, j] = save.gridFlat[idx];
                        puzzle[i, j] = save.puzzleFlat[idx];
                    }
                }
                difficulty = save.difficulty;
            }
            else
            {
                CreateGrid();
                CreatePuzzle();
            }
        }

        CreateButtons();
    }

    // === Added for Note Mode ===
   public void ToggleNoteMode()
{
    isNoteMode = !isNoteMode;
    NoteButton.isNoteMode = isNoteMode; // đồng bộ với nút
    Debug.Log("Note Mode: " + (isNoteMode ? "ON" : "OFF"));
}


    public bool IsNoteMode()
    {
        return isNoteMode;
    }

    public void SetSelectedCell(SudokuCell cell)
    {
        selectedCell = cell;
    }

    public SudokuCell GetSelectedCell()
    {
        return selectedCell;
    }

    public void InputNumber(int number)
    {
        if (selectedCell == null) return;

        if (isNoteMode)
        {
            selectedCell.ToggleNote(number);
        }
        else
        {
            selectedCell.SetValue(number);
            UpdatePuzzle(selectedCell.GetRow(), selectedCell.GetCol(), number);
        }
    }
    // ============================


    // Update is called once per frame
    void Update()
    {
        
    }

    void ConsoleOutputGrid(int [,] g)
    {
        string output = "";
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                output += g[i, j];
            }
            output += "\n";
        }
        //Debug.Log(output);
    }

    bool ColumnContainsValue(int col, int value)
    {
        for (int i = 0; i < 9; i++)
        {
            if (grid[i, col] == value)
            {
                return true;
            }
        }

        return false;
    }

    bool RowContainsValue(int row, int value)
    {
        for (int i = 0; i < 9; i++)
        {
            if (grid[row, i] == value)
            {
                return true;
            }
        }

        return false;
    }

    bool SquareContainsValue(int row, int col, int value)
    {
        //blocks are 0-2, 3-5, 6-8
        //row / 3 is the first grid coord * 3 
        //ints 

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (grid[ row / 3 * 3 + i , col / 3 * 3 + j ] == value)
                {
                    return true;
                }
            }
        }

        return false;
    }

    bool CheckAll(int row, int col, int value)
    {
        if (ColumnContainsValue(col,value)) {
            //Debug.Log(row + " " + col);
            return false;
        }
        if (RowContainsValue(row, value))
        {
            //Debug.Log(row + " " + col);
            return false;
        }
        if (SquareContainsValue(row, col, value))
        {
            //Debug.Log(row + " " + col);
            return false;
        }

        return true;
    }

    bool IsValid()
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (grid[i,j] == 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    void CreateGrid()
    {
        List<int> rowList = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        List<int> colList = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        int value = rowList[Random.Range(0, rowList.Count)];
        grid[0, 0] = value;
        rowList.Remove(value);
        colList.Remove(value);

        for (int i = 1; i < 9; i++)
        {
            value = rowList[Random.Range(0, rowList.Count)];
            grid[i, 0] = value;
            rowList.Remove(value);
        }

        for (int i = 1; i < 9; i++)
        {
            value = colList[Random.Range(0, colList.Count)];
            if (i < 3)
            {
                while(SquareContainsValue(0, 0, value))
                {
                    value = colList[Random.Range(0, colList.Count)]; // reroll
                }
            }
            grid[0, i] = value;
            colList.Remove(value);
        }

        for (int i = 6; i < 9; i++)
        {
            value = Random.Range(1, 10);
            while (SquareContainsValue(0, 8, value) || SquareContainsValue(8, 0, value) || SquareContainsValue(8, 8, value))
            {
                value = Random.Range(1, 10);
            }
            grid[i, i] = value;
        }

        ConsoleOutputGrid(grid);

        SolveSudoku();
    }

    bool SolveSudoku()
    {
        int row = 0;
        int col = 0;

        if (IsValid())
        {
            return true;
        }

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (grid[i, j] == 0)
                {
                    row = i;
                    col = j;
                }
            }
        }

        for (int i = 1; i <=9; i++)
        {
            if (CheckAll(row, col, i)) {
                grid[row, col] = i;
                //ConsoleOutputGrid(grid);
                
                if (SolveSudoku())
                {
                    return true;
                }
                else
                {
                    grid[row, col] = 0;
                }
            }
        }
        return false;
    }

    void CreatePuzzle()
    {
        System.Array.Copy(grid, puzzle, grid.Length);

        // Remove cells
        for (int i = 0; i < difficulty; i++)
        {
            int row = Random.Range(0, 9);
            int col = Random.Range(0, 9);

            while (puzzle[row,col] == 0)
            {
                row = Random.Range(0, 9);
                col = Random.Range(0, 9);
            }

            puzzle[row, col] = 0;
        }

        // Make sure every generated puzzle has at least 8 different numbers on it. This ensures there is only one solution to each puzzle.
        List<int> onBoard = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        RandomizeList(onBoard);

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                for (int k = 0; k < onBoard.Count - 1; k++)
                {
                    if (onBoard[k] == puzzle[i,j])
                    {
                        onBoard.RemoveAt(k);
                    }
                }
            }
        }

        while (onBoard.Count - 1 > 1)
        {
            int row = Random.Range(0, 9);
            int col = Random.Range(0, 9);

            if (grid[row,col] == onBoard[0])
            {
                puzzle[row, col] = grid[row, col];
                onBoard.RemoveAt(0);
            }

        }

        ConsoleOutputGrid(puzzle);

    }

    void RandomizeList(List<int> l)
    {
        //var count = l.Count;
        //var last = count - 1;
        for (var i = 0; i < l.Count - 1; i++)
        {
            int rand = Random.Range(i, l.Count);
            int temp = l[i];
            l[i] = l[rand];
            l[rand] = temp;
        }
    }

    void CreateButtons()
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                GameObject newButton = Instantiate(SudokuCell_Prefab);
                SudokuCell sudokuCell = newButton.GetComponent<SudokuCell>();
                sudokuCell.SetValues(i, j, puzzle[i, j], i + "," + j, this);
                newButton.name = i.ToString() + j.ToString();

                // Store cell reference for error checking
                allCells[i, j] = sudokuCell;

                if (i < 3)
                {
                    if (j < 3)
                    {
                        newButton.transform.SetParent(square00, false);
                    }
                    if (j > 2 && j < 6)
                    {
                        newButton.transform.SetParent(square01, false);
                    }
                    if (j >= 6)
                    {
                        newButton.transform.SetParent(square02, false);
                    }
                }

                if (i >= 3 && i < 6)
                {
                    if (j < 3)
                    {
                        newButton.transform.SetParent(square10, false);
                    }
                    if (j > 2 && j < 6)
                    {
                        newButton.transform.SetParent(square11, false);
                    }
                    if (j >= 6)
                    {
                        newButton.transform.SetParent(square12, false);
                    }
                }

                if (i >= 6)
                {
                    if (j < 3)
                    {
                        newButton.transform.SetParent(square20, false);
                    }
                    if (j > 2 && j < 6)
                    {
                        newButton.transform.SetParent(square21, false);
                    }
                    if (j >= 6)
                    {
                        newButton.transform.SetParent(square22, false);
                    }
                }
        
            }
        }
    }

 public void UpdatePuzzle(int row, int col, int value)
{
    if (gameOver) return;

    int oldValue = puzzle[row, col];
    puzzle[row, col] = value;

    // ✅ Kiểm tra sai ngay sau khi nhập
    if (value != 0 && grid[row, col] != 0 && value != grid[row, col] && oldValue != value)
    {
        GameStatsManager.instance.AddMistake();
        Debug.Log($"❌ Mistake! Nhập {value} nhưng đúng là {grid[row, col]}");

        if (GameStatsManager.instance.GetMistakeCount() >= GameStatsManager.instance.GetMaxMistakes())
        {
            gameOver = true;
            loseText.SetActive(true);
            GameStatsManager.instance.PauseGame();
            Debug.Log("🔴 Game Over Triggered From Board!");
            return;
        }
    }
    else
    {
        Debug.Log($"🟢 No mistake. oldValue={oldValue}, newValue={value}");
    }

    // ✅ Sau khi xử lý logic thì mới lưu và highlight
    SaveSystem.SaveBoard(grid, puzzle, difficulty);

    if (autoCheckErrors)
        CheckAndHighlightErrors();

    // ✅ Kiểm tra hoàn thành Sudoku
    if (CheckGrid())
    {
        gameOver = true;
        GameStatsManager.instance.CompleteGame();
        winMenu.SetActive(true);
    }
}





    // Enable or disable automatic error checking
    public void SetAutoCheckErrors(bool enabled)
    {
        autoCheckErrors = enabled;
        if (!enabled)
        {
            ClearAllErrors();
        }
        else
        {
            CheckAndHighlightErrors();
        }
    }

    // Check for errors and highlight conflicting cells
    public void CheckAndHighlightErrors()
    {
        // First, clear all error highlights
        ClearAllErrors();

        // Check each cell for conflicts
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (puzzle[i, j] != 0 && puzzle[i, j] != grid[i, j])
                {
                    // This cell has a wrong value, highlight it and all conflicting cells
                    allCells[i, j].HighlightError();
                    HighlightConflictingCells(i, j, puzzle[i, j]);
                }
            }
        }
    }

    // Highlight all cells in the same row and column that have conflicts
    private void HighlightConflictingCells(int row, int col, int value)
    {
        // Highlight cells in the same row with the same value
        for (int j = 0; j < 9; j++)
        {
            if (j != col && puzzle[row, j] == value)
            {
                allCells[row, j].HighlightError();
            }
        }

        // Highlight cells in the same column with the same value
        for (int i = 0; i < 9; i++)
        {
            if (i != row && puzzle[i, col] == value)
            {
                allCells[i, col].HighlightError();
            }
        }

        // Highlight cells in the same 3x3 square with the same value
        int squareRow = (row / 3) * 3;
        int squareCol = (col / 3) * 3;
        for (int i = squareRow; i < squareRow + 3; i++)
        {
            for (int j = squareCol; j < squareCol + 3; j++)
            {
                if ((i != row || j != col) && puzzle[i, j] == value)
                {
                    allCells[i, j].HighlightError();
                }
            }
        }
    }

    // Clear all error highlights
    public void ClearAllErrors()
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (allCells[i, j] != null)
                {
                    allCells[i, j].ClearError();
                }
            }
        }
    }

    public void CheckComplete()
    {
        if (CheckGrid())
        {
            winMenu.SetActive(true);
            gameOver = true;
        GameStatsManager.instance.CompleteGame(); // ✅ cộng điểm
        }
        else
        {
            loseText.SetActive(true);
            //Debug.Log("Loser");
            GameStatsManager.instance.AddMistake(); // ❌ thêm lỗi
        }
    }

    bool CheckGrid()
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j =  0; j < 9; j++)
            {
                if (puzzle[i,j] != grid[i,j])
                {
                    return false;
                }
            }
        }
        return true;
    }


    // === Added for number buttons ===
public void OnNumberButtonClicked(int number)
{
    InputNumber(number);
}
public int[,] GetSolutionGrid()
{
    return grid;
}

    public void HighlightRowAndColumn(int row, int col, SudokuCell centerCell)
    {
        if (centerCell == null)
        {
            return;
        }

        ClearHoverHighlight();
        currentHoveredCell = centerCell;

        for (int c = 0; c < 9; c++)
        {
            SudokuCell cell = allCells[row, c];
            if (cell == null)
            {
                continue;
            }

            bool isCenter = cell == centerCell;
            cell.ApplyHoverHighlight(hoverLineColor, hoverCenterColor, isCenter);
            if (!hoverHighlightedCells.Contains(cell))
            {
                hoverHighlightedCells.Add(cell);
            }
        }

        for (int r = 0; r < 9; r++)
        {
            SudokuCell cell = allCells[r, col];
            if (cell == null)
            {
                continue;
            }

            bool isCenter = cell == centerCell;
            cell.ApplyHoverHighlight(hoverLineColor, hoverCenterColor, isCenter);
            if (!hoverHighlightedCells.Contains(cell))
            {
                hoverHighlightedCells.Add(cell);
            }
        }
    }

    public void OnCellHoverExit(SudokuCell cell)
    {
        if (cell == null)
        {
            return;
        }

        if (currentHoveredCell == cell)
        {
            ClearHoverHighlight();
        }
    }

    private void ClearHoverHighlight()
    {
        if (hoverHighlightedCells.Count == 0)
        {
            currentHoveredCell = null;
            return;
        }

        foreach (SudokuCell cell in hoverHighlightedCells)
        {
            cell?.ClearHoverHighlight();
        }

        hoverHighlightedCells.Clear();
        currentHoveredCell = null;
    }
public bool RevealRandomHint()
    {
        List<(int r, int c)> emptyCells = new List<(int, int)>();

        // Tìm tất cả ô trống trong puzzle
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                if (puzzle[r, c] == 0)
                {
                    emptyCells.Add((r, c));
                }
            }
        }

        // Không có ô nào để gợi ý
        if (emptyCells.Count == 0)
        {
            return false;
        }

        // Chọn ngẫu nhiên 1 ô trống
        var chosen = emptyCells[Random.Range(0, emptyCells.Count)];

        int row = chosen.r;
        int col = chosen.c;

        int correctValue = grid[row, col]; // Lấy đáp án đúng từ grid

        // Cập nhật puzzle
        puzzle[row, col] = correctValue;

        // Cập nhật UI cell
        if (allCells[row, col] != null)
        {
            allCells[row, col].SetValue(correctValue);
        }

        // Lưu game lại để tránh mất dữ liệu
        SaveSystem.SaveBoard(grid, puzzle, difficulty);

        // Tự động check lỗi (nếu đang bật)
        if (autoCheckErrors)
        {
            CheckAndHighlightErrors();
        }

        return true;
    }



}
