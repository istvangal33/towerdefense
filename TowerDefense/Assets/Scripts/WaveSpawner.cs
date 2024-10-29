using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System; 

public class WaveSpawner : MonoBehaviour
{
    public GameObject[] easyEnemies;
    public GameObject[] mediumEnemies;
    public GameObject[] hardEnemies;
    public Transform spawnPoint;
    public float waveDuration = 10f;  
    private float countdown = 0f;  
    private int waveNumber = 1;
    public int maxWaves = 15; 
    private bool firstWave = true;

    public Image waveProgressBarForeground;
    public TextMeshProUGUI waveNumberText;  

    private int playerScore;
    private bool gameOver = false;

    private string csvFilePath = "wave_data.csv"; 

    void Start()
    {
        UpdateWaveText();
        waveProgressBarForeground.fillAmount = 0f;

        // CSV fájl fejlécének létrehozása
        string header = "waveNumber,enemyType,spawnTime,playerHealth,playerMoney";
        WriteDataToCSV(header);
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(1.5f); 

        StartCoroutine(SpawnWave());  
        countdown = 20f;  
    }

    void Update()
    {
        if (gameOver)
            return;

        countdown -= Time.deltaTime;

        if (countdown <= 0f && waveNumber <= maxWaves)
        {
            StartCoroutine(SpawnWave());
            countdown = 30f;  
        }
    }

    IEnumerator SpawnWave()
    {
        Debug.Log("Hullám " + waveNumber + " indul");

       
        waveProgressBarForeground.fillAmount = 0f;

        float elapsedTime = 0f;
        GameObject[] enemyPool = DetermineEnemyPool();

        if (firstWave)
        {
            StartCoroutine(UpdateProgressBar(waveDuration)); 
            firstWave = false;  
        }

        float spawnDelay = UnityEngine.Random.Range(1f, 2f);

        while (elapsedTime < waveDuration)
        {
            SpawnEnemy(enemyPool);
            elapsedTime += spawnDelay;
            yield return new WaitForSeconds(spawnDelay);
            spawnDelay = UnityEngine.Random.Range(1f, 2f);
        }

        waveNumber++;
        UpdateWaveText();

        if (!firstWave)
        {
            StartCoroutine(UpdateProgressBar(waveDuration)); 
        }

        waveProgressBarForeground.fillAmount = 0f;

        if (waveNumber > maxWaves)
        {
            gameOver = true;
            Debug.Log("Vége a játéknak.");
        }
    }



    IEnumerator UpdateProgressBar(float duration)
    {
        float elapsedTime = 0f;
        waveProgressBarForeground.fillAmount = 0f;

        while (elapsedTime <= duration)
        {
            elapsedTime += Time.deltaTime;
            waveProgressBarForeground.fillAmount = Mathf.Clamp01(elapsedTime / duration);
            yield return null;
        }
    }


    
    GameObject[] DetermineEnemyPool()
    {
        if (waveNumber == 1)
        {
            
            int numMediumEnemies = Mathf.Min(mediumEnemies.Length, 2);
            GameObject[] mixedPool = new GameObject[easyEnemies.Length + numMediumEnemies];
            Array.Copy(easyEnemies, mixedPool, easyEnemies.Length);
            Array.Copy(mediumEnemies, 0, mixedPool, easyEnemies.Length, numMediumEnemies);
            return mixedPool;
        }
        else if (waveNumber == 2)
        {
            
            int numEasyEnemies = Mathf.Min(easyEnemies.Length, 2); 
            GameObject[] mixedPool = new GameObject[mediumEnemies.Length + numEasyEnemies];
            Array.Copy(mediumEnemies, mixedPool, mediumEnemies.Length);
            Array.Copy(easyEnemies, 0, mixedPool, mediumEnemies.Length, numEasyEnemies);
            return mixedPool;
        }
        else if (waveNumber >= 3)
        {
            
            GameObject[] mixedPool = new GameObject[easyEnemies.Length + mediumEnemies.Length + hardEnemies.Length];
            Array.Copy(easyEnemies, mixedPool, easyEnemies.Length);
            Array.Copy(mediumEnemies, 0, mixedPool, easyEnemies.Length, mediumEnemies.Length);
            Array.Copy(hardEnemies, 0, mixedPool, easyEnemies.Length + mediumEnemies.Length, hardEnemies.Length);
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
        GameObject enemy = enemyPool[randomIndex];

        Debug.Log("Spawning enemy: " + enemy.name);

        
        float spawnDelay = UnityEngine.Random.Range(1f, 3f);
        if (Array.IndexOf(hardEnemies, enemy) != -1)
        {
            spawnDelay = Mathf.Max(spawnDelay, 1f);
        }

        StartCoroutine(SpawnEnemyWithDelay(enemy, spawnDelay));
    }



    IEnumerator SpawnEnemyWithDelay(GameObject enemy, float delay)
    {
        yield return new WaitForSeconds(delay);

        Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);

        
        string enemyType = GetEnemyType(enemy);
        string spawnTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); 
        int playerHealth = PlayerStats.Lives;
        int playerMoney = PlayerStats.Money; 

       
        string towerData = GetTowerData();

        
        string data = $"{waveNumber},{enemyType},{spawnTime},{playerHealth},{playerMoney},{towerData}";
        WriteDataToCSV(data);
    }

    
    string GetEnemyType(GameObject enemy)
    {
        if (Array.IndexOf(easyEnemies, enemy) != -1) return "easy"; 
        if (Array.IndexOf(mediumEnemies, enemy) != -1) return "medium"; 
        if (Array.IndexOf(hardEnemies, enemy) != -1) return "hard"; 
        return "unknown";
    }


    
    void WriteDataToCSV(string data)
    {
        using (StreamWriter writer = new StreamWriter(csvFilePath, true))
        {
            writer.WriteLine(data);
        }
    }

    
    string GetTowerData()
    {
        
        return "tower data"; 
    }


    void UpdateWaveText()
    {
        waveNumberText.text = waveNumber.ToString() + "/15"; 
    }
}