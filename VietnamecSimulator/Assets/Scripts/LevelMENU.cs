using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMENU : MonoBehaviour
{

    public void loadScene(int SceneId)
    {
        string levelScene = "Scene" + SceneId;
        SceneManager.LoadScene((SceneId));

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
