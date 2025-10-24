using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameStatsManager : MonoBehaviour
{
    public static GameStatsManager instance;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI allTimeText;
    [SerializeField] private TextMeshProUGUI mistakesText;
    [SerializeField] private TextMeshProUGUI currentScoreText;  // Điểm hiện tại
    [SerializeField] private TextMeshProUGUI finalScoreText;    // Điểm cuối cùng khi thua/thắng
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Game Settings")]
    [SerializeField] private int maxMistakes = 3;
    [SerializeField] private string difficulty = "Easy"; // Nhận từ PlayerPrefs

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
        // Load high score
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        // Load difficulty if available
        if (PlayerPrefs.HasKey("Difficulty"))
            difficulty = PlayerPrefs.GetString("Difficulty");

        UpdateAllTimeUI();
        UpdateMistakesUI();
        UpdateCurrentScoreUI();

        Time.timeScale = 1f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
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

    // Tính điểm
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

    // Cập nhật UI đang chơi
    private void UpdateCurrentScoreUI()
    {
        if (currentScoreText != null)
            currentScoreText.text = $"Score: {currentScore}";
    }

    private void UpdateMistakesUI()
    {
        if (mistakesText != null)
            mistakesText.text = $"Mistakes: {mistakeCount}/{maxMistakes}";
    }

    // Gây lỗi
    public void AddMistake()
    {
        if (isPaused || isGameFinished) return;

        mistakeCount++;
        UpdateMistakesUI();

        if (mistakeCount >= maxMistakes)
            GameOver();
    }

    // Xử lý Game Over
    private void GameOver()
    {
        isGameFinished = true;
        isPaused = true;
        Time.timeScale = 0f;

        ShowFinalScore();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Debug.Log("❌ Game Over! Too many mistakes.");
    }

    // Xử lý hoàn thành game
    public void CompleteGame()
    {
        if (isGameFinished) return;

        isGameFinished = true;
        isPaused = true;
        Time.timeScale = 0f;

        currentScore = CalculateScore();

        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        UpdateAllTimeUI();
        ShowFinalScore();

        Debug.Log($"🎉 Game Completed | Score: {currentScore} | High Score: {highScore}");
    }

    // Hiển thị điểm cuối
    private void ShowFinalScore()
    {
        if (finalScoreText != null)
            finalScoreText.text = $"Final Score: {currentScore}";
    }

    // UI điểm cao nhất
    private void UpdateAllTimeUI()
    {
        if (allTimeText != null)
            allTimeText.text = $"All Time: {highScore}";
    }

    // Pause & Resume
    public void PauseGame()
    {
        if (isPaused || isGameFinished) return;

        isPaused = true;
        Time.timeScale = 0f;
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

    // Restart game
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Quay lại menu
    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Tên scene menu của bạn
    }

    public bool CanInput()
    {
        return !isPaused && !isGameFinished;
    }

    public int GetMistakeCount() => mistakeCount;
    public int GetMaxMistakes() => maxMistakes;
}
