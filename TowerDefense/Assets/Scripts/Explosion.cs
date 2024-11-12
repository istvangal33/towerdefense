using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float explosionRadius = 2f;
    public int explosionDamage = 25;
    public GameObject explosionEffect;

    void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.tag == "Enemy")
            {
                EnemyAI enemy = collider.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    enemy.TakeDamage(explosionDamage);

                    
                    if (explosionEffect != null)
                    {
                        Instantiate(explosionEffect, enemy.transform.position, Quaternion.identity);
                    }
                }
            }
        }

        Destroy(gameObject);
    }
}