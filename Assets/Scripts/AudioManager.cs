using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // Singleton
    private AudioSource audioSource;
    private bool isMuted = false;

    private void Awake()
    {
        // Chỉ cho phép tồn tại 1 AudioManager duy nhất
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ lại khi đổi scene

            audioSource = GetComponent<AudioSource>();

            if (audioSource != null)
            {
                audioSource.loop = true;         // Lặp nhạc
                audioSource.playOnAwake = false; // Không tự phát khi Awake

                // Nếu chưa phát thì phát nhạc
                if (!audioSource.isPlaying && !isMuted)
                {
                    audioSource.Play();
                }
            }
            else
            {
                Debug.LogWarning("⚠ AudioManager: Không tìm thấy AudioSource trên GameObject này!");
            }
        }
        else
        {
            Destroy(gameObject); // Hủy bản trùng lặp
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Khi load scene mới
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Không cần auto stop ở WinScene nữa — cho phép người chơi mute theo ý
        if (audioSource != null && !audioSource.isPlaying && !isMuted)
        {
            audioSource.Play();
        }

        // Giữ đúng trạng thái mute khi đổi scene
        audioSource.mute = isMuted;
    }

    // Bật / tắt nhạc (gọi từ các nút Mute)
    public void ToggleMute()
    {
        if (audioSource == null) return;

        isMuted = !isMuted;
        audioSource.mute = isMuted;
    }

    // Trả về trạng thái mute hiện tại
    public bool IsMuted()
    {
        return isMuted;
    }
}
