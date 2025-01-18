using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject gameOverUI; 

    
    public void ShowGameOver()
    {
        gameOverUI.SetActive(true);  
        Time.timeScale = 0f;         
    }

    
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;  
        SceneManager.LoadScene("MainMenuScene");  
    }


    public void RestartLevel()
    {
        gameOverUI.SetActive(false);
        Time.timeScale = 1f; 
        PlayerStats.ResetStats();
        GameManager.GameIsOver = false; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    }

}
