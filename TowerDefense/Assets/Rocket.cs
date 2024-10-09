using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public GameObject explosionEffect;  // Az effekt prefabja
    public float explosionDuration = 2f;  // Mennyi ideig tart az effekt

    void OnCollisionEnter(Collision collision)
    {
        // Ellen�rizz�k, hogy az ellenfelet tal�ltuk-e el
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Robban�si effekt ind�t�sa
            Explode();
        }
    }

    void Explode()
    {
        // Robban�si effekt l�trehoz�sa az aktu�lis poz�ci�n�l
        GameObject explosion = Instantiate(explosionEffect, transform.position, transform.rotation);
        
        // Az effekt elt�vol�t�sa az explosionDuration id� ut�n
        Destroy(explosion, explosionDuration);

        // A rak�ta megsemmis�t�se
        Destroy(gameObject);
    }
}
