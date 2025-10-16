using UnityEngine;

public static class PlayerSettings
{
    const string DIFFICULTY_KEY = "sudoku_difficulty";

    // Default removals (or difficulty metric) - can be overwritten by UI
    public static int difficulty = 15;

    static PlayerSettings()
    {
        // Load saved difficulty if present
        if (PlayerPrefs.HasKey(DIFFICULTY_KEY))
        {
            difficulty = PlayerPrefs.GetInt(DIFFICULTY_KEY);
        }
    }

    public static void SetDifficulty(int d)
    {
        difficulty = d;
        PlayerPrefs.SetInt(DIFFICULTY_KEY, d);
        PlayerPrefs.Save();
    }
}
