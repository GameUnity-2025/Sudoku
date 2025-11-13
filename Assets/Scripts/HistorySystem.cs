using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Lưu lịch sử chơi: ngày/giờ và thời lượng ván chơi (cộng thêm vài thông tin hữu ích).
/// Dữ liệu được lưu dưới dạng JSON trong PlayerPrefs (key "Sudoku_History").
/// </summary>
public static class HistorySystem
{
    private const string KEY = "Sudoku_History";
    private const string KEY_LAST = "Sudoku_History_LastAdded"; // chống ghi trùng trong vài giây

    [Serializable]
    public class HistoryEntry
    {
        // Ngày giờ chơi (local time) định dạng yyyy-MM-dd HH:mm:ss
        public string playedAt;

        // Thời lượng ván (giây, làm tròn xuống)
        public int durationSeconds;

        // Thông tin phụ trợ (không bắt buộc theo yêu cầu, nhưng hữu ích nếu cần hiển thị)
        public string difficulty;  // VD: Easy/Medium/Hard
        public bool isWin;         // Thắng/thua
        public int score;          // Điểm ván đó (nếu có)
    }

    [Serializable]
    private class HistoryData
    {
        public List<HistoryEntry> entries = new List<HistoryEntry>();
    }

    /// <summary>
    /// Ghi thêm 1 mục lịch sử.
    /// Yêu cầu bài: lưu ngày chơi và thời gian chơi. Các tham số khác có thể để trống nếu không dùng.
    /// </summary>
    public static void AddEntry(float playTimeSeconds, string difficulty, bool isWin, int score)
    {
        // Chống ghi trùng trong ~3 giây (khi nhiều luồng kết thúc cùng lúc)
        float now = Time.realtimeSinceStartup;
        float last = PlayerPrefs.GetFloat(KEY_LAST, -9999f);
        if (now - last < 3.0f)
        {
#if UNITY_EDITOR
            Debug.Log("⏭️ HistorySystem: Skipped duplicate add within 3s window.");
#endif
            return;
        }

        var data = LoadInternal();
        var entry = new HistoryEntry
        {
            playedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            durationSeconds = Mathf.FloorToInt(playTimeSeconds),
            difficulty = difficulty ?? "",
            isWin = isWin,
            score = score
        };

        data.entries.Add(entry);
        SaveInternal(data);
        PlayerPrefs.SetFloat(KEY_LAST, now);
        PlayerPrefs.Save();

#if UNITY_EDITOR
        Debug.Log($"📝 HistorySystem: Added entry {entry.playedAt}, {entry.durationSeconds}s, diff={entry.difficulty}, win={entry.isWin}, score={entry.score}");
#endif
    }

    /// <summary>
    /// Lấy toàn bộ danh sách lịch sử.
    /// </summary>
    public static List<HistoryEntry> GetAll()
    {
        return LoadInternal().entries;
    }

    /// <summary>
    /// Xóa toàn bộ lịch sử.
    /// </summary>
    public static void Clear()
    {
        PlayerPrefs.DeleteKey(KEY);
        PlayerPrefs.Save();
#if UNITY_EDITOR
        Debug.Log("🗑️ HistorySystem: Cleared all history.");
#endif
    }

    // ===== private helpers =====
    private static HistoryData LoadInternal()
    {
        if (!PlayerPrefs.HasKey(KEY))
        {
            return new HistoryData();
        }

        try
        {
            string json = PlayerPrefs.GetString(KEY);
            var data = JsonUtility.FromJson<HistoryData>(json);
            if (data == null || data.entries == null)
            {
                return new HistoryData();
            }
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ HistorySystem: Failed to load history - {e.Message}");
            return new HistoryData();
        }
    }

    private static void SaveInternal(HistoryData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(KEY, json);
            PlayerPrefs.Save();
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ HistorySystem: Failed to save history - {e.Message}");
        }
    }
}
