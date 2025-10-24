using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameStatsManager : MonoBehaviour
{
    public static GameStatsManager instance;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI allTimeText;   // Hiển thị điểm cao nhất (All Time)
    [SerializeField] private TextMeshProUGUI mistakesText;  // Hiển thị số lỗi sai
    [SerializeField] private GameObject pauseButton;        // Nút Pause
    [SerializeField] private GameObject gameOverPanel;      // Panel hiện khi thua

    [Header("Game Settings")]
    [SerializeField] private int maxMistakes = 3;
    [SerializeField] private string difficulty = "Easy";    // Nhận từ PlayerPrefs

    private float playTime = 0f;
    private int mistakeCount = 0;
    private bool isPaused = false;
    private bool isGameFinished = false;

    private int currentScore = 0;
    private int highScore = 0;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (PlayerPrefs.HasKey("Difficulty"))
            difficulty = PlayerPrefs.GetString("Difficulty");

        UpdateAllTimeUI();
        UpdateMistakesUI();
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (!isPaused && !isGameFinished)
            playTime += Time.deltaTime;
    }

    private int CalculateScore()
    {
        int baseScore = 0;
        int maxTime = 600; // 10 phút
        switch (difficulty)
        {
            case "Easy": baseScore = 1000; break;
            case "Medium": baseScore = 2000; break;
            case "Hard": baseScore = 3000; break;
            default: baseScore = 1000; break;
        }

        int timeBonus = Mathf.Max(0, (int)(maxTime - playTime));
        int mistakePenalty = mistakeCount * 1000;
        return Mathf.Max(0, baseScore + timeBonus - mistakePenalty);
    }

    private void UpdateMistakesUI()
    {
        if (mistakesText != null)
            mistakesText.text = $"Mistakes: {mistakeCount}/{maxMistakes}";
    }

    public void AddMistake()
    {
        if (isPaused || isGameFinished) return;

        mistakeCount++;
        UpdateMistakesUI();

        if (mistakeCount >= maxMistakes)
            GameOver();
    }

    // ✅ Giữ lại phiên bản GameOver này thôi
    private void GameOver()
    {
        isGameFinished = true;
        isPaused = true;
        Time.timeScale = 0f; // dừng toàn bộ game

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Debug.Log("❌ Game Over! Too many mistakes.");

        // 🕹️ Chuyển scene sau 1s
        Invoke(nameof(LoadGameOverScene), 1f);
    }

    private void LoadGameOverScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("WinScense"); // ⚠️ đổi tên scene thành WinScense
    }

    public void CompleteGame()
    {
        if (isGameFinished) return;

        isGameFinished = true;
        isPaused = true;
        Time.timeScale = 0f;

        currentScore = CalculateScore();

        GameStats.Score = currentScore;
        GameStats.ElapsedSec = playTime;

        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        UpdateAllTimeUI();

        Debug.Log($"🎉 Game Completed | Score: {currentScore} | High Score: {highScore}");
    }

    private void UpdateAllTimeUI()
    {
        if (allTimeText != null)
            allTimeText.text = $"All Time: {highScore}";
    }

    public void PauseGame()
    {
        if (isPaused || isGameFinished) return;

        isPaused = true;
        Time.timeScale = 0f;

        if (InputButton.instance != null)
            InputButton.instance.gameObject.SetActive(false);

        Debug.Log("⏸ Game Paused");
    }

    public void ResumeGame()
    {
        if (!isPaused || isGameFinished) return;

        isPaused = false;
        Time.timeScale = 1f;

        Debug.Log("▶️ Game Resumed");
    }

    public void TogglePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public bool CanInput()
    {
        return !isPaused && !isGameFinished;
    }

    public int GetMistakeCount() => mistakeCount;
    public int GetMaxMistakes() => maxMistakes;
}
