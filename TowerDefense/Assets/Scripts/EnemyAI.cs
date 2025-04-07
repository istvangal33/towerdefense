using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public enum EnemyType { Buggy, Helicopter, Hovertank }

public class EnemyAI : MonoBehaviour
{

    public NavMeshAgent agent;
    public EnemyType enemyType;
    public float startHealth = 100;
    private float health;
    public int value = 1;
    
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

    private int waveNumber;

    private void Start()
    {
        // Inicializáld az agentet
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        OnEnemySpawned?.Invoke(this);

        GameObject c1 = GameObject.Find("Choice1");
        GameObject c2 = GameObject.Find("Choice2");
        GameObject end = GameObject.Find("End");

        if (c1 == null || c2 == null || end == null)
        {
            Debug.LogWarning("One or more path targets not found! Skipping pathfinding.");
            return;
        }

        Transform choice1 = c1.transform;
        Transform choice2 = c2.transform;
        endTarget = end.transform;

        choices = new Transform[] { choice1, choice2 };

        ChoosePath();
        health = startHealth;
    }




    public void SetWaveNumber(int waveNumber)
    {
        this.waveNumber = waveNumber;
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

    public void SetSpeed(float speedMultiplier)
    {
        if (waveNumber == 1)
        {
            speedMultiplier = 0.8f;
        }

        
        float baseSpeed = GetBaseSpeed();
        agent.speed = baseSpeed * speedMultiplier;

        Debug.Log($"[EnemyAI] {enemyType} Base Speed: {baseSpeed}, Multiplier: {speedMultiplier}, Final Speed: {agent.speed}");
    }

    private float GetBaseSpeed()
    {
        switch (enemyType)
        {
            case EnemyType.Buggy: return 3.5f;
            case EnemyType.Helicopter: return 2.5f;
            case EnemyType.Hovertank: return 2.0f;
            default: return 3.0f;
        }
    }



    private static Dictionary<EnemyType, float> baseHealthValues = new Dictionary<EnemyType, float>()
    {
        { EnemyType.Buggy, 200f },
        { EnemyType.Helicopter, 250f },
        { EnemyType.Hovertank, 650f }
    };

    public void SetHealth(float waveMultiplier, float aiMultiplier)
    {
        if (waveNumber == 1)
        {
            aiMultiplier = 0.8f;
        }

        
        startHealth = baseHealthValues[enemyType] * aiMultiplier;
        health = startHealth;

        if (healthBar != null)
        {
            healthBar.fillAmount = 1.0f;
        }

        Debug.Log($"[EnemyAI] {enemyType} Base HP: {baseHealthValues[enemyType]}, AI Multiplier: {aiMultiplier}, Final HP: {health}");
    }

}