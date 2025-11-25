using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashManager : MonoBehaviour
{
    // Thời gian chờ (giây)
    public float waitTime = 3f;

    void Start()
    {
        StartCoroutine(LoadMainMenu());
    }

    IEnumerator LoadMainMenu()
    {
        // Chờ 3 giây
        yield return new WaitForSeconds(waitTime);
        
        // Chuyển sang màn hình Menu (Đảm bảo tên Scene Menu của bạn đúng là "MainMenu")
        SceneManager.LoadScene("MainMenu"); 
    }
}