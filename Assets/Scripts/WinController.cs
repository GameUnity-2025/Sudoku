using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WinController : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] private TMP_Text titleBest;   // “All-Time New Best Score”
    [SerializeField] private TMP_Text scoreText;   // điểm hiện tại
    [SerializeField] private TMP_Text bestValue;   // best all-time
    [SerializeField] private Button newGameButton; // nút New Game

    private const string BestKey = "sudoku_bestscore"; // key lưu Best Score

    private void Start()
    {
        // Hiển thị điểm ván này
        int current = GameStats.Score;
        if (scoreText) scoreText.text = current.ToString("N0");

        // Đọc best cũ
        int best = PlayerPrefs.GetInt(BestKey, 0);

        // Nếu lập kỷ lục mới
        bool isNewBest = current > best;
        if (isNewBest)
        {
            PlayerPrefs.SetInt(BestKey, current);
            PlayerPrefs.Save();
            if (titleBest) titleBest.gameObject.SetActive(true);
        }
        else
        {
            if (titleBest) titleBest.gameObject.SetActive(false);
        }

        // Cập nhật hiển thị Best
        if (bestValue)
        {
            int nowBest = PlayerPrefs.GetInt(BestKey, current);
            bestValue.text = nowBest.ToString("N0");
        }

        // Xử lý nút New Game
        if (newGameButton)
            newGameButton.onClick.AddListener(() =>
            {
                // Đổi "YourGameplayScene" thành scene bạn muốn quay lại
                SceneManager.LoadScene("YourGameplayScene");
            });
    }
}
