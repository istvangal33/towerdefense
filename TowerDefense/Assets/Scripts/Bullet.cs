using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;

    public float speed = 200f;
    public int damage = 50;
    public GameObject impactEffect;


    public void Seek(Transform _target)
    {
        target = _target;
    }

    // Update is called once per frame
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
        // L�trehozzuk a robban�s effektet
        GameObject effectIns = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectIns, 2f);

        // Sebz�st okozunk az ellens�gnek
        if (target != null)
        {
            Damage(target);
        }

        // Elpuszt�tjuk a l�ved�ket
        Destroy(gameObject);
    }

    void Damage(Transform enemy)
    {
        // Az ellens�g keres�se az EnemyAI oszt�ly alapj�n
        EnemyAI e = enemy.GetComponent<EnemyAI>();

        if (e != null)
        {
            e.TakeDamage(damage); // Itt �tadjuk a 'damage' �rt�ket az 'amount' param�terhez
        }
    }

}
