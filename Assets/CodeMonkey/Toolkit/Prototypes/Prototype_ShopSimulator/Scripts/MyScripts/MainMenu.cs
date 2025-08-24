using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button plaButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        plaButton.onClick.AddListener(OnPlayButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
    }

    private void OnPlayButtonClicked()
    {
        SceneManager.LoadScene("ShopMain");
    }

    private void OnQuitButtonClicked()
    {

        Application.Quit();
    }
}
