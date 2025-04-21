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
    [SerializeField] private GameObject TabletPrefab;
    [SerializeField] private Transform TabletSpawningPoint;

    private bool PauseActive = false;
    
    private GameObject spawnedTablet;

    
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

        // Spawn the tablet with swoosh animation if activating
        if (OrderingTabletActive && spawnedTablet == null)
        {
            StartCoroutine(DelayedSpawnTabletWithSwoosh(0.5f)); // Add a delay of 0.5 seconds
        }
        else if (!OrderingTabletActive && spawnedTablet != null)
        {
            Destroy(spawnedTablet); // Destroy the tablet if deactivating
            spawnedTablet = null;
        }
    }
    
    private IEnumerator DelayedSpawnTabletWithSwoosh(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay

        // Spawn the prefab with the specified rotation
        spawnedTablet = Instantiate(TabletPrefab, TabletSpawningPoint.position, Quaternion.Euler(1, -7, -60 ));
    
        // Set the initial scale
        spawnedTablet.transform.localScale = new Vector3(0.25f, 0.25f, 0.400000006f);
    }

    private IEnumerator SwooshInAnimation(GameObject tablet)
    {
        float duration = 0.5f; // Animation duration
        float elapsedTime = 0f;

        Vector3 targetScale = Vector3.one * 0.25f; // Target scale

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            // Interpolate scale
            tablet.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, progress);
            yield return null;
        }

        tablet.transform.localScale = targetScale; // Ensure final scale is set
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
