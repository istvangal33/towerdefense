using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public float range = 5.0f;  // Torony hat�t�vols�ga
    private Transform target;   // Az aktu�lis c�lpont (ellens�g)

    public float fireRate = 2f;
    private float fireCountdown = 0f;

    public GameObject bulletPrefab;
    public Transform firePoint;

    public Transform rotatingPart;  // Ez lesz az a r�sz, ami mozog/forog

    public float rotationSpeed = 5f; // Forg�s sebess�ge
    public float angleThreshold = 5f; // Ha a sz�gk�l�nbs�g enn�l kisebb, akkor l�het

    void Start()
    {
        if (rotatingPart == null)
        {
            Debug.LogError("A forg� r�sz nincs hozz�rendelve a toronyhoz!");
        }
    }

    void Update()
    {
        FindTarget();  // C�lpont keres�se

        if (target != null)
        {
            // Forgat�s az ellenf�l ir�ny�ba (csak az y tengelyen)
            Vector3 direction = target.position - transform.position;
            direction.y = 0; // Nem akarjuk, hogy a torony fel-le is forduljon

            // Forgatjuk a rotatingPart-ot az ellens�g fel�
            RotateTowards(direction);

            // Ellen�rizz�k, hogy a torony el�gg� r�fordult-e az ellens�gre
            float angleToTarget = Vector3.Angle(rotatingPart.forward, direction);

            if (angleToTarget < angleThreshold)
            {
                // Csak akkor l�, ha a torony el�gg� r�fordult az ellens�gre �s a visszasz�ml�l�s lej�rt
                if (fireCountdown <= 0f)
                {
                    Shoot();
                    fireCountdown = 1f / fireRate;
                }
            }
        }

        // Csak akkor cs�kkenti a visszasz�ml�l�st, ha m�r l�het
        if (fireCountdown > 0f)
        {
            fireCountdown -= Time.deltaTime;
        }
    }

    void RotateTowards(Vector3 direction)
    {
        // Forgat�s csak a rotatingPart-ra alkalmazva
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
