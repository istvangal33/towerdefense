using UnityEngine;

public class Shop : MonoBehaviour
{
    BuildManager buildManager;

    void Start()
    {
        buildManager = BuildManager.instance;
    }

    public void Tower1()
    {
        Debug.Log("Machine gun selected");
        buildManager.SetTurretToBuild(buildManager.Tower1);
    }

    public void Tower2()
    {
        Debug.Log("Rocket tower selected");
        buildManager.SetTurretToBuild(buildManager.Tower2);
    }

    public void Tower3()
    {
        Debug.Log("Laser tower selected");
        buildManager.SetTurretToBuild(buildManager.Tower3);
    }


}