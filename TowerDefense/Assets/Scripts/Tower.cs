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

    // Hozz�adjuk a forgat�s scriptet
    private RotationScript rotationScript;

    void Start()
    {
        // Forgat�s script komponens keres�se
        rotationScript = GetComponent<RotationScript>();
    }

    void Update()
    {
        FindTarget();  // C�lpont keres�se

        if (rotationScript == null)
        {
            Debug.LogError("Nincs RotationScript komponens a tornyon!");
            return;
        }

        if (target != null)
        {
            // Forgat�s az ellenf�l ir�ny�ba (csak az y tengelyen)
            Vector3 direction = target.position - transform.position;
            direction.y = 0; // Nem akarjuk, hogy a torony fel-le is forduljon

            rotationScript.RotateObjectTowards(direction);

            // Csak akkor l�, ha van c�lpont �s a visszasz�ml�l�s lej�rt
            if (fireCountdown <= 0f)
            {
                Shoot();
                fireCountdown = 1f / fireRate;
            }
        }
        else
        {
            Debug.Log("Nincs c�lpont az Update-ben.");
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

        Debug.Log("Tal�lt ellens�gek sz�ma: " + enemies.Length);  // Ki�rja, h�ny ellens�get tal�lt a p�ly�n

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            Debug.Log("T�vols�g az ellens�gt�l: " + distanceToEnemy);  // Ki�rja az ellens�g t�vols�g�t

            if (distanceToEnemy < shortestDistance && distanceToEnemy <= range)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null)
        {
            target = nearestEnemy.transform;
            Debug.Log("C�lpont megtal�lva: " + target.name); // Ellen�rz�s c�lponttal
        }
        else
        {
            target = null;
            Debug.Log("Nincs c�lpont a hat�t�vban.");
        }
    }



    // Megjelen�t�shez (gizmos) a torony hat�t�vja
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
