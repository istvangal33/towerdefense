using UnityEngine;

public class Shop : MonoBehaviour
{

    BuildManager buildManager;
    void Start()
    {
        buildManager = BuildManager.instance;
    }

    public void PurchaseMachineTower()
    {
        Debug.Log("MachineTower Selected");
        buildManager.SetTurretToBuild(buildManager.Tower1);
    }
    public void PurchaseRocketTower()
    {
        Debug.Log("RocketTower Selected");
        buildManager.SetTurretToBuild(buildManager.Tower2);
    }
    public void PurchaseLaserTower()
    {
        Debug.Log("LaserTower Selected");
        buildManager.SetTurretToBuild(buildManager.Tower3);
    }
}
