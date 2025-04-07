using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class WaveSpawnerPlayModeTests
{
    private GameObject spawnerObject;
    private WaveSpawner waveSpawner;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        
        if (SceneManager.GetActiveScene().name != "LVL1") 
        {
            SceneManager.LoadScene("LVL1");
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(2f); 

        waveSpawner = GameObject.FindObjectOfType<WaveSpawner>();
        Assert.IsNotNull(waveSpawner, "WaveSpawner nincs a scene-ben.");
    }

    [UnityTest]
    public IEnumerator EnemySpawnsWithinExpectedTime()
    {
        float startTime = Time.time;

        yield return new WaitUntil(() => GameObject.FindWithTag("Enemy") != null);

        float spawnTime = Time.time - startTime;
        Assert.IsTrue(spawnTime >= 0.0f && spawnTime <= 2f, $"Spawn idon kivuli tartomany: {spawnTime}");
    }

    [UnityTest]
    public IEnumerator EnemyMovesAfterSpawning()
    {
        yield return new WaitUntil(() => GameObject.FindWithTag("Enemy") != null);
        GameObject enemy = GameObject.FindWithTag("Enemy");

        Vector3 startPos = enemy.transform.position;
        yield return new WaitForSeconds(1f);
        Vector3 newPos = enemy.transform.position;

        float moved = Vector3.Distance(startPos, newPos);
        Assert.Greater(moved, 0.1f, "Enemy nem mozdult el a spawn utan.");
    }

    [UnityTest]
    public IEnumerator EnemyHasValidStats()
    {
        yield return new WaitUntil(() => GameObject.FindWithTag("Enemy") != null);
        GameObject enemy = GameObject.FindWithTag("Enemy");

        EnemyAI ai = enemy.GetComponent<EnemyAI>();
        Assert.IsNotNull(ai, "EnemyAI komponens hianyzik!");

        Assert.Greater(ai.startHealth, 0, "Életerõ <= 0!");
        Assert.Greater(ai.GetComponent<UnityEngine.AI.NavMeshAgent>().speed, 0, "speed <= 0!");

        yield return null;
    }

    [UnityTest]
    public IEnumerator AIPredictionReturnsValidMultipliers()
    {
        yield return null; 

        ONNXModelLoader model = GameObject.FindObjectOfType<ONNXModelLoader>();
        Assert.IsNotNull(model, "ONNXModelLoader nem talalhato!");

        float[] input = new float[] { 1, 1, 15, 0 }; 
        float[] output = model.Predict(input);

        Assert.IsTrue(output.Length >= 2, "Az AI output hossza nem megfelelo!");
        Assert.Greater(output[0], 0f, "Health szorzo hibas!");
        Assert.Greater(output[1], 0f, "Speed szorzo hibas!");
    }
}
