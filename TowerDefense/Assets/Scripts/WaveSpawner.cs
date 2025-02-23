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

    private float healthMultiplier = 1.0f; 
    private float speedMultiplier = 1.0f; 

    public int totalEnemiesSpawned = 0;
    public Dictionary<EnemyType, int> enemyTypeCounts = new Dictionary<EnemyType, int>
    {
        { EnemyType.Buggy, 0 },
        { EnemyType.Helicopter, 0 },
        { EnemyType.Hovertank, 0 }
    };

    [Tooltip("Drag the GO with OnnxModelManager as Component here")]
    public ONNXModelLoader onnxModelLoader;

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

    void TestAIPredictions()
    {
        Debug.Log("🔍 AI Prediction Testing Started...");

        float[][] testCases = new float[][]
        {
        new float[] { 1, 1, 15, 0 },
        new float[] { 5, 3, 10, 70 },
        new float[] { 10, 1, 15, 80 },
        new float[] { 3, 2, 5, 50 },
        new float[] { 7, 1, 10, 20 } 
        };

        foreach (var test in testCases)
        {
            float[] output = onnxModelLoader.Predict(test);
            Debug.Log($"🧠 AI Prediction for Input: {string.Join(", ", test)}");
            Debug.Log($"➡️ Health Multiplier: {output[0]:F3}, Speed Multiplier: {output[1]:F3}");
        }

        Debug.Log("✅ AI Prediction Testing Finished.");
    }




    void Start()
    {
        UpdateWaveText();
        waveProgressBarForeground.fillAmount = 0f;
        StartCoroutine(UpdateProgressBar());
        StartCoroutine(ManageWave());
        InitializeTowerCoverage();
        TestAIPredictions();
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

    IEnumerator SpawnEnemiesAndProgressBar(float healthMultiplier, float speedMultiplier)
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
            StartCoroutine(SpawnEnemyWithDelay(enemyPool[UnityEngine.Random.Range(0, enemyPool.Length)], spawnDelay, healthMultiplier, speedMultiplier));

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

            float[] aiInput = new float[]
            {
                GameManager.Instance.CurrentLevel,
                waveNumber,
                PlayerStats.Lives,
                FindObjectOfType<GridCoverageManager>()?.GetCoverageByTower() ?? 0f
        };


            Debug.Log($"📥 Normalized AI Input: {string.Join(", ", aiInput)}");

            
            float[] output = onnxModelLoader.Predict(aiInput);
            Debug.Log($"ONNX Output (Raw): {string.Join(", ", output)}");

            
            float healthMultiplier = output[0];
            float speedMultiplier = output[1];


            Debug.Log($"📤 Denormalized AI Output: Health={healthMultiplier}, Speed={speedMultiplier}");


            yield return StartCoroutine(SpawnEnemiesAndProgressBar(healthMultiplier, speedMultiplier));
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



    void AdjustEnemyStats(float healthMultiplier, float countMultiplier)
    {
        Debug.Log($"Életerő szorzó: {healthMultiplier}, Darabszám szorzó: {countMultiplier}");

        
        foreach (var enemy in FindObjectsOfType<EnemyAI>())
        {
            enemy.SetHealth(healthMultiplier, 1.0f);
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
            StartCoroutine(SpawnEnemyWithDelay(enemyPool[UnityEngine.Random.Range(0, enemyPool.Length)], spawnDelay, healthMultiplier, speedMultiplier));
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

    IEnumerator SpawnEnemyWithDelay(GameObject enemyPrefab, float delay, float healthMultiplier, float speedMultiplier)
    {
        yield return new WaitForSeconds(delay);
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.SetWaveNumber(waveNumber);

            float aiHealthMultiplier = 1.0f;
            float aiSpeedMultiplier = 1.0f;

            if (waveNumber > 1)
            {
                aiHealthMultiplier = Mathf.Clamp(healthMultiplier, 0.8f, 2.5f);
                aiSpeedMultiplier = Mathf.Clamp(speedMultiplier, 0.7f, 1.5f);
            }

            enemyAI.SetHealth(1.0f, aiHealthMultiplier);
            enemyAI.SetSpeed(aiSpeedMultiplier);

            Debug.Log($"[SpawnEnemy] {enemyAI.enemyType} | Wave: {waveNumber} | AI HP Multiplier: {aiHealthMultiplier}, AI Speed Multiplier: {aiSpeedMultiplier}, Final HP: {enemyAI.startHealth * aiHealthMultiplier}");
        }

        UpdateEnemyCounts(enemy);
        totalEnemiesSpawned++;
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

}