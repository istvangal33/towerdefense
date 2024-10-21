using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool GameIsOver;
    public GameOver gameOverScript;

    void Start()
    {
        GameIsOver = false;
    }

    
    void Update()
    {
        if (GameIsOver)
            return;

        if (Input.GetKeyDown("e"))
        {
            EndGame();
        }

        if (PlayerStats.Lives <= 0)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        GameIsOver = true;
        gameOverScript.ShowGameOver(); 

       
        StopAllEnemies();
    }

    
    void StopAllEnemies()
    {
        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
        foreach (EnemyAI enemy in enemies)
        {
            
            UnityEngine.AI.NavMeshAgent agent = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null)
            {
                agent.isStopped = true; 
            }

            
            enemy.StopAllCoroutines(); 
            enemy.enabled = false;     
        }
    }
}
