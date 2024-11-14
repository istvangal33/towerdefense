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

    public int GetSellAmount(int upgradeLevel)
    {
        switch (upgradeLevel)
        {
            case 0:
                return cost / 2; 
            case 1:
                return (cost + upgradeCost) / 2; 
            case 2:
                return (cost + upgradeCost + upgradeCost2) / 2; 
            default:
                return 0;
        }
    }
}

