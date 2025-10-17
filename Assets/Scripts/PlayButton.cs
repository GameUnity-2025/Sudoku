using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayButton : MonoBehaviour
{
    public void StartButtonClicked()
    {
        Debug.Log("Starting game with difficulty: " + PlayerSettings.difficulty);
        SceneManager.LoadScene("SudokuPlay");
    }

    public void MainMenuButtonClicked()
    {
        SceneManager.LoadScene("LevelSelect");
    }
}
