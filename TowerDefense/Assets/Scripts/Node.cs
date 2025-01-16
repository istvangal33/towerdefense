using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;


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

    public int gridX;
    public int gridY;
    public List<Vector2Int> coveredCells = new List<Vector2Int>();

    public float range;

    public void CalculateCoverage()
    {
        coveredCells.Clear();
        int gridRange = Mathf.CeilToInt(range / GridCreator.cellSize);

        for (int x = -gridRange; x <= gridRange; x++)
        {
            for (int y = -gridRange; y <= gridRange; y++)
            {
                Vector2Int cell = new Vector2Int(
                    Mathf.RoundToInt(transform.position.x / GridCreator.cellSize) + x,
                    Mathf.RoundToInt(transform.position.z / GridCreator.cellSize) + y
                );
                if (Vector2.Distance(new Vector2(cell.x, cell.y), new Vector2(transform.position.x, transform.position.z)) <= range / GridCreator.cellSize)
                {
                    coveredCells.Add(cell);
                }
            }
        }
    }

    void Start()
    {

        CalculateGridPosition();
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;

        buildManager = BuildManager.instance;

        FindNeighbors();
    }

    void FindNeighbors()
    {
        neighbors = new Node[3];
        Node[] allNodes = FindObjectsOfType<Node>();


        Quaternion gridRotation = transform.parent.rotation;

        foreach (Node node in allNodes)
        {
            if (node == this) continue;


            Vector3 relativePos = Quaternion.Inverse(gridRotation) * (node.transform.position - transform.position);


            float tolerance = 0.1f;

            if (Mathf.Abs(relativePos.x) < tolerance && relativePos.z > 0 && Mathf.Abs(relativePos.z - GridCreator.cellSize) < tolerance)
            {
                neighbors[0] = node;
            }
            else if (Mathf.Abs(relativePos.z) < tolerance && relativePos.x > 0 && Mathf.Abs(relativePos.x - GridCreator.cellSize) < tolerance)
            {
                neighbors[1] = node;
            }
            else if (relativePos.x > 0 && relativePos.z > 0 && Mathf.Abs(relativePos.x - GridCreator.cellSize) < tolerance && Mathf.Abs(relativePos.z - GridCreator.cellSize) < tolerance)
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

        GameObject _turret = Instantiate(upgradedPrefab, GetBuildPosition(), Quaternion.identity);
        turret = _turret;

        Tower towerComponent = _turret.GetComponent<Tower>();
        if (towerComponent != null)
        {
            towerComponent.node = this;
            towerComponent.currentLevel = ++upgradeLevel;
            towerComponent.CalculateCoverage();
        }

        Debug.Log($"Turret upgraded to level {upgradeLevel}! Cost: {upgradeCost}");
    }




    public void SellTurret()
    {
        SoundManager.Instance.PlayExplosionSound();

        
        PlayerStats.Money += turretBlueprint.GetSellAmount(upgradeLevel);

        
        GameObject effect = (GameObject)Instantiate(buildManager.sellEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f);

        
        if (turret != null)
        {
            Tower towerScript = turret.GetComponent<Tower>();
            if (towerScript != null)
            {
               
                towerScript.coveredCells.Clear();
            }

            Destroy(turret);
        }

        
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
            Debug.Log("Node is already occupied.");
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
        if (buildManager.CanBuild && buildManager.HasMoney)
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
            Debug.Log("Cannot build: either no turret selected or not enough money.");
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

        GameObject _turret = Instantiate(blueprint.prefab, GetBuildPosition(), Quaternion.identity);
        turret = _turret;

        turretBlueprint = blueprint;
        isOccupied = true;

        Tower towerScript = _turret.GetComponent<Tower>();
        if (towerScript != null)
        {
            towerScript.node = this;
            towerScript.CalculateCoverage();
        }


        SoundManager.Instance.PlayBuildSound();

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

    void CalculateGridPosition()
    {
        gridX = Mathf.RoundToInt(transform.position.x / GridCreator.cellSize);
        gridY = Mathf.RoundToInt(transform.position.z / GridCreator.cellSize);
    }
}
