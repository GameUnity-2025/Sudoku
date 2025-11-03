using System;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int[] gridFlat;     // 81 phần tử - lưu toàn bộ trạng thái grid (có lời giải)
    public int[] puzzleFlat;   // 81 phần tử - lưu puzzle ban đầu (các ô trống)
    public int difficulty;     // mức độ khó
}

public static class SaveSystem
{
    private const string SAVE_KEY = "Sudoku_Save";

    // 🔹 Lưu dữ liệu Sudoku hiện tại
    public static void SaveBoard(int[,] grid, int[,] puzzle, int difficulty)
    {
        try
        {
            SaveData d = new SaveData
            {
                gridFlat = new int[81],
                puzzleFlat = new int[81],
                difficulty = difficulty
            };

            int idx = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    d.gridFlat[idx] = grid[i, j];
                    d.puzzleFlat[idx] = puzzle[i, j];
                    idx++;
                }
            }

            string json = JsonUtility.ToJson(d);
            PlayerPrefs.SetString(SAVE_KEY, json);
            PlayerPrefs.Save();

#if UNITY_EDITOR
            Debug.Log($"✅ SaveSystem: Saved board successfully (difficulty {difficulty}).");
#endif
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ SaveSystem: Failed to save board - {e.Message}");
        }
    }

    // 🔹 Kiểm tra xem có dữ liệu lưu hay chưa
    public static bool HasSave()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }

    // 🔹 Tải dữ liệu Sudoku đã lưu
    public static SaveData LoadBoard()
    {
        if (!HasSave())
        {
#if UNITY_EDITOR
            Debug.Log("⚠️ SaveSystem: No save found.");
#endif
            return null;
        }

        try
        {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            SaveData d = JsonUtility.FromJson<SaveData>(json);

            if (d == null || d.gridFlat == null || d.puzzleFlat == null)
            {
                Debug.LogWarning("⚠️ SaveSystem: Save data is incomplete or corrupted.");
                return null;
            }

#if UNITY_EDITOR
            Debug.Log("✅ SaveSystem: Loaded board successfully.");
#endif
            return d;
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ SaveSystem: Failed to load save - {e.Message}");
            return null;
        }
    }

    // 🔹 Xóa dữ liệu Sudoku (dùng khi NewGame / Restart)
    public static void ClearSave()
    {
        if (HasSave())
        {
            PlayerPrefs.DeleteKey(SAVE_KEY);
            PlayerPrefs.Save();
#if UNITY_EDITOR
            Debug.Log("🗑️ SaveSystem: Cleared Sudoku save data.");
#endif
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("ℹ️ SaveSystem: No save to clear.");
#endif
        }
    }
}
