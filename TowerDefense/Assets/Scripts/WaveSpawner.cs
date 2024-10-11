using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public GameObject[] easyEnemies;  // Könnyű ellenségek listája
    public GameObject[] mediumEnemies; // Közepes ellenségek listája
    public GameObject[] hardEnemies;   // Nehéz ellenségek listája
    public Transform spawnPoint;
    public float timeBetweenWaves = 5f;
    private float countdown = 2f;
    private int waveNumber = 1;

    private int playerScore;  // A játékos teljesítménye (pl. pontszám alapján)

    void Update()
    {
        if (countdown <= 0f)
        {
            StartCoroutine(SpawnWave());
            countdown = timeBetweenWaves;
        }
        countdown -= Time.deltaTime;
    }

    IEnumerator SpawnWave()
    {
        Debug.Log("Hullám " + waveNumber + " indul");

        int enemyCount = CalculateEnemyCount();
        for (int i = 0; i < enemyCount; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(1f);
        }

        waveNumber++;
    }

    void SpawnEnemy()
    {
        // Az ellenségtípust az AI dönti el
        GameObject enemyType = ChooseEnemyType();
        Instantiate(enemyType, spawnPoint.position, spawnPoint.rotation);
    }

    int CalculateEnemyCount()
    {
        // Az ellenségek száma a hullám száma és a játékos teljesítménye alapján növekszik
        return Mathf.RoundToInt(waveNumber * 1.5f + playerScore * 0.1f);
    }

    GameObject ChooseEnemyType()
    {
        // AI alapú döntés, hogy melyik ellenséget spawnolja, figyelembe véve a hullámot és a játékos pontszámát
        if (playerScore < 20 && waveNumber < 5)
        {
            // Ha a játékos még nem teljesített jól, könnyű ellenségeket spawnolunk
            return GetRandomEnemy(easyEnemies);
        }
        else if (playerScore < 50 && waveNumber < 10)
        {
            // Ha a játékos teljesítménye nő, közepes ellenségeket adunk
            return GetRandomEnemy(mediumEnemies);
        }
        else
        {
            // Ha a játékos jól teljesít, nehéz ellenségeket spawnolunk
            return GetRandomEnemy(hardEnemies);
        }
    }

    GameObject GetRandomEnemy(GameObject[] enemyArray)
    {
        int randomIndex = Random.Range(0, enemyArray.Length);
        return enemyArray[randomIndex];
    }

    // A játékos teljesítménye alapján növeli a pontszámot, ami befolyásolja a hullámok nehézségét
    public void IncreasePlayerScore(int amount)
    {
        playerScore += amount;
    }
}
