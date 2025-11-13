using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Hiển thị lịch sử chơi trên HistoryPanel (Main Menu).
/// Cách đơn giản: đổ chuỗi vào một TextMeshProUGUI nhiều dòng.
/// Optional: nút Clear và nút Reload có thể gắn vào các hàm công khai.
/// </summary>
public class HistoryPanelUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI listText;   // Gắn một TMP_Text trong panel
    [SerializeField] private GameObject emptyState;       // Dòng "No history" nếu không có dữ liệu (optional)
    [SerializeField] private Button clearButton;          // Nút Xoá lịch sử (optional)
    [SerializeField] private Button reloadButton;         // Nút Refresh (optional)

    [Header("Display")]
    [SerializeField] private int maxEntriesToShow = 50;   // giới hạn hiển thị để tránh chuỗi quá dài

    private void Awake()
    {
        // Gắn handler nếu có
        if (clearButton)
        {
            clearButton.onClick.RemoveAllListeners();
            clearButton.onClick.AddListener(ClearHistory);
        }
        if (reloadButton)
        {
            reloadButton.onClick.RemoveAllListeners();
            reloadButton.onClick.AddListener(Refresh);
        }

        // Cố gắng tự tìm TMP_Text nếu chưa gán trong Inspector
        TryAutoFindListText();
    }

    private void OnEnable()
    {
        // Auto-wire lần nữa trong trường hợp Awake chưa bắt được
        TryAutoFindListText();
        Refresh();
    }

    public void Refresh()
    {
        if (listText == null)
        {
            // Tạo tạm một Text nếu không có để đảm bảo người dùng thấy được nội dung
            var go = new GameObject("HistoryListText", typeof(RectTransform));
            go.transform.SetParent(transform, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.1f, 0.1f);
            rt.anchorMax = new Vector2(0.9f, 0.9f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            listText = go.AddComponent<TextMeshProUGUI>();
            listText.enableWordWrapping = true;
            listText.fontSize = 28f;
            listText.alignment = TextAlignmentOptions.TopLeft;

            Debug.LogWarning("HistoryPanelUI: Auto-created a TMP_Text because none was assigned.");
        }

        List<HistorySystem.HistoryEntry> all = HistorySystem.GetAll();
        if (all == null || all.Count == 0)
        {
            listText.text = "No history yet.";
            if (emptyState) emptyState.SetActive(true);
            return;
        }
        if (emptyState) emptyState.SetActive(false);

        // Mới nhất lên trên
        int start = Mathf.Max(0, all.Count - maxEntriesToShow);
        var sb = new StringBuilder();
        for (int i = all.Count - 1; i >= start; i--)
        {
            var e = all[i];
            string mmss = FormatDuration(e.durationSeconds);
            string w = e.isWin ? "Win" : "Lose";
            string diff = string.IsNullOrEmpty(e.difficulty) ? "" : $"[{e.difficulty}] ";
            // Dòng: 2025-11-13 17:20:33  [Easy]  05:42  Win  Score: 6200
            sb.AppendLine($"{e.playedAt}  {diff}{mmss}  {w}  Score: {e.score}");
        }

        listText.text = sb.ToString();
        Debug.Log($"HistoryPanelUI: rendered {all.Count} entries (showing up to {Mathf.Min(all.Count, maxEntriesToShow)}).");
    }

    public void ClearHistory()
    {
        HistorySystem.Clear();
        Refresh();
    }

    private static string FormatDuration(int seconds)
    {
        int m = seconds / 60;
        int s = seconds % 60;
        return $"{m:00}:{s:00}";
    }

    private void TryAutoFindListText()
    {
        if (listText != null) return;

        var tmps = GetComponentsInChildren<TextMeshProUGUI>(true);
        if (tmps != null && tmps.Length > 0)
        {
            // Ưu tiên tên gợi ý
            foreach (var t in tmps)
            {
                string n = t.gameObject.name.ToLower();
                if (n.Contains("list") || n.Contains("content") || n.Contains("history"))
                {
                    listText = t;
                    return;
                }
            }
            // Nếu không có, chọn text có Rect cao nhất (thường là ô nội dung)
            TextMeshProUGUI best = tmps[0];
            float bestH = 0f;
            foreach (var t in tmps)
            {
                var rt = t.rectTransform;
                float h = Mathf.Abs(rt.rect.height);
                if (h > bestH)
                {
                    bestH = h;
                    best = t;
                }
            }
            listText = best;
        }
    }
}
