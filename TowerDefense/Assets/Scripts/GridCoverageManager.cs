using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GridCoverageManager : MonoBehaviour
{
    public List<GameObject> walkableObjects = new List<GameObject>();
    public Dictionary<GameObject, List<Vector3>> pathPoints = new Dictionary<GameObject, List<Vector3>>();
    private Dictionary<string, float> maxCoveragePercentages = new Dictionary<string, float>
    {
        { "LVL1", 64.0f },
        { "LVL2", 87.5f },
        { "LVL3", 80.0f },
        { "LVL4", 74.0f },
        { "LVL5", 100.0f }
    };

    private Dictionary<string, int> maxTowerSpotsByLevel = new Dictionary<string, int>
    {
        { "LVL1", 23 },
        { "LVL2", 12 },
        { "LVL3", 28 },
        { "LVL4", 54 }
    };


    private int totalTowerSpots = 0;


    void Start()
    {
        GameObject[] paths = GameObject.FindGameObjectsWithTag("Road");
        foreach (GameObject path in paths)
        {
            walkableObjects.Add(path);
        }

        foreach (GameObject path in walkableObjects)
        {
            if (path != null)
            {
                List<Vector3> points = GeneratePathPoints(path, 10);
                pathPoints[path] = points;
                Debug.Log($"Mintapontok generalva a(z) {path.name} objektumhoz: {points.Count} pont.");
            }
            else
            {
                Debug.LogWarning("Null path objektum talalhato a walkableObjects listaban!");
            }
        }

        Debug.Log($"osszes path inicializalva: {pathPoints.Count}");

        
        CountTotalTowerSpots();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            string currentLevelName = SceneManager.GetActiveScene().name;
            if (maxCoveragePercentages.TryGetValue(currentLevelName, out float maxCoverage))
            {
                CalculateCoverage(maxCoverage);
            }
            else
            {
                Debug.LogError($"Nincs maximalis lefedettségi adat a(z) {currentLevelName} palyahoz!");
            }
        }
    }

    public List<Vector3> GeneratePathPoints(GameObject path, int sampleCount)
    {
        List<Vector3> points = new List<Vector3>();
        Renderer renderer = path.GetComponent<Renderer>();

        if (renderer != null)
        {
            Bounds bounds = renderer.bounds;

            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / (sampleCount - 1);
                Vector3 point = Vector3.Lerp(bounds.min, bounds.max, t);
                points.Add(point);
            }
        }
        else
        {
            Debug.LogWarning($"Nincs Renderer komponens a {path.name} objektumon! Pontokat nem lehet generalni.");
        }

        return points;
    }

    public void CalculateCoverage(float maxCoveragePercentage)
    {
        HashSet<Vector3> coveredPoints = new HashSet<Vector3>();

        Tower[] towers = FindObjectsOfType<Tower>();

        foreach (Tower tower in towers)
        {
            foreach (var path in pathPoints)
            {
                foreach (Vector3 point in path.Value)
                {
                    float distance = Vector3.Distance(tower.transform.position, point);
                    if (distance <= tower.range)
                    {
                        coveredPoints.Add(point);
                    }
                }
            }
        }

        int totalPoints = 0;
        foreach (var path in pathPoints)
        {
            totalPoints += path.Value.Count;
        }

        float coveragePercentage = (float)coveredPoints.Count / totalPoints * 100f;
        float normalizedCoverage = (coveragePercentage / maxCoveragePercentage) * 100f;

        Debug.Log($"Lefedett pontok: {coveredPoints.Count}/{totalPoints} ({coveragePercentage}%)");
        Debug.Log($"Normalizalt lefedettseg: {normalizedCoverage}%");

        foreach (Vector3 point in coveredPoints)
        {
            Debug.Log($"Lefedett pont: {point}");
        }
    }

    public float GetCurrentCoverage()
    {
        HashSet<Vector3> coveredPoints = new HashSet<Vector3>();

        Tower[] towers = FindObjectsOfType<Tower>();

        foreach (Tower tower in towers)
        {
            foreach (var path in pathPoints)
            {
                foreach (Vector3 point in path.Value)
                {
                    float distance = Vector3.Distance(tower.transform.position, point);
                    if (distance <= tower.range)
                    {
                        coveredPoints.Add(point);
                    }
                }
            }
        }

        int totalPoints = 0;
        foreach (var path in pathPoints)
        {
            totalPoints += path.Value.Count;
        }

        float coveragePercentage = (float)coveredPoints.Count / totalPoints * 100f;
        string currentLevelName = SceneManager.GetActiveScene().name;

        if (maxCoveragePercentages.TryGetValue(currentLevelName, out float maxCoverage))
        {
            float normalizedCoverage = (coveragePercentage / maxCoverage) * 100f;
            Debug.Log($"Normalized Coverage: {normalizedCoverage}%");
            return normalizedCoverage;
        }
        else
        {
            Debug.LogError($"No max coverage data for level {currentLevelName}");
            return 0f;
        }
    }

    
    void CountTotalTowerSpots()
    {
        string currentLevelName = SceneManager.GetActiveScene().name;

        if (maxTowerSpotsByLevel.TryGetValue(currentLevelName, out int maxSpots))
        {
            totalTowerSpots = maxSpots;
            Debug.Log($"osszes lehetseges toronyhely ({currentLevelName}): {totalTowerSpots}");
        }
        else
        {
            Debug.LogError($"Nincs megadva toronyhely adat a(z) {currentLevelName} szinthez!");
            totalTowerSpots = 0; 
        }
    }


    public float GetCoverageByTower()
    {
        
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        int placedTowers = towers.Length;

        
        if (totalTowerSpots == 0)
        {
            Debug.LogWarning("A totalTowerSpots erteke 0, lefedettseg nem szamolhato.");
            return 0f;
        }

        
        float coverageByTower = (float)placedTowers / totalTowerSpots * 100f;

       
        Debug.Log($"Toronyhely lefedettseg: {coverageByTower:F2}% ({placedTowers}/{totalTowerSpots})");

        return coverageByTower;
    }


}
