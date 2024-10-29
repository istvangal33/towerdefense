using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;

    
    public GameObject sellEffect;

    private TurretBlueprint turretToBuild;
    private Node selectedNode;
    public NodeUI nodeUI;

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

        node.turretBlueprint = turretToBuild;
        node.isOccupied = true;

        

        Debug.Log("Turret built! Money left: " + PlayerStats.Money);
    }

    public void SelectNode(Node node)
    {
        if (node.turret == null) 
        {
            DeselectNode();
            return;
        }

        selectedNode = node;
        nodeUI.SetTarget(node);
        nodeUI.gameObject.SetActive(true);
        nodeUI.transform.position = node.GetBuildPosition();
    }

    public void DeselectNode()
    {
        selectedNode = null;
        nodeUI.Hide();
    }

    public void SelectTurretToBuild(TurretBlueprint turret)
    {
        turretToBuild = turret;
        selectedNode = null;
    }

    public TurretBlueprint GetTurretToBuild()
    {
        return turretToBuild;
    }
}
