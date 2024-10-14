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
    [SerializeField] private float heightOffset = 0f; // Magass�g �ll�t�sa
    [SerializeField] private float arrivalThreshold = 0.5f; // Ekkora t�vols�g eset�n tekintj�k c�lba �rtnek
    private Transform choiceTarget;  // El�gaz�si pont c�lja
    private Transform endTarget;     // V�gs� c�lpont
    private Transform altEndTarget;  // Alternat�v v�gs� c�lpont

    public GameObject deathEffect;
    public GameObject explosionPrefab; // A robban�s prefab
    private bool isAtEnd = false; // Jelzi, hogy az ellens�g el�rte az "End" objektumot
    private bool hasMadeChoice = false; // Jelzi, hogy az el�gaz�sn�l m�r d�nt�tt

    [Header("Unity Staff")]
    public Image healthBar;

    [SerializeField] private float movementSpeed = 3.5f; // A sebess�g be�ll�t�sa
    [SerializeField] private float acceleration = 8f;    // Gyorsul�s be�ll�t�sa
    [SerializeField] private float angularSpeed = 120f;  // Forg�si sebess�g be�ll�t�sa
    [SerializeField] private float stoppingDistance = 0.2f; // Meg�ll�si t�vols�g

    private void Start()
    {
        // Keress�k meg a "Choice", "End" �s "AltEnd" nev� GameObjecteket a p�ly�n
        GameObject choiceObject = GameObject.Find("Choice");
        GameObject endObject = GameObject.Find("End");
        GameObject altEndObject = GameObject.Find("AltEnd");

        if (choiceObject != null)
        {
            choiceTarget = choiceObject.transform;
            Debug.Log("Choice megtal�lva: " + choiceTarget.position);
        }
        else
        {
            Debug.LogError("No GameObject named 'Choice' found in the scene.");
        }

        if (endObject != null)
        {
            endTarget = endObject.transform;
            Debug.Log("End megtal�lva: " + endTarget.position);
        }
        else
        {
            Debug.LogError("No GameObject named 'End' found in the scene.");
        }

        if (altEndObject != null)
        {
            altEndTarget = altEndObject.transform;
            Debug.Log("AltEnd megtal�lva: " + altEndTarget.position);
        }
        else
        {
            Debug.LogError("No GameObject named 'AltEnd' found in the scene.");
        }

        // Kezdj�k az �tvonalat a Choice fel�
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
        Debug.Log("Ellens�g meghalt, effekt l�trehoz�sa.");
        PlayerStats.Money += value;

        // Az effekt poz�ci�j�nak be�ll�t�sa, hogy egy kicsit a leveg�ben legyen
        Vector3 effectPosition = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);

        // Hal�l effekt l�trehoz�sa a megadott poz�ci�n
        GameObject effect = (GameObject)Instantiate(deathEffect, effectPosition, Quaternion.identity);

        // Az effekt elt�vol�t�sa 5 m�sodperc ut�n
        Destroy(effect, 5f);

        // Az ellens�g megsemmis�t�se
        Destroy(gameObject);
    }

    private void Update()
    {
        if (isAtEnd)
        {
            return; // Ha m�r el�rte az "End"-et, nem kell tov�bb mozgatni
        }

        // Ha a choiceTarget nem null, �s el�rte a Choice pontot, d�nts�n merre menjen tov�bb
        if (choiceTarget != null && !hasMadeChoice && Vector3.Distance(transform.position, choiceTarget.position) < arrivalThreshold)
        {
            Debug.Log("Ellens�g el�rte a Choice pontot");
            MakeChoice();  // D�nt�si logika megh�v�sa
            hasMadeChoice = true;  // Egyszeri d�nt�s
        }


        // Ha m�r d�nt�tt, ellen�rizz�k, hogy el�rte-e a v�gs� c�lt
        if (hasMadeChoice && endTarget != null && Vector3.Distance(transform.position, endTarget.position) < arrivalThreshold)
        {
            StartCoroutine(HandleArrival()); // Ind�tsd el a coroutine-t, amikor az ellens�g meg�rkezik
        }
    }

    void MakeChoice()
    {
        int randomChoice = Random.Range(0, 2); // 0 vagy 1 visszat�r�si �rt�k
        Debug.Log("V�letlenszer� d�nt�s: " + randomChoice);

        if (randomChoice == 0)
        {
            // Menjen a v�gs� c�lpont fel� (End)
            agent.SetDestination(endTarget.position);
            Debug.Log("Ellens�g az End fel� halad.");
        }
        else
        {
            // Menjen az alternat�v c�lpont fel� (AltEnd)
            agent.SetDestination(altEndTarget.position);
            Debug.Log("Ellens�g az AltEnd fel� halad.");
        }
    }


    IEnumerator HandleArrival()
    {
        // Az ellens�g el�rte az "End"-et, �ll�tsuk le
        isAtEnd = true;
        agent.isStopped = true; // Meg�ll�tjuk az ellens�get
        Debug.Log("Ellens�g meg�rkezett az End-hez, 0.75 m�sodperc m�lva robban.");

        // V�rjunk 0.75 m�sodpercet
        yield return new WaitForSeconds(0.75f);

        // Robban�si effekt l�trehoz�sa
        Vector3 explosionPosition = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        GameObject explosion = Instantiate(explosionPrefab, explosionPosition, Quaternion.identity);

        // Robban�si effekt megsemmis�t�se 5 m�sodperc ut�n
        Destroy(explosion, 5f);

        // Cs�kkents�k a j�t�kos �leterej�t
        PlayerStats.Lives--;

        // Az ellens�g megsemmis�t�se a robban�s ut�n
        Destroy(gameObject);
    }
}
