using System;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int[] gridFlat; // 81 elements
    public int[] puzzleFlat; // 81 elements
    public int difficulty;
}

public static class SaveSystem
{
    private const string SAVE_KEY = "Sudoku_Save";

    public static void SaveBoard(int[,] grid, int[,] puzzle, int difficulty)
    {
        SaveData d = new SaveData();
        d.gridFlat = new int[81];
        d.puzzleFlat = new int[81];
        d.difficulty = difficulty;

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
        Debug.Log("SaveSystem: Saved board to PlayerPrefs.");
#endif
    }

    public static bool HasSave()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }

    public static SaveData LoadBoard()
    {
        if (!HasSave()) return null;
        try
        {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            SaveData d = JsonUtility.FromJson<SaveData>(json);
            return d;
        }
        catch (Exception e)
        {
            Debug.LogError("SaveSystem: Failed to load save - " + e.Message);
            return null;
        }
    }

    public static void ClearSave()
    {
        if (HasSave())
        {
            PlayerPrefs.DeleteKey(SAVE_KEY);
            PlayerPrefs.Save();
        }
    }
}
