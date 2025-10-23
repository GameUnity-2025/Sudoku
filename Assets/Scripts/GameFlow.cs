using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlow : MonoBehaviour
{
    // Gọi hàm này khi bàn chơi hoàn thành
    public void OnPuzzleSolved()
    {
        // TẠM: tính điểm dựa trên thời gian; bạn có thể thay công thức
        // Ví dụ: bắt đầu từ 10.000, mỗi giây trừ 20 điểm
        float elapsed = Time.timeSinceLevelLoad;
        int score = Mathf.Max(0, 10000 - Mathf.RoundToInt(elapsed * 20f));

        GameStats.Score = score;
        GameStats.ElapsedSec = elapsed;

        SceneManager.LoadScene("WinScene");
    }
}
