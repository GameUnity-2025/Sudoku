using UnityEngine;
using UnityEngine.SceneManagement; // Thư viện cần thiết để chuyển cảnh (scene)

public class MainMenuManager : MonoBehaviour
{
    // Hàm này sẽ được gọi bởi nút Play
    public void PlayGame()
    {
        // Thay "TenSceneGameCuaBan" bằng tên scene game chính của bạn.
        // Bạn có thể xem tên scene trong thư mục Assets/Scenes.
        // Dựa vào hình của bạn, có thể tên scene là "Sudoku" hoặc "Main"
        SceneManager.LoadScene("LevelSelect"); // <<=== THAY ĐỔI TÊN SCENE Ở ĐÂY
        Debug.Log("Chuyển đến màn hình chơi game!");
    }

    // Hàm này cho nút Continue
    public void ContinueGame()
    {
        // Chức năng này phức tạp, cần hệ thống lưu game.
        // Tạm thời chúng ta sẽ in ra một thông báo.
        Debug.Log("Nút Continue được bấm! (Chưa có chức năng)");
    }

    // Hàm này cho nút Settings
    public void OpenSettings()
    {
        // Tương tự, chức năng này cần một màn hình cài đặt riêng.
        Debug.Log("Nút Settings được bấm! (Chưa có chức năng)");
    }

    // Hàm này cho nút Quit
    public void QuitGame()
    {
        // In ra thông báo để kiểm tra trong Editor
        Debug.Log("Đã bấm nút thoát game!");
        // Lệnh này chỉ hoạt động khi game đã được build thành file .exe
        Application.Quit();
    }
}