using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField]  RectTransform Logo;
    [SerializeField]  float topposy, middleposy;
    [SerializeField]  float tweenDuration;
    

    
    // Start is called before the first frame update
    void Start()
    {
        LogoLoad();
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
   
}
