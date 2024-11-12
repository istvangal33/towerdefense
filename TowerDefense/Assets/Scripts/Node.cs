using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour
{
    public Color hoverColor;
    public Color notEnoughMoneyColor;
    public Vector3 positionOffSet;

    public GameObject turret;
    public TurretBlueprint turretBlueprint;
    public bool isUpgraded = false;
    public int upgradeLevel = 0;

    private Renderer rend;
    private Color startColor;

    public bool isOccupied = false;

    private Node[] neighbors;

    BuildManager buildManager;

    void Start()
    {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;

        buildManager = BuildManager.instance;

        FindNeighbors();
    }

    void FindNeighbors()
    {
        neighbors = new Node[3];
        Node[] allNodes = FindObjectsOfType<Node>();

        // Get the grid's rotation
        Quaternion gridRotation = transform.parent.rotation;

        foreach (Node node in allNodes)
        {
            if (node == this) continue;

            // Calculate the relative position in local space
            Vector3 relativePos = Quaternion.Inverse(gridRotation) * (node.transform.position - transform.position);

            // Use a small tolerance for comparisons
            float tolerance = 0.1f;

            if (Mathf.Abs(relativePos.x) < tolerance && relativePos.z > 0 && Mathf.Abs(relativePos.z - GridCreator.cellSize) < tolerance) // Access cellSize from GridCreator
            {
                neighbors[0] = node;
            }
            else if (Mathf.Abs(relativePos.z) < tolerance && relativePos.x > 0 && Mathf.Abs(relativePos.x - GridCreator.cellSize) < tolerance) // Access cellSize from GridCreator
            {
                neighbors[1] = node;
            }
            else if (relativePos.x > 0 && relativePos.z > 0 && Mathf.Abs(relativePos.x - GridCreator.cellSize) < tolerance && Mathf.Abs(relativePos.z - GridCreator.cellSize) < tolerance) // Access cellSize from GridCreator
            {
                neighbors[2] = node;
            }
        }
    }

    public Vector3 GetBuildPosition()
    {
        return transform.position + positionOffSet;
    }

    public void UpgradeTurret()
    {
        if (upgradeLevel == 2)
        {
            Debug.Log("Maximum upgrade level reached!");
            return;
        }

        int upgradeCost = (upgradeLevel == 0) ? turretBlueprint.upgradeCost : turretBlueprint.upgradeCost2;
        GameObject upgradedPrefab = (upgradeLevel == 0) ? turretBlueprint.upgradedPrefab : turretBlueprint.upgradedPrefab2;

        if (PlayerStats.Money < upgradeCost)
        {
            Debug.Log("Not enough money to upgrade that!");
            return;
        }

        PlayerStats.Money -= upgradeCost;

        Destroy(turret);

        GameObject _turret = (GameObject)Instantiate(upgradedPrefab, GetBuildPosition(), Quaternion.identity);
        turret = _turret;


        Tower towerComponent = _turret.GetComponent<Tower>();
        if (towerComponent != null)
        {
            towerComponent.node = this;
        }

        upgradeLevel++;
        Debug.Log("Turret upgraded to level " + upgradeLevel + "!");

        BuildManager.instance.nodeUI.SetTarget(this);
    }

    public void SellTurret()
    {
        PlayerStats.Money += turretBlueprint.GetSellAmount(upgradeLevel);

        GameObject effect = (GameObject)Instantiate(buildManager.sellEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f);

        Destroy(turret);
        turretBlueprint = null;
        isUpgraded = false;
        isOccupied = false;


        if (neighbors[0] != null) neighbors[0].isOccupied = false;
        if (neighbors[1] != null) neighbors[1].isOccupied = false;
        if (neighbors[0] != null && neighbors[0].neighbors[1] != null) neighbors[0].neighbors[1].isOccupied = false;
        if (neighbors[1] != null && neighbors[1].neighbors[0] != null) neighbors[1].neighbors[0].isOccupied = false;

        Debug.Log("Turret sold!");
        upgradeLevel = 0;
    }

    void OnMouseDown()
    {
        Debug.Log("OnMouseDown() called for " + gameObject.name);

        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Mouse over UI element, exiting.");
            return;
        }

        if (isOccupied)
        {
            Debug.Log("Node is occupied, but NodeUI will not be shown.");
            return;
        }

        Debug.Log("Checking neighbors...");

        for (int i = 0; i < neighbors.Length; i++)
        {
            if (neighbors[i] != null)
            {
                Debug.Log("Neighbor " + i + " isOccupied: " + neighbors[i].isOccupied);
            }
        }

        if (
            (neighbors[0] != null && neighbors[0].isOccupied) ||
            (neighbors[1] != null && neighbors[1].isOccupied) ||
            (neighbors[0] != null && neighbors[0].neighbors[1] != null && neighbors[0].neighbors[1].isOccupied)
           )
        {
            Debug.Log("Cannot build here due to occupied neighbors.");
            return;
        }

        Debug.Log("Checking build manager...");
        if (buildManager.CanBuild)
        {
            Debug.Log("Building tower...");
            BuildTurret(buildManager.GetTurretToBuild());
            isOccupied = true;

            Debug.Log("Occupying neighbors...");

            if (neighbors[0] != null) neighbors[0].isOccupied = true;
            if (neighbors[1] != null) neighbors[1].isOccupied = true;
            if (neighbors[0] != null && neighbors[0].neighbors[1] != null) neighbors[0].neighbors[1].isOccupied = true;
        }
        else
        {
            Debug.Log("Cannot build: buildManager.CanBuild is false.");
        }
    }

    public void BuildTurret(TurretBlueprint blueprint)
    {
        if (PlayerStats.Money < blueprint.cost)
        {
            Debug.Log("Not enough money to build that!");
            return;
        }

        PlayerStats.Money -= blueprint.cost;

        GameObject _turret = (GameObject)Instantiate(blueprint.prefab, GetBuildPosition(), Quaternion.identity);
        turret = _turret;

        turretBlueprint = blueprint;
        isOccupied = true;


        Tower towerScript = _turret.GetComponent<Tower>();
        if (towerScript != null)
        {
            towerScript.node = this;
        }

        Debug.Log("Turret built!");
    }


    void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Debug.Log("Entering node. isOccupied: " + isOccupied);

        if (isOccupied || (neighbors[0] != null && neighbors[0].isOccupied) ||
            (neighbors[1] != null && neighbors[1].isOccupied) ||
            (neighbors[2] != null && neighbors[2].isOccupied))
        {
            Debug.Log("Neighbor or current node is occupied, showing red.");
            rend.material.color = notEnoughMoneyColor;
            return;
        }

        if (!buildManager.CanBuild)
            return;

        if (buildManager.HasMoney)
        {
            rend.material.color = hoverColor;
        }
        else
        {
            rend.material.color = notEnoughMoneyColor;
        }
    }

    void OnMouseExit()
    {
        rend.material.color = startColor;
    }
}
