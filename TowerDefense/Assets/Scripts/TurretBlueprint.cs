using System.Collections;
using UnityEngine;


[System.Serializable]
public class TurretBlueprint
{
    public GameObject prefab;
    public int cost;

    public GameObject upgradedPrefab;
    public int upgradeCost;

    public GameObject upgradedPrefab2;
    public int upgradeCost2;

    public float[] rangeByLevel = new float[3];
    public float[] fireRateByLevel = new float[3];
    public int[] damageByLevel = new int[3];

    public int GetSellAmount(int upgradeLevel, float sellMultiplier = 0.5f)
    {
        switch (upgradeLevel)
        {
            case 0:
                return Mathf.RoundToInt(cost * sellMultiplier);
            case 1:
                return Mathf.RoundToInt((cost + upgradeCost) * sellMultiplier);
            case 2:
                return Mathf.RoundToInt((cost + upgradeCost + upgradeCost2) * sellMultiplier);
            default:
                return 0;
        }
    }



    public float GetRange(int upgradeLevel)
    {
        return rangeByLevel[upgradeLevel];
    }

    public float GetFireRate(int upgradeLevel)
    {
        return fireRateByLevel[upgradeLevel];
    }

    public int GetDamage(int upgradeLevel)
    {
        return damageByLevel[upgradeLevel];
    }

    public string GetStats(int upgradeLevel)
    {
        return $"Cost: {cost}, UpgradeCost1: {upgradeCost}, UpgradeCost2: {upgradeCost2}, " +
               $"Range: {GetRange(upgradeLevel)}, FireRate: {GetFireRate(upgradeLevel)}, Damage: {GetDamage(upgradeLevel)}";
    }
}

