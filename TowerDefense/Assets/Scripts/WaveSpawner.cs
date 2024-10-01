using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public enum SpawnState { SPAWNING, WAITING, COUNTING };

    [System.Serializable]
    public class Wave
    {
        public string name;
        public Transform[] enemies;
        public int count;
        public float rate;
    }

    public Wave[] waves;
    private int nextWave = 0;

    public Transform[] spawnPoints;

    public float timeBetweenWaves = 5f;
    private float waveCountDown;

    private float searchCountDown = 1f;

    private SpawnState state = SpawnState.COUNTING;

    void Start()
    {
        waveCountDown = timeBetweenWaves;
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");
        }
    }

    void Update()
    {
        if (state == SpawnState.WAITING)
        {
            if (!EnemyIsAlive())
            {
                WaveCompleted();
            }
            else
            {
                return;
            }
        }

        if (waveCountDown <= 0)
        {
            if (state != SpawnState.SPAWNING)
            {
                // Check if the nextWave is a valid index
                if (nextWave >= 0 && nextWave < waves.Length)
                {
                    StartCoroutine(SpawnWave(waves[nextWave]));
                }
                else
                {
                    Debug.LogError("Invalid wave index: " + nextWave);
                }
            }
        }
        else
        {
            waveCountDown -= Time.deltaTime;
        }
    }

    void WaveCompleted()
    {
        Debug.Log("Wave Completed");

        state = SpawnState.COUNTING;
        waveCountDown = timeBetweenWaves;

        if (nextWave + 1 >= waves.Length)
        {
            // Implement difficulty increase or end game logic here
            nextWave = 0;
            Debug.Log("ALL WAVES COMPLETE! Restarting...");
        }
        else
        {
            nextWave++;
        }
    }

    bool EnemyIsAlive()
    {
        searchCountDown -= Time.deltaTime;
        if (searchCountDown <= 0)
        {
            searchCountDown = 1f;
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                return false;
            }
        }
        return true;
    }

    IEnumerator SpawnWave(Wave _wave)
    {
        Debug.Log("Spawning Wave: " + _wave.name);
        state = SpawnState.SPAWNING;

        for (int i = 0; i < _wave.count; i++)
        {
            Debug.Log("Spawning enemy: " + i);
            SpawnEnemy(_wave.enemies); // Pass the array of enemies here
            yield return new WaitForSeconds(1f / _wave.rate);
        }

        state = SpawnState.WAITING;

        yield break;
    }

    void SpawnEnemy(Transform[] enemies)
    {
        if (enemies.Length == 0) 
        {
            Debug.LogError("No enemies available!");
            return; // Avoid further issues if no enemies are available
        }

        // Pick a random enemy from the array
        Transform enemy = enemies[Random.Range(0, enemies.Length)];

        if (enemy == null) 
        {
            Debug.LogError("Randomly selected enemy is null!");
            return; // Avoid further issues if the selected enemy is null
        }

        Debug.Log("Spawning Enemy: " + enemy.name);

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
    }
}
