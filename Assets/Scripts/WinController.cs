using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WinController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text titleBest;     // "All-Time New Best Score" (để inactive sẵn)
    [SerializeField] private TMP_Text resultText;    // "You Win!" / "Game Over"
    [SerializeField] private TMP_Text scoreText;     // Điểm ván này
    [SerializeField] private TMP_Text bestValue;     // Best All-Time
    [SerializeField] private TMP_Text mistakesText;  // "Mistakes x/3" (optional)
    [SerializeField] private TMP_Text timeText;      // "Time 00:00"   (optional)

    [Header("Buttons")]
    [SerializeField] private Button newGameButton;   // Quay lại scene chơi (SudokuPlay)
    [SerializeField] private Button menuButton;      // Về MainMenu (nếu có)

    [Header("FX (optional)")]
    [SerializeField] private ParticleSystem confetti;
    [SerializeField] private AudioSource fanfare;    // âm chúc mừng nếu muốn

    [Header("Scene Names")]
    [SerializeField] private string playSceneName = "SudokuPlay";
    [SerializeField] private string menuSceneName = "MainMenu";

    private void Start()
    {
        // Đọc dữ liệu snapshot từ GameStats
        int score = GameStats.Score;
        int best = GameStats.HighScore;         // đã chốt từ GameStatsManager
        int mistakes = GameStats.Mistakes;
        float playTime = GameStats.PlayTime;          // (alias cho ElapsedSec)
        bool isWin = GameStats.IsWin;
        bool isNewBest = GameStats.IsNewBest;
        string diff = string.IsNullOrEmpty(GameStats.Difficulty) ? "Easy" : GameStats.Difficulty;

        // Fallback: nếu vào trực tiếp WinScene khi test, lấy best từ PlayerPrefs
        if (best <= 0)
        {
            // nếu team bạn tách best theo độ khó, đổi key ở đây
            string key = "HighScore"; // hoặc $"HighScore_{diff}"
            best = PlayerPrefs.GetInt(key, 0);
        }

        // ==== Hiển thị ====
        if (resultText) resultText.text = isWin ? "You Win!" : "Game Over";
        if (scoreText) scoreText.text = score.ToString("N0");
        if (bestValue) bestValue.text = best.ToString("N0");
        if (mistakesText) mistakesText.text = $"Mistakes {mistakes}";
        if (timeText) timeText.text = FormatTime(playTime);

        if (titleBest) titleBest.gameObject.SetActive(isNewBest);
        if (isNewBest)
        {
            if (confetti) confetti.Play();
            if (fanfare) fanfare.Play();
        }

        // ==== Buttons ====
        if (newGameButton)
            newGameButton.onClick.AddListener(() =>
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene("LevelSelect");
            });

        if (menuButton)
            menuButton.onClick.AddListener(() =>
            {
                Time.timeScale = 1f;
                if (!string.IsNullOrEmpty(menuSceneName))
                    SceneManager.LoadScene(menuSceneName);
            });
    }

    private string FormatTime(float sec)
    {
        int m = Mathf.FloorToInt(sec / 60f);
        int s = Mathf.FloorToInt(sec % 60f);
        return $"{m:00}:{s:00}";
    }
}
