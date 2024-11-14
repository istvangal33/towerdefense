using UnityEngine.SceneManagement;
using UnityEngine;

public class GameCompleteMenu : MonoBehaviour
{
    public GameObject GameCompleteUI;

    public void ShowGameComplete()
    {
        GameCompleteUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenuScene");
    }
}
