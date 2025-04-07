using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteMenu : MonoBehaviour
{
    public GameObject levelCompleteUI;

    public void ShowLevelComplete()
    {
        levelCompleteUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void NextLevel()
    {
        
        PlayerStats.Lives += 5;

        
        if (PlayerStats.Lives > 15)
        {
            PlayerStats.Lives = 15;
        }

        
        int bonusMoney = 0;
        if (PlayerStats.Lives >= 6 && PlayerStats.Lives <= 10)
        {
            bonusMoney = 8;
        }
        else if (PlayerStats.Lives >= 11 && PlayerStats.Lives <= 14)
        {
            bonusMoney = 6;
        }
        else if (PlayerStats.Lives == 15) 
        {
            bonusMoney = 4;
        }

        
        PlayerStats.Money = 20 + bonusMoney; 

        Debug.Log($"A player {bonusMoney} bonuszpenzt kapott. Current money: {PlayerStats.Money}");

        
        Time.timeScale = 1f;

        
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogError("Nincs tobb palya!");
            GoToMainMenu();
        }
    }





    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}