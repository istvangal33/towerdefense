using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class LaserTower
{
    GameObject towerGO, enemyCloser, endGO, soundManagerGO;

    [UnityTest]
    public IEnumerator LaserTower_Damages_And_Slows_Enemy()
    {
        
        new GameObject("AudioListenerGO").AddComponent<AudioListener>();

        
        soundManagerGO = new GameObject("SoundManager");
        var audioSource = soundManagerGO.AddComponent<AudioSource>();
        var soundManager = soundManagerGO.AddComponent<SoundManager>();
        soundManager.laserAudioSource = audioSource;

        
        endGO = new GameObject("End");
        endGO.transform.position = new Vector3(20, 0, 0);

        
        towerGO = new GameObject("LaserTower");
        Tower tower = towerGO.AddComponent<Tower>();
        tower.range = 50f;
        tower.useLaser = true;
        tower.laserDamagePerSecond = 50f;
        tower.towerType = TowerType.Laser;
        tower.rotatingPart = new GameObject("RotatingPart").transform;
        tower.rotatingPart.SetParent(towerGO.transform);
        tower.firePoint = new GameObject("FirePoint").transform;
        tower.firePoint.SetParent(towerGO.transform);
        tower.firePoint.localPosition = Vector3.zero;
        tower.lineRenderer = towerGO.AddComponent<LineRenderer>();
        tower.endPoint = endGO.transform;

        
        enemyCloser = GameObject.CreatePrimitive(PrimitiveType.Cube);
        enemyCloser.transform.position = new Vector3(10, 0, 0);
        enemyCloser.tag = "Enemy";

        var ai = enemyCloser.AddComponent<EnemyAI>();
        ai.enemyType = EnemyType.Buggy;
        
        ai.startHealth = 200;
        ai.healthBar = new GameObject("DummyHealthBar").AddComponent<UnityEngine.UI.Image>();
        ai.deathEffect = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        enemyCloser.AddComponent<UnityEngine.AI.NavMeshAgent>().speed = 3f;

        yield return new WaitUntil(() => tower.target != null && tower.lineRenderer.enabled);

        float health = (float)ai.GetType().GetField("health", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(ai);

        Assert.Less(health, 200f, "A lasertower nem sebzett!");
        Assert.IsTrue(ai.isSlowed, "A lasertower nem lassított!");
        Assert.IsTrue(tower.lineRenderer.enabled, "A LineRenderer nem lett bekapcsolva!");
    }


    [TearDown]
    public void Cleanup()
    {
        Object.Destroy(towerGO);
        Object.Destroy(enemyCloser);
        Object.Destroy(endGO);
        Object.Destroy(soundManagerGO);
    }
}