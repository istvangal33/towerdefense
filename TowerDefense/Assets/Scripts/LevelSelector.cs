using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    public void LoadLevel(int levelIndex)
    {
        string levelName = "LVL" + levelIndex;
        SceneManager.LoadScene(levelName);
    }
}
