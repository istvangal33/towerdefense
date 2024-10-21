using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    public GameObject[] easyEnemies;
    public GameObject[] mediumEnemies;
    public GameObject[] hardEnemies;
    public Transform spawnPoint;
    public float waveDuration = 60f;  // Minden hullám 60 másodpercig tart
    private float countdown = 0f;  // Kezdő visszaszámlálás (az első hullám azonnal indul)
    private int waveNumber = 1;
    public int maxWaves = 10;

    public Image waveProgressBarForeground;
    public TextMeshProUGUI waveNumberText;  // Hullám számának megjelenítése

    private int playerScore;
    private bool gameOver = false;

    void Start()
    {
        UpdateWaveText();
        waveProgressBarForeground.fillAmount = 0f;  // Progress bar 0-ról indul
    }

    void Update()
    {
        if (gameOver)
            return;

        countdown -= Time.deltaTime;

        if (countdown <= 0f && waveNumber <= maxWaves)
        {
            StartCoroutine(SpawnWave());
            countdown = 20f;  // 20 másodperces visszaszámlálás a következő hullámig
        }
    }

    IEnumerator SpawnWave()
    {
        Debug.Log("Hullám " + waveNumber + " indul");

        float elapsedTime = 0f;
        GameObject[] enemyPool = DetermineEnemyPool();

        // Frissítjük a progress bar-t a hullám ideje alatt
        StartCoroutine(UpdateProgressBar(waveDuration));

        while (elapsedTime < waveDuration)
        {
            SpawnEnemy(enemyPool);
            float spawnDelay = Random.Range(3f, 5f);  // 3-5 másodperces időköz az ellenségek között
            elapsedTime += spawnDelay;
            yield return new WaitForSeconds(spawnDelay);
        }

        waveNumber++;
        UpdateWaveText();

        // A hullám végén a progress bar-t nullára állítjuk
        waveProgressBarForeground.fillAmount = 0f;

        if (waveNumber > maxWaves)
        {
            gameOver = true;
            Debug.Log("Vége a játéknak.");
        }
    }

    // Folyamatosan frissíti a progress bar-t a hullám során
    IEnumerator UpdateProgressBar(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            waveProgressBarForeground.fillAmount = Mathf.Clamp01(elapsedTime / duration);
            yield return null;
        }

        // A progress bar visszaáll nullára a hullám végén
        waveProgressBarForeground.fillAmount = 0f;
    }

    // Ellenségek meghatározása
    GameObject[] DetermineEnemyPool()
    {
        if (waveNumber == 1)
        {
            return easyEnemies;  // 1. hullám: csak easy ellenségek
        }
        else if (waveNumber == 2)
        {
            return mediumEnemies;  // 2. hullám: csak medium ellenségek
        }
        else if (waveNumber == 3)
        {
            return hardEnemies;  // 3. hullám: csak hard ellenségek
        }
        else if (waveNumber >= 4)
        {
            // 4. hullámtól kezdve minden ellenségtípus vegyesen jön
            GameObject[] mixedPool = new GameObject[easyEnemies.Length + mediumEnemies.Length + hardEnemies.Length];
            easyEnemies.CopyTo(mixedPool, 0);
            mediumEnemies.CopyTo(mixedPool, easyEnemies.Length);
            hardEnemies.CopyTo(mixedPool, easyEnemies.Length + mediumEnemies.Length);
            return mixedPool;
        }

        return easyEnemies;  // Alapértelmezett: easy ellenségek
    }

    void SpawnEnemy(GameObject[] enemyPool)
    {
        int randomIndex = Random.Range(0, enemyPool.Length);
        GameObject enemy = enemyPool[randomIndex];
        Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
    }

    void UpdateWaveText()
    {
        waveNumberText.text = waveNumber.ToString() + "/10";
    }
}
