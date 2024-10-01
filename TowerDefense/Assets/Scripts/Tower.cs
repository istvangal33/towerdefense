using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public float range = 5.0f;      // Torony hat�t�vols�ga
    private Transform target;       // Az aktu�lis c�lpont (ellens�g)

    public float fireRate = 2f;
    private float fireCountdown = 0f;

    public GameObject bulletPrefab;
    public Transform firePoint;

    void Update()
    {
        FindTarget();  // Ellenf�l keres�se

        if (target != null)
        {
            // Forgat�s az ellenf�l ir�ny�ba (csak az y tengelyen)
            Vector3 direction = target.position - transform.position;
            direction.y = 0; // Nem akarjuk, hogy a torony fel-le is forduljon

            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            // Csak akkor l�, ha van c�lpont
            if (fireCountdown <= 0f)
            {
                Shoot();
                fireCountdown = 1f / fireRate;
            }
        }

        // Csak akkor cs�kkenti a visszasz�ml�l�st, ha m�r l�het
        if (fireCountdown > 0f)
        {
            fireCountdown -= Time.deltaTime;
        }
    }


    void Shoot()
    {
        GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.Seek(target);
            Debug.Log("L�ved�k kil�ve: " + bullet.name + ", c�lpont: " + target.name);
        }
        else
        {
            Debug.LogError("A l�ved�k prefab nem tartalmaz Bullet komponenst!");
        }
    }

    // C�lpont keres�se
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

    // Megjelen�t�shez (gizmos) a torony hat�t�vja
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
