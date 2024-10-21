using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    public void PlayGame()
    {
        SceneManager.LoadScene("LVL1"); 
    }

    
    public void OpenOptions()
    {
        
        Debug.Log("Options Menu Opened");
    }

    
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting"); 
    }
}
