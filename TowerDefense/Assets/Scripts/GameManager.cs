using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static bool GameIsOver;
    public GameOver gameOverScript;

    public static GameManager Instance { get; private set; }

    public int CurrentLevel { get; private set; }
    private string filePath = "GameData.csv";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        
        filePath = Path.Combine(Application.dataPath, "GameData.csv");
        Debug.Log($"GameData fajl helye: {filePath}");
    }


    void Start()
    {
        GameIsOver = false;

        
        CurrentLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        Debug.Log("Current Level: " + CurrentLevel);

        
        InitializeCSVFile();

        
        LogTowerDataToCSV("GameStart");
    }


    void InitializeCSVFile()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "CurrentLevel;WaveNumber;StartMoney;CurrentLives;TotalEnemiesSpawned;Buggy%;Helicopter%;Hovertank%;Coverage;CoverageByTower;Timestamp\n");
            Debug.Log("CSV fajl inicializalva: " + filePath);

            SaveInitialGameDataToCSV();
        }
    }

    void SaveInitialGameDataToCSV()
    {
        string currentLevel = CurrentLevel.ToString();
        string startMoney = PlayerStats.startMoney.ToString();
        string startLives = PlayerStats.Lives.ToString();

        string line = $"{currentLevel},0,{startMoney},{startLives}";
        File.AppendAllText(filePath, line + "\n");

        Debug.Log($"Kezdeti adatok mentve: Level={currentLevel}, StartMoney={startMoney}, StartLives={startLives}");
    }

    public void SaveGameDataToCSV(int waveNumber, int totalEnemiesSpawned, Dictionary<EnemyType, int> enemyTypeCounts)
    {
        string currentLevel = CurrentLevel.ToString();
        string wave = waveNumber.ToString();
        string currentMoney = PlayerStats.Money.ToString();
        string currentLives = PlayerStats.Lives.ToString();

        float towerCoverage = FindObjectOfType<GridCoverageManager>()?.GetCurrentCoverage() ?? 0f;
        float coverageByTower = FindObjectOfType<GridCoverageManager>()?.GetCoverageByTower() ?? 0f;

        int totalEnemies = Mathf.Max(1, totalEnemiesSpawned);
        float buggyPercentage = (enemyTypeCounts[EnemyType.Buggy] / (float)totalEnemies) * 100f;
        float helicopterPercentage = (enemyTypeCounts[EnemyType.Helicopter] / (float)totalEnemies) * 100f;
        float hovertankPercentage = (enemyTypeCounts[EnemyType.Hovertank] / (float)totalEnemies) * 100f;

        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "CurrentLevel;WaveNumber;StartMoney;CurrentLives;TotalEnemiesSpawned;Buggy%;Helicopter%;Hovertank%;Coverage;CoverageByTower;Timestamp\n");
        }

        string line = $"{currentLevel};{wave};{currentMoney};{currentLives};{totalEnemiesSpawned};{buggyPercentage:F2};{helicopterPercentage:F2};{hovertankPercentage:F2};{towerCoverage:F2};{coverageByTower:F2};{timestamp}";
        File.AppendAllText(filePath, line + "\n");

        Debug.Log($"Adatok mentve: Level={currentLevel}, Wave={wave}, Money={currentMoney}, Lives={currentLives}, TotalEnemies={totalEnemiesSpawned}, Coverage={towerCoverage:F2}, CoverageByTower={coverageByTower:F2}, Timestamp={timestamp}");
    }





    public void LogPlayerMoneyAtWaveStart(int waveNumber)
    {
        Debug.Log($"Wave {waveNumber} Start - Player Money: {PlayerStats.Money}");
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

        if (Input.GetKeyDown(KeyCode.F9))
        {
            Time.timeScale = 0.5f;
            
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Time.timeScale = 1f; 
            
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Time.timeScale = 2f; 
            
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            Time.timeScale = 3f;
            
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            Time.timeScale = 4f;
            
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Time.timeScale = 5f;
            
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            Time.timeScale = 6f;

        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            Time.timeScale = 7f;

        }
        if (Input.GetKeyDown(KeyCode.F8))
        {
            Time.timeScale = 8f;

        }
    }


    void EndGame()
    {
        GameIsOver = true;

        gameOverScript.ShowGameOver();
        StopAllEnemies();
        LogTowerStats();

        WaveSpawner waveSpawner = FindObjectOfType<WaveSpawner>();
        if (waveSpawner != null)
        {
            float towerCoverage = FindObjectOfType<GridCoverageManager>()?.GetCurrentCoverage() ?? 0f;

            SaveGameDataToCSV(
                waveSpawner.waveNumber,
                waveSpawner.totalEnemiesSpawned,
                new Dictionary<EnemyType, int>(waveSpawner.enemyTypeCounts)
            );

            Debug.Log($"Tower coverage at game over: {towerCoverage}%");
        }
        else
        {
            Debug.LogError("WaveSpawner instance not found!");
        }

       
        if (!GameIsOver && waveSpawner.waveNumber >= waveSpawner.maxWaves)
        {
            AwardBonusLife(5);
        }
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

    public void LogPlayerMoneyChange(int oldMoney, int newMoney)
    {
        int moneyChange = newMoney - oldMoney;
        if (moneyChange > 0)
        {
            Debug.Log($"Player earned {moneyChange} money. Current money: {newMoney}");
        }
        else if (moneyChange < 0)
        {
            Debug.Log($"Player spent {Mathf.Abs(moneyChange)} money. Current money: {newMoney}");
        }
    }

    public void LogPlayerMoneyAtWaveEnd(int waveNumber)
    {
        Debug.Log($"Wave {waveNumber} ended. Player money: {PlayerStats.Money}");
    }

    public void LogWaveNumber(int waveNumber)
    {
        Debug.Log($"Wave {waveNumber} ended.");
    }

    public void LogPlayerLivesChange(int oldLives, int newLives)
    {
        int livesChange = newLives - oldLives;
        if (livesChange < 0)
        {
            Debug.Log($"Player lost {Mathf.Abs(livesChange)} lives. Current lives: {newLives}");
        }
        else if (livesChange > 0)
        {
            Debug.Log($"Player gained {livesChange} lives. Current lives: {newLives}");
        }
    }

    public void LogPlayerLivesAtWaveEnd()
    {
        Debug.Log($"Wave ended. Player lives: {PlayerStats.Lives}");
    }

    public void LogTowerStats()
    {
        Tower[] towers = FindObjectsOfType<Tower>();
        Debug.Log($"Total towers: {towers.Length}");

        foreach (Tower tower in towers)
        {
            int currentCost = tower.node?.turretBlueprint?.GetSellAmount(tower.currentLevel) * 2 ?? 0;

            Debug.Log($"Type: {tower.towerType}, Position: {tower.transform.position}, Range: {tower.range}, FireRate: {tower.fireRate}, Level: {tower.currentLevel}, Cost: {currentCost}");

            LogTowerDataToCSV("Update", tower);
        }
    }

    public void LogTowerDataToCSV(string action, Tower tower = null)
    {
        string filePath = Path.Combine(Application.dataPath, "TowerData.csv");

        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "CurrentLevel;WaveNumber;TowerID;TowerType;Action;Level;Range;FireRate;Cost;Timestamp\n");
        }

        int currentLevel = CurrentLevel;
        int waveNumber = FindObjectOfType<WaveSpawner>()?.waveNumber ?? 0;

        string towerID = tower != null ? tower.GetInstanceID().ToString() : "NaN";
        string towerType = tower != null ? tower.towerType.ToString() : "NaN";
        string level = tower != null ? tower.currentLevel.ToString() : "NaN";
        string range = tower != null ? tower.range.ToString("F2") : "NaN";
        string fireRate = tower != null ? tower.fireRate.ToString("F2") : "NaN";
        string currentCost = tower != null && tower.node != null ? (tower.node.turretBlueprint?.GetSellAmount(tower.currentLevel) * 2).ToString() : "NaN";

        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string line = $"{currentLevel};{waveNumber};{towerID};{towerType};{action};{level};{range};{fireRate};{currentCost};{timestamp}";

        File.AppendAllText(filePath, line + "\n");
        Debug.Log($"Tower data mentve: {line}");
    }


    public void AwardBonusLife(int bonusLives)
    {
        int maxLives = 15;
        int currentLives = PlayerStats.Lives;

        if (currentLives >= maxLives)
        {
            Debug.Log("A plyer mar max elettel rendelkezik.");
            return;
        }

        PlayerStats.Lives = Mathf.Min(currentLives + bonusLives, maxLives);
        Debug.Log($"bonusz eletek hozzaadva: {bonusLives}. current lives: {PlayerStats.Lives}");
    }


}
