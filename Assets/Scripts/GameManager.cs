using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using MouseButton = UnityEngine.UIElements.MouseButton;

public class GameManager : MonoBehaviour
{
    [SerializeField] public GameObject PauseMenu;
    [SerializeField] public GameObject PlayerCamera;
    [SerializeField] public MonoBehaviour FirstPersonController;
    [SerializeField] public MonoBehaviour PlayerMovementScript;
    private bool PauseActive = false;
    
    // Start is called before the first frame update
    void Start()
    {      
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void QuitGame()
    {
    Application.Quit();
    Debug.Log("quitting");
    }


    private void TogglePause()
    {
        PauseActive = !PauseActive;
        PauseMenu.SetActive(PauseActive);
        Time.timeScale = PauseActive ? 0 : 1;
        if (PauseActive)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None; // Unlock the cursor
            if (FirstPersonController != null) FirstPersonController.enabled = false; // Disable camera movement
            if (PlayerMovementScript != null) PlayerMovementScript.enabled = false;  // Disable player movement
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked; // Lock the cursor back
            if (FirstPersonController != null) FirstPersonController.enabled = true; // Enable camera movement
            if (PlayerMovementScript != null) PlayerMovementScript.enabled = true;  // Enable player movement
        }
    }
}
