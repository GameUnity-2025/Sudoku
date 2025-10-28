using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WinController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text titleBest;     // banner “All-Time New Best Score”
    // [SerializeField] private TMP_Text resultText;   // “All-Time New Best” (first/newbest) / “Excellent!”
    [SerializeField] private TMP_Text scoreText;     // điểm ván này
    [SerializeField] private TMP_Text bestValue;     // best all-time
    // [SerializeField] private TMP_Text mistakesText;  // optional
    // [SerializeField] private TMP_Text timeText;      // optional
    [SerializeField] private TMP_Text excellentText; // Thêm dòng này


    [Header("Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button menuButton;

    [Header("FX (optional)")]
    [SerializeField] private ParticleSystem confetti;
    // [SerializeField] private AudioSource fanfare;

    [Header("Scene Names")]
    [SerializeField] private string playSceneName = "SudokuPlay";
    [SerializeField] private string menuSceneName = "MainMenu";

    private void Start()
    {
        int score = GameStats.Score;
        int best = GameStats.HighScore;
        int mistakes = GameStats.Mistakes;
        float playTime = GameStats.PlayTime;
        bool isNewBest = GameStats.IsNewBest;
        string diff = string.IsNullOrEmpty(GameStats.Difficulty) ? "Easy" : GameStats.Difficulty;

        // Fallback khi mở trực tiếp WinScene
        if (best <= 0)
        {
            string key = PlayerPrefs.HasKey($"HighScore_{diff}") ? $"HighScore_{diff}" : "HighScore";
            best = PlayerPrefs.GetInt(key, 0);
        }

        // ===== HIỂN THỊ =====
        // Khi là lần đầu / kỷ lục mới → hiện banner + lời “All-Time New Best”
        // Ngược lại → chỉ hiện “Excellent!”
        if (titleBest) titleBest.gameObject.SetActive(isNewBest);
        if (resultText) resultText.text = isNewBest ? "All-Time New Best" : "Excellent!";

        if (scoreText) scoreText.text = score.ToString("N0");
        if (bestValue) bestValue.text = best.ToString("N0");
        if (mistakesText) mistakesText.text = $"Mistakes {mistakes}";
        if (timeText) timeText.text = FormatTime(playTime);

        if (isNewBest)
        {
            if (confetti) confetti.Play();
            if (fanfare) fanfare.Play();
        }

        // ==== Cập nhật thêm logic ====
        // Hiển thị "Excellent!" nếu điểm thấp hơn Best Score
        if (isNewBest)
        {
            titleBest.gameObject.SetActive(true); // Hiển thị "All-Time New Best"
            resultText.text = "All-Time New Best"; // Nếu có kỷ lục mới
            if (excellentText) excellentText.gameObject.SetActive(false); // Ẩn Excellent!
        }
        else
        {
            titleBest.gameObject.SetActive(false);  // Ẩn banner
            resultText.text = "Excellent!"; // Khi điểm thấp hơn best score
            if (excellentText) excellentText.gameObject.SetActive(true); // Hiển thị Excellent!
        }


        // ==== Các Button ====
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
                if (!string.IsNullOrEmpty("MainMenu"))
                    SceneManager.LoadScene("MainMenu");
            });
    }

    // Hàm định dạng thời gian
    private string FormatTime(float sec)
    {
        int m = Mathf.FloorToInt(sec / 60f);
        int s = Mathf.FloorToInt(sec % 60f);
        return $"{m:00}:{s:00}";
    }
}
