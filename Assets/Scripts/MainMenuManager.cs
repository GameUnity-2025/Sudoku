using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // --- PHẦN MỚI THÊM VÀO ---
    // Kéo các Panel từ Hierarchy vào các ô này trong Inspector
    [SerializeField] private GameObject menuPanel;       // Panel chứa các nút Play, Continue, Setting, Quit
    [SerializeField] private GameObject settingsPanel;   // Panel chứa các cài đặt (sẽ tạo sau)

    void Start()
    {
        // Đảm bảo khi game bắt đầu, chỉ có menu chính được hiển thị
        menuPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    // --- CÁC HÀM CŨ VẪN GIỮ NGUYÊN ---
    // Hàm này sẽ được gọi bởi nút Play
    public void PlayGame()
    {
        SceneManager.LoadScene("LevelSelect");
        Debug.Log("Chuyển đến màn hình chọn độ khó!");
    }

    // Hàm này cho nút Continue
    public void ContinueGame()
    {
        Debug.Log("Nút Continue được bấm! (Chưa có chức năng)");
    }

    // --- HÀM SETTINGS ĐƯỢC NÂNG CẤP ---
    // Hàm này cho nút Settings, giờ nó sẽ mở panel cài đặt
    public void OpenSettingsPanel() // Đổi tên từ OpenSettings để rõ ràng hơn
    {
        menuPanel.SetActive(false);     // Ẩn panel menu chính
        settingsPanel.SetActive(true);    // Hiện panel cài đặt
        Debug.Log("Đã mở màn hình cài đặt!");
    }

    // --- HÀM MỚI ĐỂ ĐÓNG CÀI ĐẶT ---
    // Hàm này sẽ được dùng cho nút "Back" hoặc "Quay Lại" trong màn hình cài đặt
    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);   // Ẩn panel cài đặt
        menuPanel.SetActive(true);      // Hiện lại panel menu chính
    }


    // Hàm này cho nút Quit
    public void QuitGame()
    {
        Debug.Log("Đã bấm nút thoát game!");
        Application.Quit(); // Dùng Application.Quit() để thoát game khi đã build
        // SceneManager.LoadScene("LevelSelect"); // Dòng này có thể không cần thiết nữa nếu ý định là thoát game
    }


}