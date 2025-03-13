using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public enum TowerType { MachineGun, Rocket, Laser }

public class Tower : MonoBehaviour
{
    public Vector3 positionOffset;

    public Node node;
    
    public List<Vector2Int> coveredCells = new List<Vector2Int>();
    public TowerType towerType;
    private Transform target;

    public float range = 5.0f;
    public Transform endPoint;

    public float fireRate = 2f;
    private float fireCountdown = 0f;

    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Laser Settings")]
    public bool useLaser = false;
    public LineRenderer lineRenderer;
    public float laserDamagePerSecond = 10f;

    [Header("General Settings")]
    public Transform rotatingPart;
    public float rotationSpeed = 5f;
    public float angleThreshold = 5f;


    public int upgradeCost = 8;
    public int sellAmount = 2;
    public bool isMaxLevel = false;

    public int currentLevel = 1;
    public int maxLevel = 3;
    private AudioSource audioSource;

    void Start()
    {
        if (rotatingPart == null)
        {
            Debug.LogError("A forgó rész nincs hozzárendelve a toronyhoz!");
        }

        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }

        GameObject endObject = GameObject.Find("End");
        if (endObject != null)
        {
            endPoint = endObject.transform;
        }
        else
        {
            Debug.LogError("Nem található End GameObject a jelenetben!");
        }

        transform.position += positionOffset;

        audioSource = GetComponent<AudioSource>();

        
        if (node != null)
        {
            currentLevel = node.upgradeLevel; 
        }
        else
        {
            currentLevel = 1; 
        }

        Debug.Log($"Tower initialized with level: {currentLevel}");
    }


    void Update()
    {
        FindTarget();

        if (target != null)
        {
            Vector3 direction = target.position - transform.position;
            direction.y = 0;

            RotateTowards(direction);

            if (useLaser)
            {
                Laser();
            }
            else
            {
                float angleToTarget = Vector3.Angle(rotatingPart.forward, direction);

                if (angleToTarget < angleThreshold && fireCountdown <= 0f)
                {
                    Shoot();
                    fireCountdown = 1f / fireRate;
                }
            }
        }

        if (fireCountdown > 0f)
        {
            fireCountdown -= Time.deltaTime;
        }
    }

    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (node != null)
        {
            BuildManager.instance.SelectNode(node);
        }
        else
        {
            Debug.LogError("The target Node is null!");
        }

    }

    void RotateTowards(Vector3 direction)
    {
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(rotatingPart.rotation, lookRotation, Time.deltaTime * rotationSpeed).eulerAngles;
        rotatingPart.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    void Shoot()
    {
        GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.Seek(target);


            if (towerType == TowerType.Rocket)
            {
                Explosion explosion = bulletGO.AddComponent<Explosion>();
                explosion.explosionRadius = 2f;
                explosion.explosionDamage = bullet.damage;
            }
        }
        else
        {
            Debug.LogError("A lövedék prefab nem tartalmaz Bullet komponenst!");
        }


        PlayTowerShootSound();
    }

    void PlayTowerShootSound()
    {
        switch (towerType)
        {
            case TowerType.MachineGun:
                SoundManager.Instance.PlayMachineGunShot();
                break;
            case TowerType.Rocket:
                SoundManager.Instance.PlayRocketLaunch();
                break;
            case TowerType.Laser:
                SoundManager.Instance.PlayLaserShoot();
                break;
        }
    }



    void Laser()
    {
        if (target == null || !target.gameObject.activeSelf)
        {
            lineRenderer.enabled = false;
            SoundManager.Instance.StopLaserShoot();
            return;
        }

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, target.position);

        
        if (!SoundManager.Instance.laserAudioSource.isPlaying)
        {
            SoundManager.Instance.PlayLaserShoot();
        }

        
        EnemyAI enemyAI = target.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.TakeDamage(laserDamagePerSecond * Time.deltaTime);
            enemyAI.Slow(0.25f);
        }
    }


    void FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistanceToEnd = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();

            if (enemyAI != null && IsTargetAllowed(enemyAI.enemyType))
            {
                float distanceToEnd = Vector3.Distance(enemy.transform.position, endPoint.position);

                if (distanceToEnd < shortestDistanceToEnd && Vector3.Distance(transform.position, enemy.transform.position) <= range)
                {
                    shortestDistanceToEnd = distanceToEnd;
                    nearestEnemy = enemy;
                }
            }
        }

        if (nearestEnemy != null)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;

            if (useLaser && lineRenderer != null)
            {
                lineRenderer.enabled = false;
                SoundManager.Instance.StopLaserShoot();
            }
        }
    }




    bool IsTargetAllowed(EnemyType enemyType)
    {

        switch (towerType)
        {
            case TowerType.MachineGun:
            case TowerType.Laser:
                return true;

            case TowerType.Rocket:
                return enemyType == EnemyType.Buggy || enemyType == EnemyType.Hovertank;

            default:
                return false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    public void Upgrade()
    {
        if (!isMaxLevel && PlayerStats.Money >= upgradeCost)
        {

            PlayerStats.Money -= upgradeCost;


            fireRate *= 1.2f;
            range *= 1.1f;


            upgradeCost = (int)(upgradeCost * 1.5f);


            currentLevel++;


            if (currentLevel >= maxLevel)
            {
                isMaxLevel = true;
                Debug.Log("Tower is at max level!");
            }
        }
        else
        {
            Debug.Log("Not enough money or tower is at max level!");
        }
    }

    public void Sell()
    {
        PlayerStats.Money += sellAmount;
        SoundManager.Instance.StopLaserShoot();
        Destroy(gameObject);
    }

    public void CalculateCoverage()
    {
        if (node == null)
        {
            Debug.LogError("Node is not set for this tower!");
            return;
        }

        coveredCells.Clear();
        int gridRange = Mathf.CeilToInt(range / GridCreator.cellSize);

        for (int x = -gridRange; x <= gridRange; x++)
        {
            for (int y = -gridRange; y <= gridRange; y++)
            {
                Vector2Int cell = new Vector2Int(node.gridX + x, node.gridY + y);
                if (Vector2.Distance(new Vector2(cell.x, cell.y), new Vector2(node.gridX, node.gridY)) <= range / GridCreator.cellSize)
                {
                    coveredCells.Add(cell);
                }
            }
        }

        Debug.Log($"Coverage calculated for tower at {node.gridX}, {node.gridY}");
    }
}