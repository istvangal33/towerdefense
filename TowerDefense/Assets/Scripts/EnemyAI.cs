using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public int health = 100;
    public int value = 1;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float heightOffset = 0f; // Magasság állítása
    [SerializeField] private float arrivalThreshold = 0.5f; // Ekkora távolság esetén tekintjük célba értnek
    private Transform target;

    public GameObject deathEffect;

    private void Start()
    {
        // Keressük meg az "End" nevû GameObjectet a pályán
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
        
        // Megsemmisítjük az ellenséget, ha elfogy az életereje
        Destroy(gameObject);
    }

    private void Update()
    {
        if (target != null)
        {
            // Állítsuk be a célpont pozícióját, hozzáadva a magasságot (Y tengelyen)
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y + heightOffset, target.position.z);
            agent.SetDestination(targetPosition);

            // Ellenõrizzük, hogy az ellenség megérkezett-e a célponthoz
            if (Vector3.Distance(transform.position, targetPosition) < arrivalThreshold)
            {
                // Csökkentjük a játékos életerejét
                PlayerStats.Lives--;

                // Megsemmisítjük az ellenséget
                Destroy(gameObject);
            }
        }
    }
}
