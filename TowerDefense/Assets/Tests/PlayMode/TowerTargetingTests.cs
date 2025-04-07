using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TowerTargetingTests
{
    private GameObject towerGO;
    private GameObject enemyCloser;
    private GameObject enemyFurther;
    private GameObject endGO;
    private GameObject dummyBullet;
    private GameObject testCamera;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        
        testCamera = new GameObject("TestCamera");
        testCamera.AddComponent<Camera>();
        testCamera.AddComponent<AudioListener>();

        
        dummyBullet = new GameObject("DummyBullet");
        dummyBullet.AddComponent<Bullet>();
        dummyBullet.SetActive(false);

        yield return null;
    }

    [UnityTest]
    public IEnumerator Tower_Selects_Enemy_Closer_To_End()
    {
        
        endGO = new GameObject("End");
        endGO.transform.position = new Vector3(20, 0, 0);

        
        towerGO = new GameObject("Tower");
        var tower = towerGO.AddComponent<Tower>();
        tower.range = 50f;
        tower.fireRate = 1f; 
        tower.towerType = TowerType.MachineGun;

        
        tower.rotatingPart = new GameObject("RotatingPart").transform;
        tower.rotatingPart.SetParent(towerGO.transform);

        var firePoint = new GameObject("FirePoint");
        tower.firePoint = firePoint.transform;
        tower.firePoint.SetParent(towerGO.transform);

        tower.endPoint = endGO.transform;
        tower.bulletPrefab = dummyBullet;

        enemyCloser = GameObject.CreatePrimitive(PrimitiveType.Cube);
        enemyCloser.name = "CloserEnemy";
        enemyCloser.transform.position = new Vector3(15, 0, 0);
        enemyCloser.tag = "Enemy";
        var aiCloser = enemyCloser.AddComponent<EnemyAI>();
        aiCloser.enemyType = EnemyType.Buggy;
        aiCloser.enabled = false;

        
        enemyFurther = GameObject.CreatePrimitive(PrimitiveType.Cube);
        enemyFurther.name = "FurtherEnemy";
        enemyFurther.transform.position = new Vector3(5, 0, 0);
        enemyFurther.tag = "Enemy";
        var aiFurther = enemyFurther.AddComponent<EnemyAI>();
        aiFurther.enemyType = EnemyType.Buggy;
        aiFurther.enabled = false;

        
        yield return new WaitForSeconds(0.1f);

        
        Debug.Log($"Tower target: {tower.target?.name}");
        Debug.Log($"Distance of closer enemy to end: {Vector3.Distance(enemyCloser.transform.position, endGO.transform.position)}");
        Debug.Log($"Distance of further enemy to end: {Vector3.Distance(enemyFurther.transform.position, endGO.transform.position)}");

        
        Assert.IsNotNull(tower.target, "Tower should have selected a target");
        Assert.AreEqual(enemyCloser.transform, tower.target,
            "Tower should target enemy closer to end point. " +
            $"Expected: {enemyCloser.name} at {enemyCloser.transform.position}, " +
            $"Actual: {tower.target.name} at {tower.target.position}");
    }

    [UnityTest]
    public IEnumerator Tower_DoesNotTarget_Enemy_Outside_Range()
    {
        
        endGO = new GameObject("End");
        endGO.transform.position = new Vector3(20, 0, 0);

        towerGO = new GameObject("Tower");
        var tower = towerGO.AddComponent<Tower>();
        tower.range = 5f;
        tower.fireRate = 1f;
        tower.towerType = TowerType.MachineGun;

        tower.rotatingPart = new GameObject("RotatingPart").transform;
        tower.rotatingPart.SetParent(towerGO.transform);

        var firePoint = new GameObject("FirePoint");
        tower.firePoint = firePoint.transform;
        tower.firePoint.SetParent(towerGO.transform);

        tower.endPoint = endGO.transform;
        tower.bulletPrefab = dummyBullet;

        
        enemyCloser = GameObject.CreatePrimitive(PrimitiveType.Cube);
        enemyCloser.transform.position = new Vector3(50, 0, 0);
        enemyCloser.tag = "Enemy";
        var ai = enemyCloser.AddComponent<EnemyAI>();
        ai.enemyType = EnemyType.Buggy;
        ai.enabled = false;

        yield return new WaitForSeconds(0.1f);

        Assert.IsNull(tower.target, "Tower should not target enemy outside range");
    }

    [UnityTearDown]
    public IEnumerator Cleanup()
    {
        
        if (towerGO != null) Object.Destroy(towerGO);
        if (enemyCloser != null) Object.Destroy(enemyCloser);
        if (enemyFurther != null) Object.Destroy(enemyFurther);
        if (endGO != null) Object.Destroy(endGO);
        if (dummyBullet != null) Object.Destroy(dummyBullet);
        if (testCamera != null) Object.Destroy(testCamera);

        yield return null;
    }
}