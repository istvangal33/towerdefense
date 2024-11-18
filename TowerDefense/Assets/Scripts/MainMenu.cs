using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;

    private void Start()
    {
        ShowMainMenu();
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayMainMenuMusic();
        }
    }

    public void OpenOptions()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }

    public void PlayGame()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopMusic();
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelSelector");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting");
    }

    private void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }
}
