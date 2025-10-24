using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

//
// GameStatsManager.cs
// Attach this to a GameObject in your gameplay scene (e.g., SudokuPlay).
//
public class GameStatsManager : MonoBehaviour
{
    public static GameStatsManager instance;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI allTimeText;      // High score (All Time)
    [SerializeField] private TextMeshProUGUI mistakesText;     // "Mistakes x/3"
    [SerializeField] private TextMeshProUGUI currentScoreText; // "Score: ####"
    [SerializeField] private TextMeshProUGUI finalScoreText;   // "Final Score: ####" (overlay in play scene, optional)
    [SerializeField] private GameObject pauseButton;           // Optional
    [SerializeField] private GameObject gameOverPanel;         // Overlay when losing (optional)
    [SerializeField] private GameObject inputRoot;             // Optional: parent of input UI to disable on pause

    [Header("Game Settings")]
    [SerializeField] private int maxMistakes = 3;
    [SerializeField] private string difficulty = "Easy";          // Loaded from PlayerPrefs if exists
    [SerializeField] private bool separateHighScoreByDifficulty = false;

    [Header("Scenes & Flow")]
    [SerializeField] private string playSceneName = "SudokuPlay"; // change to your gameplay scene name
    [SerializeField] private string winSceneName = "WinScene";   // result scene
    [SerializeField] private string menuSceneName = "MainMenu";   // optional main menu
    [SerializeField] private float winSceneDelaySeconds = 0.7f;   // small delay before switching scene

    // Runtime state
    private float playTime = 0f;
    private int mistakeCount = 0;
    private bool isPaused = false;
    private bool isGameFinished = false;

    private int currentScore = 0;
    private int highScore = 0;

    // ---------- Unity Lifecycle ----------
    private void Awake()
    {
        if (instance == null) instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        // Difficulty override from PlayerPrefs (if teammate set it elsewhere)
        if (PlayerPrefs.HasKey("Difficulty"))
            difficulty = PlayerPrefs.GetString("Difficulty");

        // Load HighScore
        highScore = PlayerPrefs.GetInt(GetHighScoreKey(), 0);

        // Init UI
        UpdateAllTimeUI();
        UpdateMistakesUI();
        UpdateCurrentScoreUI();

        // Ensure gameplay runs
        Time.timeScale = 1f;

        if (gameOverPanel) gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        if (!isPaused && !isGameFinished)
        {
            playTime += Time.deltaTime;
            currentScore = CalculateScore();
            UpdateCurrentScoreUI();
        }
    }

    // ---------- Scoring ----------
    private int CalculateScore()
    {
        // Base per difficulty
        int baseScore;
        switch (difficulty)
        {
            case "Medium": baseScore = 2000; break;
            case "Hard": baseScore = 3000; break;
            case "Easy":
            default: baseScore = 1000; break;
        }

        // Time bonus: counts down from 10 minutes
        const int maxTime = 600; // seconds
        int timeBonus = Mathf.Max(0, (int)(maxTime - playTime));

        // Penalty per mistake
        int mistakePenalty = mistakeCount * 1000;

        return Mathf.Max(0, baseScore + timeBonus - mistakePenalty);
    }

    private string GetHighScoreKey()
    {
        return separateHighScoreByDifficulty
            ? $"HighScore_{difficulty}"
            : "HighScore";
    }

    // ---------- Public Hooks ----------
    public void AddMistake()
    {
        if (isPaused || isGameFinished) return;

        mistakeCount++;
        UpdateMistakesUI();

        if (mistakeCount >= maxMistakes)
            GameOver();
    }

    public void CompleteGame()
    {
        if (isGameFinished) return;

        isGameFinished = true;
        isPaused = true;
        Time.timeScale = 0f;

        currentScore = CalculateScore();

        // High score update
        bool isNewBest = currentScore > highScore;
        if (isNewBest)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt(GetHighScoreKey(), highScore);
            PlayerPrefs.Save();
        }

        UpdateAllTimeUI();
        ShowFinalScore();

        // Mark snapshot for WinScene
        GameStats.Score = currentScore;
        GameStats.HighScore = highScore;
        GameStats.Mistakes = mistakeCount;
        GameStats.PlayTime = playTime;
        GameStats.Difficulty = difficulty;
        GameStats.IsWin = true;
        GameStats.IsNewBest = isNewBest;

        // Smooth handoff to WinScene using realtime delay (works even at timeScale=0)
        StartCoroutine(LoadWinSceneRealtime(winSceneDelaySeconds));
        Debug.Log($"🎉 Game Completed | Score: {currentScore} | High Score: {highScore}");
    }

    // ---------- Internal Flow ----------
    private void GameOver()
    {
        isGameFinished = true;
        isPaused = true;
        Time.timeScale = 0f;

        ShowFinalScore();
        if (gameOverPanel) gameOverPanel.SetActive(true);

        // Snapshot for losing case
        GameStats.Score = 0;          // or currentScore if you prefer
        GameStats.HighScore = highScore;
        GameStats.Mistakes = mistakeCount;
        GameStats.PlayTime = playTime;
        GameStats.Difficulty = difficulty;
        GameStats.IsWin = false;
        GameStats.IsNewBest = false;

        // Use realtime wait so it triggers while timeScale=0
        StartCoroutine(LoadWinSceneRealtime(1f));
        Debug.Log("❌ Game Over! Too many mistakes.");
    }

    private IEnumerator LoadWinSceneRealtime(float delaySeconds)
    {
        // A small realtime delay for a smoother transition / to show overlay
        yield return new WaitForSecondsRealtime(delaySeconds);
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(winSceneName))
            SceneManager.LoadScene(winSceneName);
        else
            Debug.LogError("Win scene name is empty. Please set 'winSceneName' in the inspector.");
    }

    // ---------- UI ----------
    private void UpdateCurrentScoreUI()
    {
        if (currentScoreText)
            currentScoreText.text = $"Score: {currentScore}";
    }

    private void UpdateMistakesUI()
    {
        if (mistakesText)
            mistakesText.text = $"Mistakes: {mistakeCount}/{maxMistakes}";
    }

    private void UpdateAllTimeUI()
    {
        if (allTimeText)
            allTimeText.text = $"All Time: {highScore}";
    }

    private void ShowFinalScore()
    {
        if (finalScoreText)
            finalScoreText.text = $"Final Score: {currentScore}";
    }

    // ---------- Pause / Resume ----------
    public void PauseGame()
    {
        if (isPaused || isGameFinished) return;

        isPaused = true;
        Time.timeScale = 0f;

        if (inputRoot) inputRoot.SetActive(false);
        if (pauseButton) pauseButton.SetActive(false);

        Debug.Log("⏸ Game Paused");
    }

    public void ResumeGame()
    {
        if (!isPaused || isGameFinished) return;

        isPaused = false;
        Time.timeScale = 1f;

        if (inputRoot) inputRoot.SetActive(true);
        if (pauseButton) pauseButton.SetActive(true);

        Debug.Log("▶️ Game Resumed");
    }

    public void TogglePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    // ---------- Scene Shortcuts (optional buttons) ----------
    public void RestartGame()
    {
        Time.timeScale = 1f;
        Scene thisScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(thisScene.name);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(menuSceneName))
            SceneManager.LoadScene(menuSceneName);
        else
            Debug.LogError("Menu scene name is empty. Please set 'menuSceneName' in the inspector.");
    }

    // ---------- Helpers (optional public) ----------
    public bool CanInput() => !isPaused && !isGameFinished;
    public int GetMistakeCount() => mistakeCount;
    public int GetMaxMistakes() => maxMistakes;
}

//
// A tiny snapshot holder so WinScene can read the result without needing references.
// Keep it in the same file for convenience, or move to its own GameStats.cs.
//
public static class GameStats
{
    public static int Score;
    public static int HighScore;
    public static int Mistakes;
    public static float PlayTime;
    public static string Difficulty = "Easy";
    public static bool IsWin;
    public static bool IsNewBest;
}
