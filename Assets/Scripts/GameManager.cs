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
    [SerializeField] private GameObject centerDot;
    [SerializeField] public GameObject OrderingTablet;
    [SerializeField] private GameObject inventoryUI;
    private bool PauseActive = false;
    
    // Start is called before the first frame update
    void Start()
    {      
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (centerDot != null)
            centerDot.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleOrderingTablet();
        }
    }

    public void QuitGame()
    {
    Application.Quit();
    Debug.Log("quitting");
    }
    
    public void ToggleOrderingTablet()
    {
        // Check if OrderingTablet is active and toggle its state
        bool isActive = OrderingTablet.activeSelf; // Get the current active state
        OrderingTablet.SetActive(!isActive);       // Toggle the active state

        // Update the OrderingTabletActive state
        bool OrderingTabletActive = !isActive;

        // Enable/disable related components based on the new state
        if (FirstPersonController != null) 
            FirstPersonController.enabled = !OrderingTabletActive;
        if (PlayerMovementScript != null) 
            PlayerMovementScript.enabled = !OrderingTabletActive;
        if (centerDot != null) 
            centerDot.SetActive(!OrderingTabletActive);

        // Enable cursor visibility and unlock it when the tablet is active
        Cursor.visible = OrderingTabletActive;
        Cursor.lockState = OrderingTabletActive ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void ToggleInventory()
    {
        bool isActive = inventoryUI.activeSelf; // Get the current active state
        inventoryUI.SetActive(!isActive);       // Toggle the active state

        // Enable/disable player movement and camera while inventory is open
        if (FirstPersonController != null) 
            FirstPersonController.enabled = isActive;
        if (PlayerMovementScript != null) 
            PlayerMovementScript.enabled = isActive;
        if (centerDot != null) 
            centerDot.SetActive(isActive);

        // Enable cursor visibility and unlock it when inventory is active
        Cursor.visible = !isActive;
        Cursor.lockState = !isActive ? CursorLockMode.None : CursorLockMode.Locked;
    }



    public void TogglePause()
    {
        PauseActive = !PauseActive;
        PauseMenu.SetActive(PauseActive);
        Time.timeScale = PauseActive ? 0 : 1;
        if (PauseActive)
        {
            if (OrderingTablet != null) OrderingTablet.SetActive(false);
            if (inventoryUI != null) inventoryUI.SetActive(false);
                
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None; // Unlock the cursor
            if (FirstPersonController != null) FirstPersonController.enabled = false; // Disable camera movement
            if (PlayerMovementScript != null) PlayerMovementScript.enabled = false; 
            if (centerDot != null) centerDot.SetActive(false);
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked; // Lock the cursor back
            if (FirstPersonController != null) FirstPersonController.enabled = true; // Enable camera movement
            if (PlayerMovementScript != null) PlayerMovementScript.enabled = true;  // Enable player movement
            if (centerDot != null) centerDot.SetActive(true); 
        }
    }
}
