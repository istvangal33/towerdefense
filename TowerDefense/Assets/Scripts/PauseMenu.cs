using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject mainMenuConfirmButton; 
    [SerializeField] GameObject restartConfirmButton; 
    private bool isPaused = false;
    private bool isPlayerAlive = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume(); 
            }
            else
            {
                Pause(); 
            }
        }
    }



    public void Pause()
    {
        if (!isPaused && isPlayerAlive)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f; 
            isPaused = true;
            mainMenuConfirmButton.SetActive(false); 
            restartConfirmButton.SetActive(false);
        }
    }

    
    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;       
        isPaused = false;           
    }


    
    public void OnMainMenuButton()
    {
        mainMenuConfirmButton.SetActive(true);  
        restartConfirmButton.SetActive(false);  
    }

    
    public void OnRestartButton()
    {
        restartConfirmButton.SetActive(true);   
        mainMenuConfirmButton.SetActive(false); 
    }

    
    public void OnMainMenuConfirm()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenuScene"); 
        CloseConfirm(); 
    }

    
    public void OnRestartConfirm()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
        CloseConfirm(); 
    }

    
    public void CloseConfirm()
    {
        mainMenuConfirmButton.SetActive(false); 
        restartConfirmButton.SetActive(false);  
    }

    
    public void PlayerDied()
    {
        isPlayerAlive = false;
    }

    
    public void PlayerRespawned()
    {
        isPlayerAlive = true;
    }
}
