using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;

    public float speed = 200f;
    public int damage = 50;
    public GameObject impactEffect;
    public bool hasExploded = false;

    public enum ProjectileType { Bullet, Rocket }
    public ProjectileType projectileType; 

    public void Seek(Transform _target)
    {
        target = _target;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }

    void HitTarget()
    {
        GameObject effectIns = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectIns, 2f);

        if (projectileType == ProjectileType.Bullet) 
        {
            Damage(target); 
        }

        hasExploded = true;

        Destroy(gameObject);
    }

    void Damage(Transform enemy)
    {
        EnemyAI e = enemy.GetComponent<EnemyAI>();

        if (e != null)
        {
            e.TakeDamage(damage);
        }
    }
}