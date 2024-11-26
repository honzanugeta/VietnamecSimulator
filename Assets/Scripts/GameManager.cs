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
    private bool PauseActive = false;
    
    // Start is called before the first frame update
    void Start()
    {      
        Cursor.visible = false;
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
    }


    private void TogglePause()
    {
        PauseActive = !PauseActive;
        PauseMenu.SetActive(PauseActive);
        Time.timeScale = PauseActive ? 0 : 1;
    }
}
