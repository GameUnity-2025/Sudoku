using UnityEngine;
using TMPro;

public class GameStatsManager : MonoBehaviour
{
    public static GameStatsManager instance;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI allTimeText;   // Hiển thị điểm cao nhất (All Time)
    [SerializeField] private TextMeshProUGUI mistakesText;  // Hiển thị số lỗi sai
    [SerializeField] private GameObject pauseButton;        // Nút Pause (icon hình tròn)
    [SerializeField] private GameObject gameOverPanel;      // (tuỳ chọn) panel hiện khi thua

    [Header("Game Settings")]
    [SerializeField] private int maxMistakes = 3;
    [SerializeField] private string difficulty = "Easy";    // Có thể nhận từ PlayerPrefs hoặc DifficultySelector

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
        Time.timeScale = 1f; // đảm bảo game chạy bình thường khi start
    }

    private void Update()
    {
        if (!isPaused && !isGameFinished)
            playTime += Time.deltaTime;
    }

    // 🧮 Tính điểm dựa theo độ khó, thời gian, lỗi
    private int CalculateScore()
    {
        int baseScore = 0;
        int maxTime = 600; // Giới hạn 10 phút
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

    // ❌ Cập nhật giao diện lỗi
    private void UpdateMistakesUI()
    {
        if (mistakesText != null)
            mistakesText.text = $"Mistakes: {mistakeCount}/{maxMistakes}";
    }

    // ❌ Gọi khi người chơi nhập sai
    public void AddMistake()
    {
        if (isPaused || isGameFinished) return;

        // ✅ chỉ cộng 1 lỗi/lần
        mistakeCount++;
        UpdateMistakesUI();

        if (mistakeCount >= maxMistakes)
            GameOver();
    }

    // 🔚 Khi sai quá giới hạn
    private void GameOver()
    {
        isGameFinished = true;
        isPaused = true;
        Time.timeScale = 0f; // dừng toàn bộ game

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Debug.Log("❌ Game Over! Too many mistakes.");
    }

    // ✅ Khi hoàn thành Sudoku
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

    // 🏆 Cập nhật hiển thị điểm cao nhất
    private void UpdateAllTimeUI()
    {
        if (allTimeText != null)
            allTimeText.text = $"All Time: {highScore}";
    }

    // 🔘 Tạm dừng game
    public void PauseGame()
    {
        if (isPaused || isGameFinished) return;

        isPaused = true;
        Time.timeScale = 0f; // dừng game thực tế

        // ẩn menu nhập số (nếu có)
        if (InputButton.instance != null)
            InputButton.instance.gameObject.SetActive(false);

        Debug.Log("⏸ Game Paused");
    }

    // ▶️ Tiếp tục game
    public void ResumeGame()
    {
        if (!isPaused || isGameFinished) return;

        isPaused = false;
        Time.timeScale = 1f; // tiếp tục game

        Debug.Log("▶️ Game Resumed");
    }

    // 🔄 Toggle khi bấm nút Pause
    public void TogglePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    // ⚙️ Kiểm tra có thể nhập ô Sudoku không
    public bool CanInput()
    {
        return !isPaused && !isGameFinished;
    }

    // ✅ Getter cho số lỗi hiện tại
    public int GetMistakeCount() => mistakeCount;

    // ✅ Getter cho giới hạn lỗi tối đa
    public int GetMaxMistakes() => maxMistakes;
}
