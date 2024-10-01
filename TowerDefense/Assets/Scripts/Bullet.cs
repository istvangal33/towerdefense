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

        Debug.Log("L�ved�k t�vols�g a c�lhoz: " + dir.magnitude);
        Debug.Log("L�ved�k mozg�si t�vols�g: " + distanceThisFrame);

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

        // Elpuszt�tjuk az ellens�get
        Destroy(target.gameObject);

        // Elpuszt�tjuk a l�ved�ket
        Destroy(gameObject);
    }

    // �tk�z�s �rz�kel�se
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("�tk�z�s t�rt�nt az: " + collision.transform.name);

        if (collision.transform == target)
        {
            HitTarget();  // Ha az �tk�z�s az ellens�ggel t�rt�nt, megh�vjuk a tal�latot kezel� met�dust
        }
    }
}
