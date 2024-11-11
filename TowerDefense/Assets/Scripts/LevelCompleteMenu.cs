using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelCompleteMenu : MonoBehaviour
{
    public GameObject levelCompleteUI;

    
    public Button nextLevelButton;
    public Button mainMenuButton;
    public Button restartButton;

    void Start()
    {
        
        levelCompleteUI.SetActive(false);

        
        nextLevelButton.onClick.AddListener(NextLevel);
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        restartButton.onClick.AddListener(RestartLevel);
    }

    public void ShowLevelComplete()
    {
       
        levelCompleteUI.SetActive(true);

        
        Time.timeScale = 0f;
    }

    private void NextLevel()
    {
        
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("No more levels!");
        }
    }

    private void ReturnToMainMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu"); 
    }

    private void RestartLevel()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void HideLevelComplete()
    {
        
        levelCompleteUI.SetActive(false);
        Time.timeScale = 1f; 
    }
}
