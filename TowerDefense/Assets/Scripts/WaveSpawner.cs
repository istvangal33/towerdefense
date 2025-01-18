using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

public class WaveSpawner : MonoBehaviour
{
    public GameObject[] easyEnemies;
    public GameObject[] mediumEnemies;
    public GameObject[] hardEnemies;
    public Transform spawnPoint;
    public float waveDuration = 10f;
    private float countdown = 0f;
    public int waveNumber = 1;
    public int maxWaves = 5;
    private bool firstWave = true;

    public LevelCompleteMenu levelCompleteMenu;
    public GameCompleteMenu gameCompleteUI;

    public Image waveProgressBarForeground;
    public TextMeshProUGUI waveNumberText;

    private List<float> spawnIntervals = new List<float>();

    private bool gameOver = false;

    public int totalEnemiesSpawned = 0;
    public Dictionary<EnemyType, int> enemyTypeCounts = new Dictionary<EnemyType, int>
    {
        { EnemyType.Buggy, 0 },
        { EnemyType.Helicopter, 0 },
        { EnemyType.Hovertank, 0 }
    };


    void LogWaveData()
    {
        Debug.Log($"Wave {waveNumber} Summary:");
        Debug.Log($"Total enemies spawned: {totalEnemiesSpawned}");
        foreach (var type in enemyTypeCounts)
        {
            Debug.Log($"{type.Key}: {type.Value}");
        }

        
        GameManager.Instance.SaveGameDataToCSV(
            waveNumber,
            totalEnemiesSpawned,
            new Dictionary<EnemyType, int>(enemyTypeCounts) 
        );

        
        totalEnemiesSpawned = 0;
        foreach (var key in enemyTypeCounts.Keys.ToList())
        {
            enemyTypeCounts[key] = 0;
        }

        spawnIntervals.Clear(); 
    }



    void Start()
    {
        UpdateWaveText();
        waveProgressBarForeground.fillAmount = 0f;
        StartCoroutine(UpdateProgressBar());
        StartCoroutine(ManageWave());
        InitializeTowerCoverage();
    }



    void InitializeTowerCoverage()
    {
        Tower[] towers = FindObjectsOfType<Tower>();
        foreach (Tower tower in towers)
        {
            tower.CalculateCoverage();
        }
    }


    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(1.5f);
        countdown = 10f;
    }

    void Update()
    {
        if (gameOver)
            return;

        countdown -= Time.deltaTime;
    }

    IEnumerator SpawnEnemiesAndProgressBar()
    {
        Debug.Log("SpawnEnemiesAndProgressBar coroutine started.");
        float elapsedTime = 0f;
        waveProgressBarForeground.fillAmount = 0f;

        GameObject[] enemyPool = DetermineEnemyPool();

        if (enemyPool.Length == 0)
        {
            Debug.LogError("Nincs ellenség a poolban!");
            yield break;
        }

        while (elapsedTime < waveDuration)
        {
            float remainingTime = waveDuration - elapsedTime;

            
            float spawnDelay = UnityEngine.Random.Range(0.5f, Mathf.Min(remainingTime, 2));
            StartCoroutine(SpawnEnemyWithDelay(enemyPool[UnityEngine.Random.Range(0, enemyPool.Length)], spawnDelay));

            elapsedTime += spawnDelay;
            waveProgressBarForeground.fillAmount = Mathf.Clamp01(elapsedTime / waveDuration);

            yield return new WaitForSeconds(spawnDelay);
        }

        
        waveProgressBarForeground.fillAmount = 1f;
        Debug.Log("SpawnEnemiesAndProgressBar coroutine ended.");
    }


    IEnumerator ManageWave()
    {
        while (waveNumber <= maxWaves)
        {
            GameManager.Instance.LogPlayerMoneyAtWaveStart(waveNumber);

            GameManager.Instance.SaveGameDataToCSV(waveNumber, totalEnemiesSpawned, new Dictionary<EnemyType, int>(enemyTypeCounts));
            Debug.Log("Hullám " + waveNumber + " indul");
            UpdateWaveText();

            yield return StartCoroutine(SpawnEnemiesAndProgressBar());

            yield return new WaitUntil(() => FindObjectsOfType<EnemyAI>().Length == 0);

            GameManager.Instance.LogWaveNumber(waveNumber);
            LogWaveData();
            GameManager.Instance.LogPlayerLivesAtWaveEnd();
            GameManager.Instance.LogTowerStats();

            GameManager.Instance.LogPlayerMoneyAtWaveEnd(waveNumber);

            if (waveNumber >= maxWaves)
            {
                gameOver = true;
                Debug.Log("Vége a játéknak.");

                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                int lastPlayableSceneIndex = 4;

                if (currentSceneIndex == lastPlayableSceneIndex)
                {
                    Debug.Log("Game complete! Showing GameCompleteUI.");
                    gameCompleteUI.ShowGameComplete();
                }
                else
                {
                    Debug.Log("Level complete! Showing LevelCompleteUI.");
                    levelCompleteMenu.ShowLevelComplete();
                }

                yield break;
            }

            waveNumber++;
            countdown = 10f;
        }
    }








    IEnumerator UpdateProgressBar()
    {
        float elapsedTime = 0f;
        waveProgressBarForeground.fillAmount = 0f;

        while (elapsedTime < waveDuration)
        {
            elapsedTime += Time.deltaTime;
            waveProgressBarForeground.fillAmount = Mathf.Clamp01(elapsedTime / waveDuration);
            yield return null;
        }
        waveProgressBarForeground.fillAmount = 1f;
    }

    IEnumerator SpawnEnemies()
    {
        Debug.Log("SpawnEnemies coroutine started.");
        float elapsedTime = 0f;
        float spawnDelay = UnityEngine.Random.Range(0.5f, 2f);
        GameObject[] enemyPool = DetermineEnemyPool();

        if (enemyPool.Length == 0)
        {
            Debug.LogError("Nincs ellenség a poolban!");
            yield break;
        }

        
        GameObject firstEnemy = enemyPool[UnityEngine.Random.Range(0, enemyPool.Length)];
        Instantiate(firstEnemy, spawnPoint.position, spawnPoint.rotation);
        UpdateEnemyCounts(firstEnemy); 
        Debug.Log("First enemy spawned immediately.");
        totalEnemiesSpawned++;

        elapsedTime += spawnDelay;

        while (elapsedTime < waveDuration)
        {
            StartCoroutine(SpawnEnemyWithDelay(enemyPool[UnityEngine.Random.Range(0, enemyPool.Length)], spawnDelay));
            elapsedTime += spawnDelay;
            yield return new WaitForSeconds(spawnDelay);
            spawnDelay = UnityEngine.Random.Range(0.5f, 2f);
        }
    }

    GameObject[] DetermineEnemyPool()
    {
        if (waveNumber == 1)
        {
            Debug.Log("Wave 1 enemy pool created.");
            int numMediumEnemies = Mathf.Min(mediumEnemies.Length, 2);
            GameObject[] mixedPool = new GameObject[easyEnemies.Length + numMediumEnemies];
            Array.Copy(easyEnemies, mixedPool, easyEnemies.Length);
            Array.Copy(mediumEnemies, 0, mixedPool, easyEnemies.Length, numMediumEnemies);
            Debug.Log($"Pool size: {mixedPool.Length}");
            return mixedPool;
        }
        else if (waveNumber == 2)
        {
            int numEasyEnemies = Mathf.Min(easyEnemies.Length, 2);
            GameObject[] mixedPool = new GameObject[mediumEnemies.Length + numEasyEnemies];
            Array.Copy(mediumEnemies, mixedPool, mediumEnemies.Length);
            Array.Copy(easyEnemies, 0, mixedPool, mediumEnemies.Length, numEasyEnemies);
            Debug.Log($"Pool size: {mixedPool.Length}");
            return mixedPool;
        }
        else if (waveNumber >= 3)
        {
            GameObject[] mixedPool = new GameObject[easyEnemies.Length + mediumEnemies.Length + hardEnemies.Length];
            Array.Copy(easyEnemies, mixedPool, easyEnemies.Length);
            Array.Copy(mediumEnemies, 0, mixedPool, easyEnemies.Length, mediumEnemies.Length);
            Array.Copy(hardEnemies, 0, mixedPool, easyEnemies.Length + mediumEnemies.Length, hardEnemies.Length);
            Debug.Log($"Pool size: {mixedPool.Length}");
            return mixedPool;
        }

        return easyEnemies;
    }

    void SpawnEnemy(GameObject[] enemyPool)
    {
        if (enemyPool.Length == 0)
        {
            Debug.LogError("Enemy pool is empty!");
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, enemyPool.Length);
        GameObject enemy = Instantiate(enemyPool[randomIndex], spawnPoint.position, spawnPoint.rotation);
        UpdateEnemyCounts(enemy);
        totalEnemiesSpawned++;
    }

    IEnumerator SpawnEnemyWithDelay(GameObject enemyPrefab, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        
        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            float healthMultiplier = GetHealthMultiplier();
            enemyAI.SetHealth(healthMultiplier);
        }

        UpdateEnemyCounts(enemy);
        totalEnemiesSpawned++;
        Debug.Log($"Enemy spawned with {GetHealthMultiplier()}x health multiplier.");
    }




    void UpdateEnemyCounts(GameObject enemy)
    {
        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            if (enemyTypeCounts.ContainsKey(enemyAI.enemyType))
            {
                enemyTypeCounts[enemyAI.enemyType]++;
            }
            else
            {
                Debug.LogError($"Unknown enemy type: {enemyAI.enemyType}");
            }
        }
    }

    void UpdateWaveText()
    {
        waveNumberText.text = waveNumber + "/" + maxWaves;
    }
    private float GetHealthMultiplier()
    {
        return 1.0f + (waveNumber - 1) * 0.15f;
    }

}
