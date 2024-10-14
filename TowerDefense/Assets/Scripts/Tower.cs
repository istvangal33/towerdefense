﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private Transform target;   // Az aktuális célpont (ellenség)

    public float range = 5.0f;  // Torony hatótávolsága

    public float fireRate = 2f;
    private float fireCountdown = 0f;

    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Laser Settings")]
    public bool useLaser = false;  // Döntsd el, hogy lézert vagy golyókat használjon-e
    public LineRenderer lineRenderer;  // LineRenderer a lézerhez
    public float laserDamagePerSecond = 10f;  // A lézer sebzése

    [Header("General Settings")]
    public Transform rotatingPart;  // Ez lesz az a rész, ami mozog/forog
    public float rotationSpeed = 5f; // Forgás sebessége
    public float angleThreshold = 5f; // Ha a szögkülönbség ennél kisebb, akkor lõhet

    void Start()
    {
        if (rotatingPart == null)
        {
            Debug.LogError("A forgó rész nincs hozzárendelve a toronyhoz!");
        }

        // Kapcsold ki a LineRenderer-t, ha nem használunk lézert
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }

    void Update()
    {
        FindTarget();  // Célpont keresése

        if (target != null)
        {
            // Forgatás az ellenfél irányába (csak az y tengelyen)
            Vector3 direction = target.position - transform.position;
            direction.y = 0; // Nem akarjuk, hogy a torony fel-le is forduljon

            // Forgatjuk a rotatingPart-ot az ellenség felé
            RotateTowards(direction);

            if (useLaser)
            {
                Laser(); // Lézer használata
            }
            else
            {
                // Ellenõrizzük, hogy a torony eléggé ráfordult-e az ellenségre
                float angleToTarget = Vector3.Angle(rotatingPart.forward, direction);

                if (angleToTarget < angleThreshold)
                {
                    // Csak akkor lõ, ha a torony eléggé ráfordult az ellenségre és a visszaszámlálás lejárt
                    if (fireCountdown <= 0f)
                    {
                        Shoot();
                        fireCountdown = 1f / fireRate;
                    }
                }
            }
        }

        // Csak akkor csökkenti a visszaszámlálást, ha már lõhet
        if (fireCountdown > 0f)
        {
            fireCountdown -= Time.deltaTime;
        }
    }

    void RotateTowards(Vector3 direction)
    {
        // Forgatás csak a rotatingPart-ra alkalmazva
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(rotatingPart.rotation, lookRotation, Time.deltaTime * rotationSpeed).eulerAngles;
        rotatingPart.rotation = Quaternion.Euler(0f, rotation.y, 0f);  // Csak az y tengelyen forgatjuk
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
        // Ha nincs célpont, akkor kapcsoljuk ki a lézert
        if (target == null)
        {
            lineRenderer.enabled = false;
            return;
        }

        // Ha van célpont, kapcsoljuk be a lézert
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, target.position);

        // Sebzés az ellenségre (ha lézer sebzést szeretnél)
        EnemyAI enemyAI = target.GetComponent<EnemyAI>();  // Használjuk az EnemyAI osztályt
        if (enemyAI != null)
        {
            enemyAI.TakeDamage(laserDamagePerSecond * Time.deltaTime); // Sebzés idő alapú
        }
    }



    // Célpont keresése
    void FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy < shortestDistance && distanceToEnemy <= range)
            {
                shortestDistance = distanceToEnemy;
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
        }
    }

    // Megjelenítéshez (gizmos) a torony hatótávja
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
