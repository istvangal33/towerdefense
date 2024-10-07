using UnityEngine;

public class Shop : MonoBehaviour
{
    public TurretBlueprint Machinegun;
    public TurretBlueprint RocketTower;
    public TurretBlueprint LaserTower;

    BuildManager buildManager;

    void Start()
    {
        buildManager = BuildManager.instance;
    }

    public void SelectTower1()
    {
        Debug.Log("Machine gun selected");
        buildManager.SelectTurretToBuild(Machinegun);
    }

    public void SelectTower2()
    {
        Debug.Log("Rocket tower selected");
        buildManager.SelectTurretToBuild(RocketTower);
    }

    public void SelectTower3()
    {
        Debug.Log("Laser tower selected");
        buildManager.SelectTurretToBuild(LaserTower);
    }


}