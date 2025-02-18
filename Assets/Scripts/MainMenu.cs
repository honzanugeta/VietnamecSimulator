using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] RectTransform Logo;
    [SerializeField] float topposy, middleposy;
    [SerializeField] float tweenDuration;
    [SerializeField] AudioMixer audioMixer; 
    [SerializeField] TMP_Dropdown resolutionDropdown; 
    [SerializeField] TMP_Dropdown qualityDropdown; 
    [SerializeField] TMP_Dropdown antiAliasingDropdown; 
    [SerializeField] TMP_Dropdown textureQualityDropdown;
    [SerializeField] TMP_Dropdown refreshrateDropdown;
    [SerializeField] Slider volumeSlider; 
    [SerializeField] Toggle fullscreenToggle;
    private Resolution[] resolutions;
    
    
    // Start is called before the first frame update
    void Start()
    {
        resolutions = Screen.resolutions;
        LogoLoad();
        InitializeResolutions();
        InitializeQuality();
        InitializeVolume();
        InitializeAntiAliasing();
        InitializeTextureQuality();
        InitializeFullscreen();
        InitializeRefreshrate();
    }

    private void InitializeRefreshrate()
    {
        refreshrateDropdown.ClearOptions();
        List<string> refreshRateOptions = new List<string>();

        foreach (Resolution res in resolutions)
        {
            string refreshRateString = $"{res.refreshRateRatio.numerator} Hz";
            if (!refreshRateOptions.Contains(refreshRateString))
            {
                refreshRateOptions.Add(refreshRateString);
            }
        }

        refreshrateDropdown.AddOptions(refreshRateOptions);
        refreshrateDropdown.onValueChanged.AddListener(SetResolution);
    }


    public void SetRefreshRate(int refreshRateIndex)
    {
        int refreshRate = int.Parse(refreshrateDropdown.options[refreshRateIndex].text.Replace(" Hz", ""));
        int resolutionIndex = resolutionDropdown.value;
        string[] selectedResolution = resolutionDropdown.options[resolutionIndex].text.Split('x');
        int width = int.Parse(selectedResolution[0]);
        int height = int.Parse(selectedResolution[1]);

        Screen.SetResolution(width, height, Screen.fullScreen, refreshRate);
        PlayerPrefs.SetInt("RefreshRate", refreshRate);
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void LogoLoad()
    {
        
    }
    public void QuitApp()
    {
        Application.Quit();
    }
    private void InitializeResolutions()
    {
        resolutionDropdown.ClearOptions();
        List<string> resolutionOptions = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string resolutionString = $"{resolutions[i].width}x{resolutions[i].height}";
            resolutionOptions.Add(resolutionString);
        }

        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.onValueChanged.AddListener(SetResolution); // ✅ Now passes index
    }


    public void SetResolution(int resolutionIndex)
    {
        string[] resolutionText = resolutionDropdown.options[resolutionDropdown.value].text.Split('x');
        if (resolutionText.Length != 2 || !int.TryParse(resolutionText[0].Trim(), out int width) || !int.TryParse(resolutionText[1].Trim(), out int height))
        {
            Debug.LogError("Invalid resolution format: " + resolutionDropdown.options[resolutionDropdown.value].text);
            return;
        }

        string refreshRateText = refreshrateDropdown.options[refreshrateDropdown.value].text.Replace(" Hz", "").Trim();
        if (!int.TryParse(refreshRateText, out int refreshRate))
        {
            Debug.LogError("Invalid refresh rate format: " + refreshRateText);
            return;
        }

        Screen.SetResolution(width, height, fullscreenToggle.isOn ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed, new RefreshRate { numerator = (uint)refreshRate, denominator = 1 });
    }


    // Initialize quality dropdown
    private void InitializeQuality()
    {
        qualityDropdown.ClearOptions();
        string[] qualityNames = QualitySettings.names;
        List<string> options = new List<string>(qualityNames);

        qualityDropdown.AddOptions(options);
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityIndex", qualityIndex);
    }

    // Initialize Anti-Aliasing dropdown
    private void InitializeAntiAliasing()
    {
        List<string> options = new List<string> { "Off", "2x", "4x", "8x" };
        antiAliasingDropdown.ClearOptions();
        antiAliasingDropdown.AddOptions(options);

        int savedValue = PlayerPrefs.GetInt("AntiAliasing", 0); // Default to "Off"
        antiAliasingDropdown.value = savedValue;
        antiAliasingDropdown.RefreshShownValue();
    }

    public void SetAntiAliasing(int value)
    {
        int[] aaLevels = { 0, 2, 4, 8 };
        QualitySettings.antiAliasing = aaLevels[value];
        PlayerPrefs.SetInt("AntiAliasing", value);
    }

    // Initialize Texture Quality dropdown
    private void InitializeTextureQuality()
    {
        List<string> options = new List<string> { "Full Res", "Half Res", "Quarter Res", "Eighth Res" };
        textureQualityDropdown.ClearOptions();
        textureQualityDropdown.AddOptions(options);

        int savedValue = PlayerPrefs.GetInt("TextureQuality", 0); // Default to "Full Res"
        textureQualityDropdown.value = savedValue;
        textureQualityDropdown.RefreshShownValue();
    }

    public void SetTextureQuality(int value)
    {
        QualitySettings.globalTextureMipmapLimit = value; // 0 = Full, 1 = Half, 2 = Quarter, etc.
        PlayerPrefs.SetInt("TextureQuality", value);
    }

    // Initialize volume slider
    private void InitializeVolume()
    {
        float savedVolume = PlayerPrefs.GetFloat("Volume", 0.75f);
        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20);
        PlayerPrefs.SetFloat("Volume", volume);
    }

    // Initialize fullscreen toggle
    private void InitializeFullscreen()
    {
        bool isFullScreen = PlayerPrefs.GetInt("FullScreen", 1) == 1; // Default to fullscreen
        fullscreenToggle.isOn = isFullScreen;
        Screen.fullScreenMode = isFullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        PlayerPrefs.SetInt("FullScreen", isFullScreen ? 1 : 0);
    }
    public void SaveSettings()
    {
        Debug.Log("Resolution Index: " + resolutionDropdown.value);
        Debug.Log("Quality Index: " + qualityDropdown.value);
        Debug.Log("Anti Aliasing: " + antiAliasingDropdown.value);
        Debug.Log("Texture Quality: " + textureQualityDropdown.value);
        Debug.Log("Refresh Rate: " + refreshrateDropdown.options[refreshrateDropdown.value].text);
        Debug.Log("Volume: " + volumeSlider.value);
        Debug.Log("Full Screen: " + fullscreenToggle.isOn);

        PlayerPrefs.SetInt("ResolutionIndex", resolutionDropdown.value);
        PlayerPrefs.SetInt("QualityIndex", qualityDropdown.value);
        PlayerPrefs.SetInt("AntiAliasing", antiAliasingDropdown.value);
        PlayerPrefs.SetInt("TextureQuality", textureQualityDropdown.value);
        PlayerPrefs.SetInt("RefreshRate", int.Parse(refreshrateDropdown.options[refreshrateDropdown.value].text.Replace(" Hz", "")));
        PlayerPrefs.SetInt("FullScreen", fullscreenToggle.isOn ? 1 : 0);
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        PlayerPrefs.Save();
        Debug.Log("Settings Saved!");
    }



}


