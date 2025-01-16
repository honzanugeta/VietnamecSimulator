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

        // Populate refresh rates based on the currently selected resolution
        int resolutionIndex = resolutionDropdown.value;
        string[] selectedResolution = resolutionDropdown.options[resolutionIndex].text.Split('x');
        int width = int.Parse(selectedResolution[0]);
        int height = int.Parse(selectedResolution[1]);

        HashSet<int> refreshRates = new HashSet<int>();
        foreach (var res in resolutions)
        {
            if (res.width == width && res.height == height)
            {
                refreshRates.Add(res.refreshRate);
            }
        }

        // Sort refresh rates and add to dropdown
        List<int> sortedRefreshRates = new List<int>(refreshRates);
        sortedRefreshRates.Sort();

        List<string> options = new List<string>();
        int currentRefreshRateIndex = 0;
        for (int i = 0; i < sortedRefreshRates.Count; i++)
        {
            options.Add(sortedRefreshRates[i] + " Hz");
            if (sortedRefreshRates[i] == Screen.currentResolution.refreshRate)
            {
                currentRefreshRateIndex = i;
            }
        }

        refreshrateDropdown.AddOptions(options);
        refreshrateDropdown.value = currentRefreshRateIndex;
        refreshrateDropdown.RefreshShownValue();
    }


    public void SetRefreshRate(int refreshRateIndex)
    {
        int refreshRate = int.Parse(refreshrateDropdown.options[refreshRateIndex].text.Replace(" Hz", ""));
        int resolutionIndex = resolutionDropdown.value;
        string[] selectedResolution = resolutionDropdown.options[resolutionIndex].text.Split('x');
        int width = int.Parse(selectedResolution[0]);
        int height = int.Parse(selectedResolution[1]);

        Screen.SetResolution(width, height, Screen.fullScreen, refreshRate);
        PlayerPrefs.SetInt("RefreshRate", refreshRateIndex);
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
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        // Collect unique resolutions
        HashSet<string> uniqueResolutions = new HashSet<string>();
        List<string> resolutionOptions = new List<string>();
        int currentResolutionIndex = 0;

        foreach (var res in resolutions)
        {
            string resolutionString = $"{res.width}x{res.height}";
            if (uniqueResolutions.Add(resolutionString))
            {
                resolutionOptions.Add(resolutionString);
            }

            if (res.width == Screen.currentResolution.width && res.height == Screen.currentResolution.height)
            {
                currentResolutionIndex = resolutionOptions.IndexOf(resolutionString);
            }
        }

        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }


    public void SetResolution(int resolutionIndex)
    {
        string[] selectedResolution = resolutionDropdown.options[resolutionIndex].text.Split('x');
        int width = int.Parse(selectedResolution[0]);
        int height = int.Parse(selectedResolution[1]);

        // Get currently selected refresh rate
        int refreshRate = int.Parse(refreshrateDropdown.options[refreshrateDropdown.value].text.Replace(" Hz", ""));

        Screen.SetResolution(width, height, Screen.fullScreen, refreshRate);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
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
        QualitySettings.antiAliasing = (int)Mathf.Pow(2, value); // 0 -> Off, 1 -> 2x, 2 -> 4x, 3 -> 8x
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
        audioMixer.SetFloat("Volume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("Volume", volume);
    }

    // Initialize fullscreen toggle
    private void InitializeFullscreen()
    {
        bool isFullScreen = PlayerPrefs.GetInt("FullScreen", 1) == 1; // Default to fullscreen
        fullscreenToggle.isOn = isFullScreen;
        Screen.fullScreen = isFullScreen;
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
        PlayerPrefs.SetString("RefreshRate", refreshrateDropdown.options[refreshrateDropdown.value].text);
        PlayerPrefs.SetInt("FullScreen", fullscreenToggle.isOn ? 1 : 0);
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        PlayerPrefs.Save();
        Debug.Log("Settings Saved!");
    }



}


