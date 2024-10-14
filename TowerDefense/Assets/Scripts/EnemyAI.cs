using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    public float startHealth = 100;
    private float health;
    public int value = 1;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float heightOffset = 0f; // Magasság állítása
    [SerializeField] private float arrivalThreshold = 0.5f; // Ekkora távolság esetén tekintjük célba értnek
    private Transform choiceTarget;  // Elágazási pont célja
    private Transform endTarget;     // Végsõ célpont
    private Transform altEndTarget;  // Alternatív végsõ célpont

    public GameObject deathEffect;
    public GameObject explosionPrefab; // A robbanás prefab
    private bool isAtEnd = false; // Jelzi, hogy az ellenség elérte az "End" objektumot
    private bool hasMadeChoice = false; // Jelzi, hogy az elágazásnál már döntött

    [Header("Unity Staff")]
    public Image healthBar;

    [SerializeField] private float movementSpeed = 3.5f; // A sebesség beállítása
    [SerializeField] private float acceleration = 8f;    // Gyorsulás beállítása
    [SerializeField] private float angularSpeed = 120f;  // Forgási sebesség beállítása
    [SerializeField] private float stoppingDistance = 0.2f; // Megállási távolság

    private void Start()
    {
        // Keressük meg a "Choice", "End" és "AltEnd" nevû GameObjecteket a pályán
        GameObject choiceObject = GameObject.Find("Choice");
        GameObject endObject = GameObject.Find("End");
        GameObject altEndObject = GameObject.Find("AltEnd");

        if (choiceObject != null)
        {
            choiceTarget = choiceObject.transform;
            Debug.Log("Choice megtalálva: " + choiceTarget.position);
        }
        else
        {
            Debug.LogError("No GameObject named 'Choice' found in the scene.");
        }

        if (endObject != null)
        {
            endTarget = endObject.transform;
            Debug.Log("End megtalálva: " + endTarget.position);
        }
        else
        {
            Debug.LogError("No GameObject named 'End' found in the scene.");
        }

        if (altEndObject != null)
        {
            altEndTarget = altEndObject.transform;
            Debug.Log("AltEnd megtalálva: " + altEndTarget.position);
        }
        else
        {
            Debug.LogError("No GameObject named 'AltEnd' found in the scene.");
        }

        // Kezdjük az útvonalat a Choice felé
        if (choiceTarget != null)
        {
            agent.SetDestination(choiceTarget.position);
        }
        health = startHealth;
    }


    public void TakeDamage(float amount)
    {
        health -= amount;
        healthBar.fillAmount = health / startHealth;


        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Ellenség meghalt, effekt létrehozása.");
        PlayerStats.Money += value;

        // Az effekt pozíciójának beállítása, hogy egy kicsit a levegõben legyen
        Vector3 effectPosition = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);

        // Halál effekt létrehozása a megadott pozíción
        GameObject effect = (GameObject)Instantiate(deathEffect, effectPosition, Quaternion.identity);

        // Az effekt eltávolítása 5 másodperc után
        Destroy(effect, 5f);

        // Az ellenség megsemmisítése
        Destroy(gameObject);
    }

    private void Update()
    {
        if (isAtEnd)
        {
            return; // Ha már elérte az "End"-et, nem kell tovább mozgatni
        }

        // Ha a choiceTarget nem null, és elérte a Choice pontot, döntsön merre menjen tovább
        if (choiceTarget != null && !hasMadeChoice && Vector3.Distance(transform.position, choiceTarget.position) < arrivalThreshold)
        {
            Debug.Log("Ellenség elérte a Choice pontot");
            MakeChoice();  // Döntési logika meghívása
            hasMadeChoice = true;  // Egyszeri döntés
        }


        // Ha már döntött, ellenõrizzük, hogy elérte-e a végsõ célt
        if (hasMadeChoice && endTarget != null && Vector3.Distance(transform.position, endTarget.position) < arrivalThreshold)
        {
            StartCoroutine(HandleArrival()); // Indítsd el a coroutine-t, amikor az ellenség megérkezik
        }
    }

    void MakeChoice()
    {
        int randomChoice = Random.Range(0, 2); // 0 vagy 1 visszatérési érték
        Debug.Log("Véletlenszerû döntés: " + randomChoice);

        if (randomChoice == 0)
        {
            // Menjen a végsõ célpont felé (End)
            agent.SetDestination(endTarget.position);
            Debug.Log("Ellenség az End felé halad.");
        }
        else
        {
            // Menjen az alternatív célpont felé (AltEnd)
            agent.SetDestination(altEndTarget.position);
            Debug.Log("Ellenség az AltEnd felé halad.");
        }
    }


    IEnumerator HandleArrival()
    {
        // Az ellenség elérte az "End"-et, állítsuk le
        isAtEnd = true;
        agent.isStopped = true; // Megállítjuk az ellenséget
        Debug.Log("Ellenség megérkezett az End-hez, 0.75 másodperc múlva robban.");

        // Várjunk 0.75 másodpercet
        yield return new WaitForSeconds(0.75f);

        // Robbanási effekt létrehozása
        Vector3 explosionPosition = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        GameObject explosion = Instantiate(explosionPrefab, explosionPosition, Quaternion.identity);

        // Robbanási effekt megsemmisítése 5 másodperc után
        Destroy(explosion, 5f);

        // Csökkentsük a játékos életerejét
        PlayerStats.Lives--;

        // Az ellenség megsemmisítése a robbanás után
        Destroy(gameObject);
    }
}
