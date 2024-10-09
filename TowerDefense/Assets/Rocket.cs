using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public GameObject explosionEffect;  // Az effekt prefabja
    public float explosionDuration = 2f;  // Mennyi ideig tart az effekt

    void OnCollisionEnter(Collision collision)
    {
        // Ellenõrizzük, hogy az ellenfelet találtuk-e el
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Robbanási effekt indítása
            Explode();
        }
    }

    void Explode()
    {
        // Robbanási effekt létrehozása az aktuális pozíciónál
        GameObject explosion = Instantiate(explosionEffect, transform.position, transform.rotation);
        
        // Az effekt eltávolítása az explosionDuration idõ után
        Destroy(explosion, explosionDuration);

        // A rakéta megsemmisítése
        Destroy(gameObject);
    }
}
