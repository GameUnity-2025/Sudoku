using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    // --- PHẦN MỚI THÊM VÀO ---
    // Kéo các Panel từ Hierarchy vào các ô này trong Inspector
    [SerializeField] private GameObject menuPanel;       // Panel chứa các nút Play, Continue, Setting, Quit
    [SerializeField] private GameObject settingsPanel;   // Panel chứa các cài đặt (sẽ tạo sau)
    [SerializeField] private GameObject historyPanel;  

    void Start()
    {
        // Đảm bảo khi game bắt đầu, chỉ có menu chính được hiển thị
        menuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        if (historyPanel != null)
    historyPanel.SetActive(false);
        // Try to auto-wire Continue button if it's present but not assigned in the Inspector
        try
        {
            var go = GameObject.Find("ContinueButton");
            if (go != null)
            {
                var btn = go.GetComponent<Button>();
                if (btn != null)
                {
                    // Remove duplicates and add our listener
                    btn.onClick.RemoveListener(ContinueGame);
                    btn.onClick.AddListener(ContinueGame);
#if UNITY_EDITOR
                    Debug.Log("MainMenuManager: Auto-wired ContinueButton to ContinueGame().");
#endif
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("MainMenuManager: Failed to auto-wire ContinueButton: " + e.Message);
        }
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
        // If a saved game exists, load the gameplay scene which will read the save on Start
        if (SaveSystem.HasSave())
        {
            Debug.Log("Continue: Save found, loading last game.");
            SceneManager.LoadScene("SudokuPlay");
        }
        else
        {
            Debug.Log("Continue: No save found, going to LevelSelect.");
            SceneManager.LoadScene("LevelSelect");
        }
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

    public void OpenHistoryPanel()
{
    menuPanel.SetActive(false);
    if (settingsPanel != null) settingsPanel.SetActive(false);
    if (historyPanel != null)
    {
        historyPanel.SetActive(true);
        // Đảm bảo panel có HistoryPanelUI và refresh nội dung
        var ui = historyPanel.GetComponent<HistoryPanelUI>();
        if (ui == null)
        {
            ui = historyPanel.AddComponent<HistoryPanelUI>();
        }
        ui.Refresh();
        Debug.Log("Đã mở màn hình lịch sử game!");
    }
}

public void CloseHistoryPanel()
{
    if (historyPanel != null)
        historyPanel.SetActive(false);

    // 🔸 Quay lại SettingsPanel thay vì MenuPanel
    if (settingsPanel != null)
        settingsPanel.SetActive(true);

    Debug.Log("Đã quay lại màn hình cài đặt từ lịch sử!");
}



}