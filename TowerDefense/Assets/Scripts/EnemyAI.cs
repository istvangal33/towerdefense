using UnityEngine;
using UnityEngine.AI;
using System.Collections;


public class EnemyAI : MonoBehaviour
{
    public float startHealth = 100;
    private float health;
    public int value = 1;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float arrivalThreshold = 0.5f;

    public GameObject deathEffect;
    public GameObject explosionPrefab;
    private bool isAtEnd = false;
    private bool hasMadeChoice = false;

    [Header("Unity Stuff")]
    public UnityEngine.UI.Image healthBar;

    [Header("Waypoints")]
    private Transform[] choices;  // Array for path choices (e.g., Choice1 and Choice2)
    private Transform endTarget;  // Final target (End)

  

    private void Start()
    {
        // Find path choices and the final target
        Transform choice1 = GameObject.Find("Choice1").transform;
        Transform choice2 = GameObject.Find("Choice2").transform;
        endTarget = GameObject.Find("End").transform;

        // Fill the choices array
        choices = new Transform[] { choice1, choice2 };

        // Choose a path based on danger
        ChoosePath();

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
        PlayerStats.Money += value;
        GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(effect, 5f);
        Destroy(gameObject);
    }

    private void Update()
    {
        if (isAtEnd)
        {
            return;
        }

        // If the enemy reaches the path choice, they make a decision
        if (!hasMadeChoice && Vector3.Distance(transform.position, agent.destination) < arrivalThreshold)
        {
            agent.SetDestination(endTarget.position);
            hasMadeChoice = true;
        }

        // If the enemy reaches the end
        if (hasMadeChoice && Vector3.Distance(transform.position, endTarget.position) < arrivalThreshold)
        {
            StartCoroutine(HandleArrival());
        }
    }

    IEnumerator HandleArrival()
    {
        isAtEnd = true;
        agent.isStopped = true;
        yield return new WaitForSeconds(0.75f);

        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(explosion, 5f);

        PlayerStats.Lives--;
        Destroy(gameObject);
    }

    public float CalculateDanger(Transform path)
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        float dangerScore = 0f;

        foreach (GameObject tower in towers)
        {
            float distanceToPath = Vector3.Distance(tower.transform.position, path.position);

            if (distanceToPath < 10f) // Példa küszöbérték a torony hatására
            {
                Tower towerScript = tower.GetComponent<Tower>();
                dangerScore += 1 / distanceToPath;  // Minél közelebb van, annál nagyobb a veszély

                switch (towerScript.towerType)
                {
                    case TowerType.MachineGun:
                        dangerScore += 10f;
                        break;
                    case TowerType.Rocket:
                        dangerScore += 20f;
                        break;
                    case TowerType.Laser:
                        dangerScore += 15f;
                        break;
                }
            }
        }

        return dangerScore;
    }


    // Make a path choice based on danger level
    void ChoosePath()
    {
        Transform choice1 = GameObject.Find("Choice1").transform;
        Transform choice2 = GameObject.Find("Choice2").transform;

        float danger1 = CalculateDanger(choice1);
        float danger2 = CalculateDanger(choice2);

        if (danger1 < danger2)
        {
            agent.SetDestination(choice1.position);
        }
        else
        {
            agent.SetDestination(choice2.position);
        }
    }
}
