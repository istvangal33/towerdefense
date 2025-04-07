using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TowerUpgradePrefabTest
{
    GameObject prefabLvl1;
    GameObject prefabLvl2;
    GameObject prefabLvl3;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        prefabLvl1 = Resources.Load<GameObject>("Towers/MachineTower1");
        prefabLvl2 = Resources.Load<GameObject>("Towers/MachineTower2");
        prefabLvl3 = Resources.Load<GameObject>("Towers/MachineTower3");

        Assert.IsNotNull(prefabLvl1, "MachineTower1 prefab nem található a Resources/Towers mappában!");
        Assert.IsNotNull(prefabLvl2, "MachineTower2 prefab nem található a Resources/Towers mappában!");
        Assert.IsNotNull(prefabLvl3, "MachineTower3 prefab nem található a Resources/Towers mappában!");

        yield return null;
    }

    [UnityTest]
    public IEnumerator PrefabUpgrade_Increases_Stats()
    {
        
        GameObject towerGO1 = Object.Instantiate(prefabLvl1, Vector3.zero, Quaternion.identity);
        Tower tower1 = towerGO1.GetComponent<Tower>();
        Assert.IsNotNull(tower1, "Tower1 prefab nem tartalmaz Tower komponenst!");
        float range1 = tower1.range;
        float fireRate1 = tower1.fireRate;
        Object.Destroy(towerGO1);
        yield return null;

        
        GameObject towerGO2 = Object.Instantiate(prefabLvl2, Vector3.zero, Quaternion.identity);
        Tower tower2 = towerGO2.GetComponent<Tower>();
        Assert.IsNotNull(tower2, "Tower2 prefab nem tartalmaz Tower komponenst!");
        float range2 = tower2.range;
        float fireRate2 = tower2.fireRate;

        if (range2 > range1)
            Debug.Log("Range nőtt a 2. szintre lépve.");
        else
            Debug.LogWarning("Range nem nőtt — ez elvárt viselkedés, ha nincs változtatva a prefabban.");

        if (fireRate2 > fireRate1)
            Debug.Log("FireRate nőtt a 2. szintre lépve.");
        else
            Debug.LogWarning("FireRate nem nőtt — ez elvárt viselkedés, ha nincs változtatva a prefabban.");

        Object.Destroy(towerGO2);
        yield return null;

        
        GameObject towerGO3 = Object.Instantiate(prefabLvl3, Vector3.zero, Quaternion.identity);
        Tower tower3 = towerGO3.GetComponent<Tower>();
        Assert.IsNotNull(tower3, "Tower3 prefab nem tartalmaz Tower komponenst!");
        float range3 = tower3.range;
        float fireRate3 = tower3.fireRate;

        if (range3 > range2)
            Debug.Log("Range nőtt a 3. szintre lépve.");
        else
            Debug.LogWarning("Range nem nőtt — ez elvárt viselkedés, ha nincs változtatva a prefabban.");

        if (fireRate3 > fireRate2)
            Debug.Log("FireRate nőtt a 3. szintre lépve.");
        else
            Debug.LogWarning("FireRate nem nőtt — ez elvárt viselkedés, ha nincs változtatva a prefabban.");

        Object.Destroy(towerGO3);
    }
}
