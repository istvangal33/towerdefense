using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TowerBasedPathDecision : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform startPoint; // Kezdõpont
    public Transform endPoint;   // Végpont (End)
    public Transform[] branchPoints; // Elágazási pontok
    public GameObject towerPrefab; // A torony prefab

    private Transform currentTarget;
    private bool isPathComplete = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(startPoint.position);
        currentTarget = startPoint;
    }

    void Update()
    {
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            if (isPathComplete)
            {
                Debug.Log("Végpont elérve!");
                return;
            }

            if (currentTarget == endPoint)
            {
                isPathComplete = true;
                return;
            }

            // Tornyok alapján legjobb elágazás kiválasztása
            Transform bestBranch = EvaluateBestBranch();
            if (bestBranch != null)
            {
                agent.SetDestination(bestBranch.position);
                currentTarget = bestBranch;
                Debug.DrawRay(bestBranch.position, Vector3.up, Color.red, 1.0f); // Debug célpont
            }
            else
            {
                // Ha nincsenek elágazások, állítsuk be a végpontot
                agent.SetDestination(endPoint.position);
                currentTarget = endPoint;
            }
        }
    }

    Transform EvaluateBestBranch()
    {
        Transform bestBranch = null;
        float bestScore = float.MaxValue;

        foreach (var branch in branchPoints)
        {
            float score = EvaluateBranch(branch);
            if (score < bestScore)
            {
                bestScore = score;
                bestBranch = branch;
            }
        }

        // Ha az elágazás pontok közül egyik sem tûnik optimálisnak, maradjunk a jelenlegi célpontnál
        if (bestBranch == null || bestBranch == currentTarget)
        {
            return null;
        }

        return bestBranch;
    }

    float EvaluateBranch(Transform branch)
    {
        // Számítsd ki az elágazás "pontszámát" a tornyok alapján
        float score = 0f;

        // Hozz létre egy kör alakú zónát az elágazás körül
        Collider[] colliders = Physics.OverlapSphere(branch.position, 10f); // 10f a sugár

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Tower")) // Gyõzõdj meg róla, hogy a tornyok megfelelõen vannak címkézve
            {
                // Növeljük a pontszámot, ha torony található az elágazás közelében
                score += 1f;
            }
        }

        return score;
    }
}
