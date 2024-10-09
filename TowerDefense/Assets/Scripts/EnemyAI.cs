using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public int health = 100;
    public int value = 1;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float heightOffset = 0f; // Magass�g �ll�t�sa
    [SerializeField] private float arrivalThreshold = 0.5f; // Ekkora t�vols�g eset�n tekintj�k c�lba �rtnek
    private Transform target;

    public GameObject deathEffect;

    private void Start()
    {
        // Keress�k meg az "End" nev� GameObjectet a p�ly�n
        GameObject endObject = GameObject.Find("End");

        if (endObject != null)
        {
            target = endObject.transform;
        }
        else
        {
            Debug.LogError("No GameObject named 'End' found in the scene.");
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        PlayerStats.Money += value;
        GameObject effect = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(effect, 5f);
        
        // Megsemmis�tj�k az ellens�get, ha elfogy az �letereje
        Destroy(gameObject);
    }

    private void Update()
    {
        if (target != null)
        {
            // �ll�tsuk be a c�lpont poz�ci�j�t, hozz�adva a magass�got (Y tengelyen)
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y + heightOffset, target.position.z);
            agent.SetDestination(targetPosition);

            // Ellen�rizz�k, hogy az ellens�g meg�rkezett-e a c�lponthoz
            if (Vector3.Distance(transform.position, targetPosition) < arrivalThreshold)
            {
                // Cs�kkentj�k a j�t�kos �leterej�t
                PlayerStats.Lives--;

                // Megsemmis�tj�k az ellens�get
                Destroy(gameObject);
            }
        }
    }
}
