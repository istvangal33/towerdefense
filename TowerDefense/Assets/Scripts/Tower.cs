using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TowerType { MachineGun, Rocket, Laser }

public class Tower : MonoBehaviour
{
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
        }
        else
        {
            Debug.LogError("A lövedék prefab nem tartalmaz Bullet komponenst!");
        }
    }

    void Laser()
    {
        if (target == null)
        {
            lineRenderer.enabled = false;
            return;
        }

        if (!target.gameObject.activeSelf)
        {
            lineRenderer.enabled = false;
            target = null;
            return;
        }

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, target.position);

        EnemyAI enemyAI = target.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.TakeDamage(laserDamagePerSecond * Time.deltaTime);
        }
    }

    void FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistanceToEnd = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnd = Vector3.Distance(enemy.transform.position, endPoint.position);

            if (distanceToEnd < shortestDistanceToEnd && Vector3.Distance(transform.position, enemy.transform.position) <= range)
            {
                shortestDistanceToEnd = distanceToEnd;
                nearestEnemy = enemy;
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
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
