using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameStatsManager : MonoBehaviour
{
    public static GameStatsManager instance;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI allTimeText;
    [SerializeField] private TextMeshProUGUI mistakesText;
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject inputRoot;

    [Header("Game Settings")]
    [SerializeField] private int maxMistakes = 3;
    [SerializeField] private string difficulty = "Easy";
    [SerializeField] private bool separateHighScoreByDifficulty = false;

    [Header("Scenes & Flow")]
    [SerializeField] private string playSceneName = "SudokuPlay";
    [SerializeField] private string winSceneName  = "WinScene"; // hoặc "WinScense" nếu bạn giữ tên cũ
    [SerializeField] private string menuSceneName = "MainMenu";
    [SerializeField] private float  winSceneDelaySeconds = 0.7f;

    // runtime
    private float playTime = 0f;
    private int mistakeCount = 0;
    private bool isPaused = false;
    private bool isGameFinished = false;

    private int currentScore = 0;
    private int highScore = 0;

    private void Awake()
    {
        if (instance == null) instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("Difficulty"))
            difficulty = PlayerPrefs.GetString("Difficulty");

        highScore = PlayerPrefs.GetInt(GetHighScoreKey(), 0);

        UpdateAllTimeUI();
        UpdateMistakesUI();
        UpdateCurrentScoreUI();

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

    // ===== scoring =====
    private int CalculateScore()
    {
        int baseScore;
        switch (difficulty)
        {
            case "Medium": baseScore = 2000; break;
            case "Hard":   baseScore = 3000; break;
            case "Easy":
            default:       baseScore = 1000; break;
        }
        const int maxTime = 600; // 10 minutes
        int timeBonus = Mathf.Max(0, (int)(maxTime - playTime));
        int mistakePenalty = mistakeCount * 1000;
        return Mathf.Max(0, baseScore + timeBonus - mistakePenalty);
    }

    private string GetHighScoreKey()
    {
        return separateHighScoreByDifficulty ? $"HighScore_{difficulty}" : "HighScore";
    }

    // ===== public hooks =====
    public void AddMistake()
    {
        if (isPaused || isGameFinished) return;
        mistakeCount++;
        UpdateMistakesUI();
        if (mistakeCount >= maxMistakes) GameOver();
    }

    public void CompleteGame()
    {
        if (isGameFinished) return;

        isGameFinished = true;
        isPaused = true;
        Time.timeScale = 0f;

        currentScore = CalculateScore();

        // --- first-play detection + new best ---
        string key = GetHighScoreKey();
        bool   hadBestBefore = PlayerPrefs.HasKey(key);   // lần đầu sẽ = false
        int    prevBest      = PlayerPrefs.GetInt(key, 0);
        bool   isNewBest     = !hadBestBefore || currentScore > prevBest;

        if (isNewBest)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt(key, highScore);
            PlayerPrefs.Save();
        }
        else
        {
            highScore = prevBest; // giữ best cũ
        }

        UpdateAllTimeUI();
        ShowFinalScore();

        // snapshot cho WinScene
        GameStats.Score      = currentScore;
        GameStats.HighScore  = highScore;
        GameStats.Mistakes   = mistakeCount;
        GameStats.PlayTime   = playTime;
        GameStats.Difficulty = difficulty;
        GameStats.IsWin      = true;
        GameStats.IsNewBest  = isNewBest;

        StartCoroutine(LoadWinSceneRealtime(winSceneDelaySeconds));
        Debug.Log($"🎉 Game Completed | Score: {currentScore} | High Score: {highScore} | FirstPlay={(!hadBestBefore)} | NewBest={isNewBest}");
    }

    private void GameOver()
    {
        isGameFinished = true;
        isPaused = true;
        Time.timeScale = 0f;

        ShowFinalScore();
        if (gameOverPanel) gameOverPanel.SetActive(true);

        GameStats.Score      = 0;      // hoặc currentScore, tùy bạn
        GameStats.HighScore  = PlayerPrefs.GetInt(GetHighScoreKey(), highScore);
        GameStats.Mistakes   = mistakeCount;
        GameStats.PlayTime   = playTime;
        GameStats.Difficulty = difficulty;
        GameStats.IsWin      = false;
        GameStats.IsNewBest  = false;

        StartCoroutine(LoadWinSceneRealtime(1f));
        Debug.Log("❌ Game Over! Too many mistakes.");
    }

    private IEnumerator LoadWinSceneRealtime(float delaySeconds)
    {
        yield return new WaitForSecondsRealtime(delaySeconds);
        Time.timeScale = 1f;
        SceneManager.LoadScene(winSceneName);
    }

    // ===== UI =====
    private void UpdateCurrentScoreUI()
    {
        if (currentScoreText) currentScoreText.text = $"Score: {currentScore}";
    }
    private void UpdateMistakesUI()
    {
        if (mistakesText) mistakesText.text = $"Mistakes: {mistakeCount}/{maxMistakes}";
    }
    private void UpdateAllTimeUI()
    {
        if (allTimeText) allTimeText.text = $"All Time: {highScore}";
    }
    private void ShowFinalScore()
    {
        if (finalScoreText) finalScoreText.text = $"Final Score: {currentScore}";
    }

    // ===== pause =====
    public void PauseGame()
    {
        if (isPaused || isGameFinished) return;
        isPaused = true; Time.timeScale = 0f;
        if (inputRoot) inputRoot.SetActive(false);
        if (pauseButton) pauseButton.SetActive(false);
    }
    public void ResumeGame()
    {
        if (!isPaused || isGameFinished) return;
        isPaused = false; Time.timeScale = 1f;
        if (inputRoot) inputRoot.SetActive(true);
        if (pauseButton) pauseButton.SetActive(true);
    }
    public void TogglePause() { if (isPaused) ResumeGame(); else PauseGame(); }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        var s = SceneManager.GetActiveScene();
        SceneManager.LoadScene(s.name);
    }
    public void GoToMenu()
    {
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(menuSceneName)) SceneManager.LoadScene(menuSceneName);
    }

    public bool CanInput() => !isPaused && !isGameFinished;
    public int  GetMistakeCount() => mistakeCount;
    public int  GetMaxMistakes()  => maxMistakes;
}

// Snapshot holder
public static class GameStats
{
    public static int    Score;
    public static int    HighScore;
    public static int    Mistakes;
    public static float  PlayTime;
    public static string Difficulty = "Easy";
    public static bool   IsWin;
    public static bool   IsNewBest;
}
