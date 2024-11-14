using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;

    public GameObject sellEffect;
    private TurretBlueprint turretToBuild;
    private Node selectedNode;
    private Tower selectedTower;
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

        
        GameObject turret = Instantiate(turretToBuild.prefab, node.GetBuildPosition(), Quaternion.identity);

        
        if (turret != null)
        {
            node.turret = turret;

            
            Tower towerScript = turret.GetComponent<Tower>();
            if (towerScript != null)
            {
                towerScript.node = node;
            }

            node.turretBlueprint = turretToBuild;
            node.isOccupied = true; 
            Debug.Log("Turret built! Money left: " + PlayerStats.Money);
        }
        else
        {
            Debug.LogError("Failed to build turret!");
        }
    }


    public void SelectNode(Node node)
    {

        if (nodeUI == null)
        {
            Debug.LogError("NodeUI reference is null in BuildManager. Check if it is assigned in the Inspector.");
            return;
        }


        if (node == null)
        {
            Debug.LogError("Node reference is null. Make sure a valid node is passed to SelectNode.");
            return;
        }

        if (node.turret == null)
        {
            DeselectNode();
            return;
        }

        selectedNode = node;
        selectedTower = null;


        try
        {
            nodeUI.SetTarget(node);
            nodeUI.gameObject.SetActive(true);
            nodeUI.transform.position = node.GetBuildPosition();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error in setting nodeUI target: " + ex.Message);
        }
    }


    public void SelectTower(Tower tower)
    {
        selectedTower = tower;
        selectedNode = null;
        nodeUI.SetTarget(tower.GetComponentInParent<Node>());
        nodeUI.gameObject.SetActive(true);
        nodeUI.transform.position = tower.transform.position;
    }

    public void DeselectNode()
    {
        selectedNode = null;
        selectedTower = null;
        nodeUI.Hide();
    }

    public void SelectTurretToBuild(TurretBlueprint turret)
    {
        turretToBuild = turret;
        selectedNode = null;
        selectedTower = null;
    }

    public TurretBlueprint GetTurretToBuild()
    {
        return turretToBuild;
    }
}
