using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public enum EnemyType { Buggy, Helicopter, Hovertank }

public class EnemyAI : MonoBehaviour
{
    public EnemyType enemyType;
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
    private Transform[] choices;  
    private Transform endTarget;  

    public bool isSlowed = false;
    public static event System.Action<EnemyAI> OnEnemySpawned;
    public static event System.Action<EnemyAI> OnEnemyDestroyed;

    private void Start()
    {
        OnEnemySpawned?.Invoke(this);

        Transform choice1 = GameObject.Find("Choice1").transform;
        Transform choice2 = GameObject.Find("Choice2").transform;
        endTarget = GameObject.Find("End").transform;

        choices = new Transform[] { choice1, choice2 };

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
        OnEnemyDestroyed?.Invoke(this);

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

        if (!hasMadeChoice && Vector3.Distance(transform.position, agent.destination) < arrivalThreshold)
        {
            agent.SetDestination(endTarget.position);
            hasMadeChoice = true;
        }

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
        OnEnemyDestroyed?.Invoke(this);
        Destroy(gameObject);
    }


    private float CalculateDanger(Transform path)
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        float dangerScore = 0f;

        foreach (GameObject tower in towers)
        {
            float distanceToPath = Vector3.Distance(tower.transform.position, path.position);

            if (distanceToPath < 10f)
            {
                Tower towerScript = tower.GetComponent<Tower>();
                dangerScore += 1 / distanceToPath;

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

    public void Slow(float slowAmount)
    {
        if (!isSlowed)
        {
            agent.speed *= (1f - slowAmount);
            isSlowed = true;
            StartCoroutine(ResetSpeed(slowAmount));
        }
    }

    IEnumerator ResetSpeed(float slowAmount)
    {
        yield return new WaitForSeconds(2f);
        agent.speed /= (1f - slowAmount);
        isSlowed = false;
    }
    public void SetHealth(float multiplier)
    {
        startHealth *= multiplier;
        health = startHealth; 
        if (healthBar != null)
        {
            healthBar.fillAmount = health / startHealth;
        }
    }


}
