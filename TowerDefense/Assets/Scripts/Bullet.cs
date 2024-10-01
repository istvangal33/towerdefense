using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;

    public float speed = 200f;

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

        Debug.Log("Lövedék távolság a célhoz: " + dir.magnitude);
        Debug.Log("Lövedék mozgási távolság: " + distanceThisFrame);

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }

    void HitTarget()
    {
        // Létrehozzuk a robbanás effektet
        GameObject effectIns = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectIns, 2f);

        // Elpusztítjuk az ellenséget
        Destroy(target.gameObject);

        // Elpusztítjuk a lövedéket
        Destroy(gameObject);
    }

    // Ütközés érzékelése
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Ütközés történt az: " + collision.transform.name);

        if (collision.transform == target)
        {
            HitTarget();  // Ha az ütközés az ellenséggel történt, meghívjuk a találatot kezelõ metódust
        }
    }
}
