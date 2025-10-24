using UnityEngine;
using UnityEngine.SceneManagement;

public class QuickTestHotkeys : MonoBehaviour
{
    [Header("Scene names (đúng với project bạn)")]
    [SerializeField] string playScene = "SudokuPlay";
    [SerializeField] string winScene = "WinScene";

    void Update()
    {
        // F1: Thắng ngay (Complete)
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (GameStatsManager.instance != null)
                GameStatsManager.instance.CompleteGame();
            else
                SeedAndGotoWin(newScore: 7310, mistakes: 0, win: true, newBest: true);
        }

        // F2: Cộng 1 mistake
        if (Input.GetKeyDown(KeyCode.F2))
        {
            GameStatsManager.instance?.AddMistake();
        }

        // F3: Pause/Resume
        if (Input.GetKeyDown(KeyCode.F3))
        {
            GameStatsManager.instance?.TogglePause();
        }

        // F4: Reset HighScore (cho test “New Best”)
        if (Input.GetKeyDown(KeyCode.F4))
        {
            PlayerPrefs.DeleteKey("HighScore");
            // Nếu tách theo độ khó: PlayerPrefs.DeleteKey("HighScore_" + currentDifficulty);
            PlayerPrefs.Save();
            Debug.Log("🧹 Cleared HighScore.");
        }

        // F5: Vào thẳng WinScene với dữ liệu giả (không cần GameStatsManager)
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SeedAndGotoWin(newScore: 6000, mistakes: 1, win: true, newBest: false);
        }
    }

    void SeedAndGotoWin(int newScore, int mistakes, bool win, bool newBest)
    {
        GameStats.Score = newScore;
        GameStats.Mistakes = mistakes;
        GameStats.PlayTime = 78f;
        GameStats.Difficulty = "Easy";
        GameStats.HighScore = Mathf.Max(PlayerPrefs.GetInt("HighScore", 0), newScore);
        GameStats.IsWin = win;
        GameStats.IsNewBest = newBest;

        Time.timeScale = 1f;
        SceneManager.LoadScene(winScene);
    }
}
