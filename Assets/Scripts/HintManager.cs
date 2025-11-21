using UnityEngine;
using UnityEngine.UI;

public class HintManager : MonoBehaviour
{
    [Header("Hint Settings")]
    public int startingHints = 3;
    public int currentHints;

    [Header("UI")]
    public Text hintText;          // Text hiển thị số hint còn lại (tuỳ chọn)
    public Button hintButton;      // Nút hint (tuỳ chọn)

    private Board board;

    private void Awake()
    {
        board = FindObjectOfType<Board>();
        if (board == null)
            Debug.LogError("❌ Board not found in scene!");
    }

    private void Start()
    {
        ResetHints();
        UpdateHintUI();
    }

    // Reset lại số hint khi start game
    public void ResetHints()
    {
        currentHints = startingHints;
    }

    // Hàm gán cho Unity Button OnClick
    public void OnHintButtonClicked()
    {
        UseHint();
    }

    // Dùng 1 hint
    public void UseHint()
    {
        if (currentHints <= 0)
        {
            Debug.Log("⚠ No hints left!");
            return;
        }

        if (board == null)
        {
            Debug.LogError("❌ Board not found!");
            return;
        }

        bool success = board.RevealRandomHint();

        if (success)
        {
            currentHints--;
            Debug.Log("✨ Hint used! Remaining: " + currentHints);
        }
        else
        {
            Debug.Log("⚠ No empty cells to reveal.");
        }

        UpdateHintUI();
    }

    // Dùng hint tự động khi bắt đầu (nếu cần)
    public void UseHintsAtStart(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (currentHints <= 0) break;

            bool success = board.RevealRandomHint();
            if (success)
                currentHints--;
            else
                break;
        }

        UpdateHintUI();
    }

    // Cập nhật UI hiển thị số hint
    private void UpdateHintUI()
    {
        if (hintText != null)
            hintText.text = currentHints.ToString();

        if (hintButton != null)
            hintButton.interactable = currentHints > 0;
    }
}
