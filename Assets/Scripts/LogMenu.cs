using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] GameObject logPanel;
    [SerializeField] TextMeshProUGUI logTextArea;
    [SerializeField] ScrollRect logScrollRect;
    [SerializeField] TMP_Dropdown logFilterDropdown;
    [SerializeField] Button clearButton;
    [SerializeField] Toggle autoScrollToggle;
    
    [Header("Keyboard Settings")]
    [SerializeField] KeyCode toggleKey = KeyCode.F1; // Default to F1 to toggle log panel
    [SerializeField] bool useAltModifier = false;
    [SerializeField] bool useCtrlModifier = false;
    [SerializeField] bool useShiftModifier = false;
    
    [Header("Log Settings")]
    [SerializeField] int maxLogEntries = 100;
    [SerializeField] Color infoColor = Color.white;
    [SerializeField] Color warningColor = Color.yellow;
    [SerializeField] Color errorColor = Color.red;
    
    private Queue<LogEntry> logEntries = new Queue<LogEntry>();
    private bool isLogPanelVisible = false;
    private LogType currentLogFilter = LogType.All;
    
    private enum LogType
    {
        All,
        Info,
        Warning,
        Error
    }
    
    private class LogEntry
    {
        public string Message;
        public LogType Type;
        public DateTime Timestamp;
        
        public LogEntry(string message, LogType type)
        {
            Message = message;
            Type = type;
            Timestamp = DateTime.Now;
        }
    }
    
    void Start()
    {
        // Hide log panel initially
        logPanel.SetActive(false);
        
        // Setup dropdown
        InitializeFilterDropdown();
        
        // Add listeners
        if (clearButton != null)
            clearButton.onClick.AddListener(ClearLogs);
        
        // Register for Unity logs
        Application.logMessageReceived += HandleUnityLog;
        
        // Log initial message
        LogInfo("Log system initialized. Press " + GetKeyComboString() + " to toggle log panel.");
    }
    
    private string GetKeyComboString()
    {
        string combo = "";
        if (useCtrlModifier) combo += "Ctrl+";
        if (useAltModifier) combo += "Alt+";
        if (useShiftModifier) combo += "Shift+";
        combo += toggleKey.ToString();
        return combo;
    }
    
    void Update()
    {
        // Check for keyboard input
        bool modifiersMatch = true;
        
        if (useCtrlModifier && !Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
            modifiersMatch = false;
            
        if (useAltModifier && !Input.GetKey(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.RightAlt))
            modifiersMatch = false;
            
        if (useShiftModifier && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
            modifiersMatch = false;
            
        if (modifiersMatch && Input.GetKeyDown(toggleKey))
        {
            ToggleLogPanel();
            LogInfo("Log panel toggled with keyboard shortcut");
        }
    }
    
    void OnDestroy()
    {
        // Unregister from Unity logs when destroyed
        Application.logMessageReceived -= HandleUnityLog;
    }
    
    private void InitializeFilterDropdown()
    {
        if (logFilterDropdown != null)
        {
            logFilterDropdown.ClearOptions();
            logFilterDropdown.AddOptions(new List<string> { "All", "Info", "Warning", "Error" });
            logFilterDropdown.onValueChanged.AddListener(SetLogFilter);
        }
    }
    
    // Handle Unity's internal logs
    private void HandleUnityLog(string logString, string stackTrace, UnityEngine.LogType type)
    {
        switch (type)
        {
            case UnityEngine.LogType.Error:
            case UnityEngine.LogType.Exception:
            case UnityEngine.LogType.Assert:
                AddLogEntry(logString, LogType.Error);
                break;
            case UnityEngine.LogType.Warning:
                AddLogEntry(logString, LogType.Warning);
                break;
            default:
                AddLogEntry(logString, LogType.Info);
                break;
        }
    }
    
    // Public methods for custom logging
    public void LogInfo(string message)
    {
        AddLogEntry(message, LogType.Info);
    }
    
    public void LogWarning(string message)
    {
        AddLogEntry(message, LogType.Warning);
    }
    
    public void LogError(string message)
    {
        AddLogEntry(message, LogType.Error);
    }
    
    private void AddLogEntry(string message, LogType type)
    {
        // Add new entry
        LogEntry entry = new LogEntry(message, type);
        logEntries.Enqueue(entry);
        
        // Maintain max log size
        if (logEntries.Count > maxLogEntries)
        {
            logEntries.Dequeue();
        }
        
        // Update UI if panel is visible
        if (isLogPanelVisible)
        {
            RefreshLogDisplay();
        }
    }
    
    private void RefreshLogDisplay()
    {
        if (logTextArea == null) return;
        
        logTextArea.text = "";
        
        foreach (LogEntry entry in logEntries)
        {
            // Skip entries that don't match the current filter
            if (currentLogFilter != LogType.All && entry.Type != currentLogFilter)
                continue;
                
            // Format timestamp
            string timestamp = entry.Timestamp.ToString("HH:mm:ss");
            
            // Determine text color
            string colorTag = GetColorHexForLogType(entry.Type);
            
            // Add formatted log entry
            logTextArea.text += $"[{timestamp}] <color={colorTag}>{entry.Message}</color>\n";
        }
        
        // Auto-scroll to bottom if enabled
        if (autoScrollToggle != null && autoScrollToggle.isOn && logScrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            logScrollRect.normalizedPosition = new Vector2(0, 0);
        }
    }
    
    private string GetColorHexForLogType(LogType type)
    {
        switch (type)
        {
            case LogType.Error:
                return "#" + ColorUtility.ToHtmlStringRGB(errorColor);
            case LogType.Warning:
                return "#" + ColorUtility.ToHtmlStringRGB(warningColor);
            default:
                return "#" + ColorUtility.ToHtmlStringRGB(infoColor);
        }
    }
    
    public void ToggleLogPanel()
    {
        isLogPanelVisible = !isLogPanelVisible;
        logPanel.SetActive(isLogPanelVisible);
        
        if (isLogPanelVisible)
        {
            RefreshLogDisplay();
        }
    }
    
    public void SetLogFilter(int filterIndex)
    {
        currentLogFilter = (LogType)filterIndex;
        RefreshLogDisplay();
    }
    
    public void ClearLogs()
    {
        logEntries.Clear();
        RefreshLogDisplay();
    }
}
