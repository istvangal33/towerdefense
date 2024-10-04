using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public GameObject[] enemyTypes;  // Az ellenség típusok listája
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
        // Az ellenségtípust a nehézségi szint alapján választja ki
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
        // Az AI kiválaszt egy ellenségtípust a hullám nehézségi szintjéhez igazítva
        int index = waveNumber % enemyTypes.Length;  // Egyszerű logika, ami az ellenségtípusokat körforgásban váltogatja
        return enemyTypes[index];
    }

    // A játékos teljesítménye alapján növeli a pontszámot, ami befolyásolja a hullámok nehézségét
    public void IncreasePlayerScore(int amount)
    {
        playerScore += amount;
    }
}
