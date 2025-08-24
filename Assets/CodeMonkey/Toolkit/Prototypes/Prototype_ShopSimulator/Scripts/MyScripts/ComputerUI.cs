using CodeMonkey.Toolkit.ShopSimulatorDemo;
using System;
using UnityEngine;

public class ComputerUI : MonoBehaviour
{
    [SerializeField] private Computer computer;
    [SerializeField] private GameObject computerUI;
    [SerializeField] private GameObject centerDot;

    private void Start()
    {
        computer.OnInteract += HandleComputerInteract;

        computerUI.SetActive(false);
    }

    private void HandleComputerInteract(bool yes)
    {
        Time.timeScale = yes ? 0f : 1f; // Pause the game when interacting with the computer

        centerDot.SetActive(!yes); 
        computerUI.SetActive(yes);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // If the computer UI is open, close it
            if (computerUI.activeSelf)
            {
                computer.Interact(IInteractable.InteractAction.InteractWithComputer, null);
            }
        }
    }
}
