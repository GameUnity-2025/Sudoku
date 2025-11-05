using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WinController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text titleBest;      // Banner "All-Time New Best"
    [SerializeField] private TMP_Text resultText;     // Dòng tiêu đề: "All-Time New Best" / "Excellent!"
    [SerializeField] private TMP_Text scoreText;      // Điểm ván này
    [SerializeField] private TMP_Text bestValue;      // Best all-time
    [SerializeField] private TMP_Text mistakesText;   // optional
    [SerializeField] private TMP_Text timeText;       // optional
    [SerializeField] private TMP_Text excellentText;  // Dòng "Excellent!" riêng (nếu bạn đặt riêng)

    [Header("Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button menuButton;

    [Header("FX (optional)")]
    [SerializeField] private ParticleSystem confetti;
    [SerializeField] private AudioSource fanfare;

    [Header("Scene Names")]
    [SerializeField] private string playSceneName = "LevelSelect";
    [SerializeField] private string menuSceneName = "MainMenu";

    private void Start()
    {
        // ---- Read snapshot ----
        int   score     = GameStats.Score;
        int   best      = GameStats.HighScore;
        int   mistakes  = GameStats.Mistakes;
        float playTime  = GameStats.PlayTime;
        bool  isNewBest = GameStats.IsNewBest;
        string diff     = string.IsNullOrEmpty(GameStats.Difficulty) ? "Easy" : GameStats.Difficulty;

        // Fallback khi mở trực tiếp WinScene
        if (best <= 0)
        {
            string key = PlayerPrefs.HasKey($"HighScore_{diff}") ? $"HighScore_{diff}" : "HighScore";
            best = PlayerPrefs.GetInt(key, 0);
        }

        // ---- Fill UI numbers ----
        if (scoreText)   scoreText.text   = score.ToString("N0");
        if (bestValue)   bestValue.text   = best.ToString("N0");
        if (mistakesText) mistakesText.text = $"Mistakes {mistakes}";
        if (timeText)    timeText.text    = FormatTime(playTime);

        // ---- SINGLE toggle for states (avoid duplicate logic) ----
        if (titleBest)      titleBest.gameObject.SetActive(isNewBest);   // Banner chỉ xuất hiện khi new best
        if (excellentText)  excellentText.gameObject.SetActive(!isNewBest); // Excellent chỉ khi KHÔNG new best
        if (resultText)     resultText.text = isNewBest ? "All-Time New Best" : "Excellent!";

        // (An toàn) Nếu tắt ExcellentText, xóa chữ để tránh layout/voice over đọc nhầm
        if (excellentText && isNewBest) excellentText.text = string.Empty;

        // ---- FX ----
        if (isNewBest)
        {
            if (confetti) confetti.Play();
            if (fanfare)  fanfare.Play();
        }

        // ---- Buttons ----
        if (newGameButton)
            newGameButton.onClick.AddListener(() =>
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(playSceneName);  // quay lại chơi
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
