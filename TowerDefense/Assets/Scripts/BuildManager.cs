using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;

    void Awake ()
    {
        if (instance != null)
        {
            Debug.LogError("More than 1 BuildManager");
            return;
        }
        instance = this;
    }

    public GameObject Tower1;
    public GameObject Tower2;
    public GameObject Tower3;



    private GameObject turretToBuild;



    public GameObject GetTurretToBuild()
    {
        return turretToBuild;
    }

    public void SetTurretToBuild(GameObject turret)
    {
        turretToBuild = turret;
    }

}