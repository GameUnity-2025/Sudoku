using UnityEngine;

public static class PlayerSettings
{
    const string DIFFICULTY_KEY = "sudoku_difficulty";

    // Default removals (or difficulty metric) - can be overwritten by UI
    public static int difficulty = 15;
    // When true, next time the Board initializes it should create a new puzzle instead of loading a save
    public static bool forceNewGame = false;

    static PlayerSettings()
    {
        // Load saved difficulty if present
        if (PlayerPrefs.HasKey(DIFFICULTY_KEY))
        {
            difficulty = PlayerPrefs.GetInt(DIFFICULTY_KEY);
            Debug.Log("PlayerSettings: Loaded difficulty from PlayerPrefs: " + difficulty);
        }
    }

    public static void SetDifficulty(int d)
    {
        difficulty = d;
        // mark that a new game should be created instead of loading an old save
        forceNewGame = true;
        PlayerPrefs.SetInt(DIFFICULTY_KEY, d);
        PlayerPrefs.Save();
        Debug.Log("PlayerSettings: SetDifficulty -> " + d);
    }
}
