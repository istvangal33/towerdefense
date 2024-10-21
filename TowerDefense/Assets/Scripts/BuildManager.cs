using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;

    public GameObject Tower1;
    public GameObject Tower2;
    public GameObject Tower3;

    private TurretBlueprint turretToBuild;
    

    
    public bool CanBuild { get { return turretToBuild != null; } }
    public bool HasMoney { get { return PlayerStats.Money >= turretToBuild.cost; } }

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than 1 BuildManager");
            return;
        }
        instance = this;
    }

    public void BuildTurretOn(Node node)
    {
        if (PlayerStats.Money < turretToBuild.cost)
        {
            Debug.Log("Not enough money to build that one");
            return;
        }

        PlayerStats.Money -= turretToBuild.cost;

        GameObject turret = (GameObject)Instantiate(turretToBuild.prefab, node.GetBuildPosition(), Quaternion.identity);
        node.turret = turret;

        Debug.Log("Tower built! Money left: " + PlayerStats.Money);
    }

    

    public void SelectTurretToBuild(TurretBlueprint turret)
    {
        turretToBuild = turret;
        
    }

}
