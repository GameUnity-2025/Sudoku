using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowMistakesButton : MonoBehaviour
{
    private Board board;
    private bool showingMistakes = true; // Default to show mistakes
    public Text buttonText;

    void Start()
    {
        board = FindObjectOfType<Board>();
        UpdateButtonText();
    }

    public void ToggleMistakes()
    {
        showingMistakes = !showingMistakes;
        board.SetAutoCheckErrors(showingMistakes);
        UpdateButtonText();
    }

    private void UpdateButtonText()
    {
        if (buttonText != null)
        {
            buttonText.text = showingMistakes ? "Hide Mistakes" : "Show Mistakes";
        }
    }

    public bool IsShowingMistakes()
    {
        return showingMistakes;
    }
}
