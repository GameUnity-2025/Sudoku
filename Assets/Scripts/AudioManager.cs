using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // Singleton
    private AudioSource audioSource;
    private bool isMuted = false;

    private void Awake()
    {
        // Singleton — chỉ giữ lại 1 AudioManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Lấy AudioSource
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                // Nếu chưa có AudioSource → tự thêm
                audioSource = gameObject.AddComponent<AudioSource>();
                Debug.LogWarning("⚠ AudioSource chưa có, đã tự động thêm mới.");
            }

            // Thiết lập mặc định
            audioSource.loop = true;
            audioSource.playOnAwake = false;

            // Lấy trạng thái mute đã lưu (nếu có)
            isMuted = PlayerPrefs.GetInt("Muted", 0) == 1;
            ApplyMuteState();

            // Chỉ phát nhạc nếu chưa mute
            if (!audioSource.isPlaying && !isMuted)
            {
                audioSource.Play();
            }
        }
        else
        {
            Destroy(gameObject);
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (audioSource != null)
        {
            ApplyMuteState();
            if (!audioSource.isPlaying && !isMuted)
                audioSource.Play();
        }
    }

    public void ToggleMute()
    {
        if (audioSource == null) return;

        isMuted = !isMuted;
        PlayerPrefs.SetInt("Muted", isMuted ? 1 : 0);
        ApplyMuteState();
    }

    private void ApplyMuteState()
    {
        if (audioSource != null)
        {
            audioSource.mute = isMuted;
        }
        AudioListener.volume = isMuted ? 0f : 1f;
    }

    public bool IsMuted()
    {
        return isMuted;
    }
}
