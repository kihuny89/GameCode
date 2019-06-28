using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ChangeScenes : MonoBehaviour
{
    public string nextSceneName;

    public void StartStage()
    {      
            SceneManager.LoadScene("Main");
       
    }

    public void GameEnd()
    {
        Application.Quit();
    }

    void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
