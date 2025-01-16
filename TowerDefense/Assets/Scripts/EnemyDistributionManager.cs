using System.Collections.Generic;
using UnityEngine;

public class EnemyDistributionManager : MonoBehaviour
{
    public float cellSize = 1.0f; // A grid cellák mérete
    public string roadTag = "Road"; 

    private List<GameObject> walkableObjects = new List<GameObject>();

    void Start()
    {
        InitializeWalkableObjects();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) 
        {
            LogEnemyDistribution();
        }
    }

    private void InitializeWalkableObjects()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag(roadTag)) // Tag checkup
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                Collider collider = obj.GetComponent<Collider>();

                if (renderer == null && collider == null)
                {
                    Debug.LogError($"Nincs Renderer vagy Collider a {obj.name} objektumon!");
                    continue;
                }

                walkableObjects.Add(obj);
                Debug.Log($"Járható objektum hozzáadva: {obj.name}");
            }
        }

        if (walkableObjects.Count == 0)
        {
            Debug.LogError("Nem található egyetlen 'Road' taggel rendelkezõ objektum sem!");
        }
    }

    public void LogEnemyDistribution()
    {
        Dictionary<Vector3, int> enemyDistribution = CalculateEnemyDistribution();

        if (enemyDistribution.Count == 0)
        {
            Debug.LogError("Az eloszlási adatok üresek! Ellenõrizd a grid generálást és az ellenfelek pozícióját!");
            return;
        }

        Debug.Log($"Ellenfelek eloszlása ({enemyDistribution.Count} cella):");
        foreach (var sector in enemyDistribution)
        {
            Debug.Log($"Cella középpontja: {sector.Key}, Ellenfelek száma: {sector.Value}");
        }
    }

    public Dictionary<Vector3, int> CalculateEnemyDistribution()
    {
        Dictionary<Vector3, int> enemyDistribution = new Dictionary<Vector3, int>();

        Bounds totalBounds = CalculateTotalBounds();
        List<Vector3> gridPoints = GenerateGrid(cellSize, totalBounds);

        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();

        foreach (EnemyAI enemy in enemies)
        {
            Vector3 enemyPosition = enemy.transform.position;
            bool foundCell = false;

            foreach (Vector3 cellCenter in gridPoints)
            {
                float halfCellSize = cellSize / 2;
                if (Mathf.Abs(enemyPosition.x - cellCenter.x) <= halfCellSize &&
                    Mathf.Abs(enemyPosition.z - cellCenter.z) <= halfCellSize)
                {
                    if (!enemyDistribution.ContainsKey(cellCenter))
                    {
                        enemyDistribution[cellCenter] = 0;
                    }

                    enemyDistribution[cellCenter]++;
                    foundCell = true;
                    Debug.Log($"Ellenfél {enemy.name} (Pozíció: {enemyPosition}) a cellában: {cellCenter}");
                    break;
                }
            }

            if (!foundCell)
            {
                Debug.LogWarning($"Ellenfél {enemy.name} (Pozíció: {enemyPosition}) nem tartozik egyik cellához sem!");
            }
        }

        return enemyDistribution;
    }

    public List<Vector3> GenerateGrid(float cellSize, Bounds bounds)
    {
        List<Vector3> gridPoints = new List<Vector3>();

        for (float x = bounds.min.x; x <= bounds.max.x; x += cellSize)
        {
            for (float z = bounds.min.z; z <= bounds.max.z; z += cellSize)
            {
                gridPoints.Add(new Vector3(x + cellSize / 2, bounds.center.y, z + cellSize / 2)); 
            }
        }

        return gridPoints;
    }

    public Bounds CalculateTotalBounds()
    {
        Bounds totalBounds = new Bounds();
        bool hasValidBounds = false;

        foreach (GameObject obj in walkableObjects)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            Collider collider = obj.GetComponent<Collider>();

            if (renderer != null)
            {
                totalBounds.Encapsulate(renderer.bounds);
                hasValidBounds = true;
                Debug.Log($"Renderer alapján hozzáadva: {obj.name}");
            }
            else if (collider != null)
            {
                totalBounds.Encapsulate(collider.bounds);
                hasValidBounds = true;
                Debug.LogWarning($"Collider alapján hozzáadva: {obj.name}");
            }
        }

        if (!hasValidBounds)
        {
            Debug.LogError("Nincs érvényes Renderer vagy Collider komponenssel rendelkezõ objektum a pályán!");
        }

        return totalBounds;
    }
}
