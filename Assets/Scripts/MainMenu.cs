using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;

    [Header("Display Settings")]
    [SerializeField] private Toggle fullscreenToggle;

    private void Start()
    {
        InitializeFullscreen();
        InitializeVolume();
        LoadSavedSettings();
    }

    private void LoadSavedSettings()
    {
        // Load fullscreen
        bool isFullScreen = PlayerPrefs.GetInt("FullScreen", 1) == 1;
        fullscreenToggle.isOn = isFullScreen;
        Screen.fullScreenMode = isFullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;

        // Load volume
        float savedVolume = PlayerPrefs.GetFloat("Volume", 0.75f);
        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);
    }

    private void InitializeVolume()
    {
        float volume = 0.75f;
        if (audioMixer.GetFloat("Volume", out float volumeValue))
        {
            volume = Mathf.Pow(10, volumeValue / 20);
        }

        volumeSlider.value = volume;
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    private void InitializeFullscreen()
    {
        fullscreenToggle.isOn = Screen.fullScreen;
        fullscreenToggle.onValueChanged.AddListener(SetFullScreen);
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20);
        PlayerPrefs.SetFloat("Volume", volume);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreenMode = isFullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        PlayerPrefs.SetInt("FullScreen", isFullScreen ? 1 : 0);
    }

    public void SaveSettings()
    {
        PlayerPrefs.Save();
        Debug.Log("Settings Saved!");
    }

    public void QuitApp()
    {
        SaveSettings();
        Application.Quit();
    }
}
