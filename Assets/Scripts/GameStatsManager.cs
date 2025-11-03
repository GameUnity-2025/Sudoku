using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI; // ✅ Thêm để dùng Image

public class GameStatsManager : MonoBehaviour
{
    public static GameStatsManager instance;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI allTimeText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI mistakesText;
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private GameObject pauseButton;          // Nút Pause (GameObject)
    [SerializeField] private Image pauseButtonImage;          // ✅ Image component để đổi icon
    [SerializeField] private Sprite pauseSprite;              // ✅ Ảnh pause.png
    [SerializeField] private Sprite playSprite;               // ✅ Ảnh playic.png
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject inputRoot;
    [SerializeField] private GameObject pausePanel;


    [Header("Game Settings")]
    [SerializeField] private int maxMistakes = 3;
    [SerializeField] private string difficulty = "Easy";
    [SerializeField] private bool separateHighScoreByDifficulty = false;

    [Header("Scenes & Flow")]
    [SerializeField] private string playSceneName = "SudokuPlay";
    [SerializeField] private string winSceneName = "WinScene";
    [SerializeField] private string menuSceneName = "MainMenu";
    [SerializeField] private float winSceneDelaySeconds = 0.7f;

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

        // ✅ Khởi tạo icon Pause
        if (pauseButtonImage == null && pauseButton != null)
        {
            pauseButtonImage = pauseButton.GetComponent<Image>();
        }
        if (pauseButtonImage != null && pauseSprite != null)
        {
            pauseButtonImage.sprite = pauseSprite; // Mặc định icon pause khi bắt đầu game
        }
    }

    private void Update()
    {
        if (!isPaused && !isGameFinished)
        {
            playTime += Time.deltaTime;
            currentScore = CalculateScore();
            UpdateCurrentScoreUI();
            UpdateTimerUI();

        }
    }

    // ===== scoring =====
    private int CalculateScore()
    {
        int baseScore;
        switch (difficulty)
        {
            case "Medium": baseScore = 2000; break;
            case "Hard": baseScore = 3000; break;
            case "Easy":
            default: baseScore = 1000; break;
        }
        const int maxTime = 600;
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

        string key = GetHighScoreKey();
        bool hadBestBefore = PlayerPrefs.HasKey(key);
        int prevBest = PlayerPrefs.GetInt(key, 0);
        bool isNewBest = !hadBestBefore || currentScore > prevBest;

        if (isNewBest)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt(key, highScore);
            PlayerPrefs.Save();
        }
        else
        {
            highScore = prevBest;
        }

        UpdateAllTimeUI();
        ShowFinalScore();

        GameStats.Score = currentScore;
        GameStats.HighScore = highScore;
        GameStats.Mistakes = mistakeCount;
        GameStats.PlayTime = playTime;
        GameStats.Difficulty = difficulty;
        GameStats.IsWin = true;
        GameStats.IsNewBest = isNewBest;

        StartCoroutine(LoadWinSceneRealtime(winSceneDelaySeconds));
    }

    private void GameOver()
    {
        isGameFinished = true;
        isPaused = true;
        Time.timeScale = 0f;

        ShowFinalScore();
        if (gameOverPanel) gameOverPanel.SetActive(true);

        GameStats.Score = 0;
        GameStats.HighScore = PlayerPrefs.GetInt(GetHighScoreKey(), highScore);
        GameStats.Mistakes = mistakeCount;
        GameStats.PlayTime = playTime;
        GameStats.Difficulty = difficulty;
        GameStats.IsWin = false;
        GameStats.IsNewBest = false;

       
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
        if (mistakesText) mistakesText.text = $"{mistakeCount}/{maxMistakes}";
    }
    private void UpdateAllTimeUI()
    {
        if (allTimeText) allTimeText.text = $"{highScore}";
    }
    private void ShowFinalScore()
    {
        if (finalScoreText) finalScoreText.text = $"Final Score: {currentScore}";
    }
    private void UpdateTimerUI()
{
    if (timerText == null) return;

    int totalSeconds = Mathf.FloorToInt(playTime);
    int minutes = totalSeconds / 60;
    int seconds = totalSeconds % 60;

    timerText.text = $"{minutes:00}:{seconds:00}";
}


    // ===== pause system =====
   public void PauseGame()
{
    if (isPaused || isGameFinished) return;
    isPaused = true;
    Time.timeScale = 0f;

    if (inputRoot) inputRoot.SetActive(false);

    // 🔹 Hiện PausePanel
    if (pausePanel) pausePanel.SetActive(true);

    // 🔄 Đổi icon sang Play khi game bị pause
    if (pauseButtonImage != null && playSprite != null)
        pauseButtonImage.sprite = playSprite;
}

public void ResumeGame()
{
    if (!isPaused || isGameFinished) return;
    isPaused = false;
    Time.timeScale = 1f;

    if (inputRoot) inputRoot.SetActive(true);

    // 🔹 Ẩn PausePanel
    if (pausePanel) pausePanel.SetActive(false);

    // 🔄 Đổi icon về Pause khi game chạy lại
    if (pauseButtonImage != null && pauseSprite != null)
        pauseButtonImage.sprite = pauseSprite;
}

public void TogglePause()
{
    if (isPaused) ResumeGame();
    else PauseGame();
}

// 🔸 Gắn vào nút "Continue" trong popup
public void OnClickContinue()
{
    ResumeGame();
}

    // ===== game flow =====
  public void RestartGame()
{
    Time.timeScale = 1f;

    // 🧹 Xóa dữ liệu Sudoku đã lưu để khi reload, bảng được tạo lại sạch sẽ
    SaveSystem.ClearSave();

    // 🔄 Load lại scene hiện tại
    var s = SceneManager.GetActiveScene();
    SceneManager.LoadScene(s.name);
}
    public void GoToMenu()
    {
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(menuSceneName)) SceneManager.LoadScene(menuSceneName);
    }

    // ===== helper getters =====
    public bool CanInput() => !isPaused && !isGameFinished;
    public int GetMistakeCount() => mistakeCount;
    public int GetMaxMistakes() => maxMistakes;
}


// Snapshot holder
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
