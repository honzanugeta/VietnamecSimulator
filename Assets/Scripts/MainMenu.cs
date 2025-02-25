using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider volumeSlider;
    
    [Header("Display Settings")]
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] TMP_Dropdown refreshRateDropdown;
    [SerializeField] Toggle fullscreenToggle;
    
    [Header("Graphics Settings")]
    [SerializeField] TMP_Dropdown qualityDropdown;
    [SerializeField] TMP_Dropdown antiAliasingDropdown;
    [SerializeField] TMP_Dropdown textureQualityDropdown;
    
    private Resolution[] resolutions;
    private Dictionary<string, List<int>> availableRefreshRates = new Dictionary<string, List<int>>();
    private bool isInitializing = true;
    
    void Start()
    {
        // First collect data about the system
        resolutions = Screen.resolutions;
        
        // Initialize UI components
        InitializeResolutions();
        InitializeRefreshRates();
        InitializeQuality();
        InitializeAntiAliasing();
        InitializeTextureQuality();
        InitializeFullscreen();
        InitializeVolume();
        
        // Load saved settings after initializing UI
        LoadSavedSettings();
        
        isInitializing = false;
    }
    
    // Load and apply all saved settings
    private void LoadSavedSettings()
    {
        // Load resolution
        int savedResIndex = PlayerPrefs.GetInt("ResolutionIndex", -1);
        if (savedResIndex >= 0 && savedResIndex < resolutionDropdown.options.Count)
        {
            resolutionDropdown.value = savedResIndex;
        }
        
        // Load refresh rate
        int savedRefreshRate = PlayerPrefs.GetInt("RefreshRate", -1);
        if (savedRefreshRate > 0)
        {
            SetRefreshRateDropdownToClosestValue(savedRefreshRate);
        }
        
        // Load quality
        int savedQuality = PlayerPrefs.GetInt("QualityIndex", -1);
        if (savedQuality >= 0 && savedQuality < qualityDropdown.options.Count)
        {
            qualityDropdown.value = savedQuality;
            QualitySettings.SetQualityLevel(savedQuality);
        }
        
        // Load anti-aliasing
        int savedAA = PlayerPrefs.GetInt("AntiAliasing", -1);
        if (savedAA >= 0 && savedAA < antiAliasingDropdown.options.Count)
        {
            antiAliasingDropdown.value = savedAA;
            int[] aaLevels = { 0, 2, 4, 8 };
            QualitySettings.antiAliasing = aaLevels[savedAA];
        }
        
        // Load texture quality
        int savedTexQuality = PlayerPrefs.GetInt("TextureQuality", -1);
        if (savedTexQuality >= 0 && savedTexQuality < textureQualityDropdown.options.Count)
        {
            textureQualityDropdown.value = savedTexQuality;
            QualitySettings.globalTextureMipmapLimit = savedTexQuality;
        }
        
        // Load fullscreen
        bool isFullScreen = PlayerPrefs.GetInt("FullScreen", 1) == 1;
        fullscreenToggle.isOn = isFullScreen;
        Screen.fullScreenMode = isFullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        
        // Load volume
        float savedVolume = PlayerPrefs.GetFloat("Volume", 0.75f);
        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);
        
        // Apply resolution and refresh rate
        if (savedResIndex >= 0)
        {
            ApplyCurrentResolutionAndRefreshRate();
        }
    }
    
    // Set the refresh rate dropdown to the closest available value
    private void SetRefreshRateDropdownToClosestValue(int targetRefreshRate)
    {
        string resolutionKey = resolutionDropdown.options[resolutionDropdown.value].text;
        
        if (availableRefreshRates.ContainsKey(resolutionKey))
        {
            var rates = availableRefreshRates[resolutionKey];
            
            int closestIndex = 0;
            int closestDiff = int.MaxValue;
            
            for (int i = 0; i < rates.Count; i++)
            {
                int diff = Mathf.Abs(rates[i] - targetRefreshRate);
                if (diff < closestDiff)
                {
                    closestDiff = diff;
                    closestIndex = i;
                }
            }
            
            // Set dropdown to closest value
            refreshRateDropdown.value = closestIndex;
        }
    }
    
    // Initialize resolution dropdown
    private void InitializeResolutions()
    {
        resolutionDropdown.ClearOptions();
        List<string> resolutionOptions = new List<string>();
        HashSet<string> uniqueResolutions = new HashSet<string>();
        
        foreach (Resolution res in resolutions)
        {
            string resolutionString = $"{res.width}x{res.height}";
            if (!uniqueResolutions.Contains(resolutionString))
            {
                uniqueResolutions.Add(resolutionString);
                resolutionOptions.Add(resolutionString);
            }
        }
        
        resolutionDropdown.AddOptions(resolutionOptions);
        
        // Set current resolution as default
        int currentResIndex = 0;
        string currentRes = $"{Screen.width}x{Screen.height}";
        
        for (int i = 0; i < resolutionOptions.Count; i++)
        {
            if (resolutionOptions[i] == currentRes)
            {
                currentResIndex = i;
                break;
            }
        }
        
        resolutionDropdown.value = currentResIndex;
        resolutionDropdown.RefreshShownValue();
        
        // Add listener after setting initial value
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
    }
    
    // Initialize refresh rate dropdown
    private void InitializeRefreshRates()
    {
        // First populate the available refresh rates dictionary
        foreach (Resolution res in resolutions)
        {
            string resKey = $"{res.width}x{res.height}";
            int refreshRate = (int)res.refreshRateRatio.numerator;
            
            if (!availableRefreshRates.ContainsKey(resKey))
            {
                availableRefreshRates[resKey] = new List<int>();
            }
            
            if (!availableRefreshRates[resKey].Contains(refreshRate))
            {
                availableRefreshRates[resKey].Add(refreshRate);
            }
        }
        
        // Sort the refresh rates for each resolution
        foreach (var key in availableRefreshRates.Keys)
        {
            availableRefreshRates[key].Sort();
        }
        
        // Initialize with current resolution's refresh rates
        UpdateRefreshRateDropdown();
        
        // Add listener
        refreshRateDropdown.onValueChanged.AddListener(OnRefreshRateChanged);
    }
    
    // Update refresh rate dropdown based on selected resolution
    private void UpdateRefreshRateDropdown()
    {
        string currentResolution = resolutionDropdown.options[resolutionDropdown.value].text;
        refreshRateDropdown.ClearOptions();
        
        List<string> refreshRateOptions = new List<string>();
        
        if (availableRefreshRates.ContainsKey(currentResolution))
        {
            foreach (int rate in availableRefreshRates[currentResolution])
            {
                refreshRateOptions.Add($"{rate} Hz");
            }
        }
        else
        {
            // Fallback if no data for current resolution
            refreshRateOptions.Add("60 Hz");
        }
        
        refreshRateDropdown.AddOptions(refreshRateOptions);
        
        // Set current refresh rate if possible
        if (availableRefreshRates.ContainsKey(currentResolution))
        {
            int currentRefreshRate = (int)Screen.currentResolution.refreshRateRatio.numerator;
            int closestIndex = 0;
            int closestDiff = int.MaxValue;
            
            for (int i = 0; i < availableRefreshRates[currentResolution].Count; i++)
            {
                int diff = Mathf.Abs(availableRefreshRates[currentResolution][i] - currentRefreshRate);
                if (diff < closestDiff)
                {
                    closestDiff = diff;
                    closestIndex = i;
                }
            }
            
            refreshRateDropdown.value = closestIndex;
        }
        
        refreshRateDropdown.RefreshShownValue();
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
        qualityDropdown.onValueChanged.AddListener(SetQuality);
    }
    
    // Initialize Anti-Aliasing dropdown
    private void InitializeAntiAliasing()
    {
        List<string> options = new List<string> { "Off", "2x", "4x", "8x" };
        antiAliasingDropdown.ClearOptions();
        antiAliasingDropdown.AddOptions(options);
        
        // Get current AA setting
        int currentAA = QualitySettings.antiAliasing;
        int dropdownIndex = 0;
        
        switch (currentAA)
        {
            case 0: dropdownIndex = 0; break;
            case 2: dropdownIndex = 1; break;
            case 4: dropdownIndex = 2; break;
            case 8: dropdownIndex = 3; break;
        }
        
        antiAliasingDropdown.value = dropdownIndex;
        antiAliasingDropdown.RefreshShownValue();
        antiAliasingDropdown.onValueChanged.AddListener(SetAntiAliasing);
    }
    
    // Initialize Texture Quality dropdown
    private void InitializeTextureQuality()
    {
        List<string> options = new List<string> { "Full Res", "Half Res", "Quarter Res", "Eighth Res" };
        textureQualityDropdown.ClearOptions();
        textureQualityDropdown.AddOptions(options);
        
        int currentTexLevel = QualitySettings.globalTextureMipmapLimit;
        textureQualityDropdown.value = currentTexLevel;
        textureQualityDropdown.RefreshShownValue();
        textureQualityDropdown.onValueChanged.AddListener(SetTextureQuality);
    }
    
    // Initialize volume slider
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
    
    // Initialize fullscreen toggle
    private void InitializeFullscreen()
    {
        fullscreenToggle.isOn = Screen.fullScreen;
        fullscreenToggle.onValueChanged.AddListener(SetFullScreen);
    }
    
    // Event handler for resolution dropdown change
    private void OnResolutionChanged(int index)
    {
        // Update refresh rate options when resolution changes
        UpdateRefreshRateDropdown();
        
        if (!isInitializing)
        {
            ApplyCurrentResolutionAndRefreshRate();
        }
    }
    
    // Event handler for refresh rate dropdown change
    private void OnRefreshRateChanged(int index)
    {
        if (!isInitializing)
        {
            ApplyCurrentResolutionAndRefreshRate();
        }
    }
    
    // Apply the currently selected resolution and refresh rate
    private void ApplyCurrentResolutionAndRefreshRate()
    {
        string resolutionText = resolutionDropdown.options[resolutionDropdown.value].text;
        string[] resParts = resolutionText.Split('x');
        
        if (resParts.Length != 2 || !int.TryParse(resParts[0], out int width) || !int.TryParse(resParts[1], out int height))
        {
            Debug.LogError("Invalid resolution format: " + resolutionText);
            return;
        }
        
        string refreshRateText = refreshRateDropdown.options[refreshRateDropdown.value].text.Replace(" Hz", "");
        if (!int.TryParse(refreshRateText, out int refreshRate))
        {
            Debug.LogError("Invalid refresh rate format: " + refreshRateText);
            return;
        }
        
        // Apply resolution, refresh rate, and fullscreen mode
        Screen.SetResolution(
            width, 
            height, 
            fullscreenToggle.isOn ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed,
            new RefreshRate { numerator = (uint)refreshRate, denominator = 1 }
        );
        
        // Save resolution and refresh rate to player prefs
        PlayerPrefs.SetInt("ResolutionIndex", resolutionDropdown.value);
        PlayerPrefs.SetInt("RefreshRate", refreshRate);
    }
    
    // Set quality level
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityIndex", qualityIndex);
    }
    
    // Set anti-aliasing level
    public void SetAntiAliasing(int value)
    {
        int[] aaLevels = { 0, 2, 4, 8 };
        QualitySettings.antiAliasing = aaLevels[value];
        PlayerPrefs.SetInt("AntiAliasing", value);
    }
    
    // Set texture quality
    public void SetTextureQuality(int value)
    {
        QualitySettings.globalTextureMipmapLimit = value;
        PlayerPrefs.SetInt("TextureQuality", value);
    }
    
    // Set volume
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20);
        PlayerPrefs.SetFloat("Volume", volume);
    }
    
    // Set fullscreen mode
    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreenMode = isFullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        PlayerPrefs.SetInt("FullScreen", isFullScreen ? 1 : 0);
        
        // Re-apply resolution to ensure it works with the new screen mode
        ApplyCurrentResolutionAndRefreshRate();
    }
    
    // Save all settings
    public void SaveSettings()
    {
        // All individual settings are already saved in their respective methods
        // This ensures PlayerPrefs are written to disk
        PlayerPrefs.Save();
        Debug.Log("Settings Saved!");
    }
    
    // Exit the application
    public void QuitApp()
    {
        SaveSettings();
        Application.Quit();
    }
}