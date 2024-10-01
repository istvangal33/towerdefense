using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TowerBasedPathDecision : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform startPoint; // Kezd�pont
    public Transform endPoint;   // V�gpont (End)
    public Transform[] branchPoints; // El�gaz�si pontok
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
                Debug.Log("V�gpont el�rve!");
                return;
            }

            if (currentTarget == endPoint)
            {
                isPathComplete = true;
                return;
            }

            // Tornyok alapj�n legjobb el�gaz�s kiv�laszt�sa
            Transform bestBranch = EvaluateBestBranch();
            if (bestBranch != null)
            {
                agent.SetDestination(bestBranch.position);
                currentTarget = bestBranch;
                Debug.DrawRay(bestBranch.position, Vector3.up, Color.red, 1.0f); // Debug c�lpont
            }
            else
            {
                // Ha nincsenek el�gaz�sok, �ll�tsuk be a v�gpontot
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

        // Ha az el�gaz�s pontok k�z�l egyik sem t�nik optim�lisnak, maradjunk a jelenlegi c�lpontn�l
        if (bestBranch == null || bestBranch == currentTarget)
        {
            return null;
        }

        return bestBranch;
    }

    float EvaluateBranch(Transform branch)
    {
        // Sz�m�tsd ki az el�gaz�s "pontsz�m�t" a tornyok alapj�n
        float score = 0f;

        // Hozz l�tre egy k�r alak� z�n�t az el�gaz�s k�r�l
        Collider[] colliders = Physics.OverlapSphere(branch.position, 10f); // 10f a sug�r

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Tower")) // Gy�z�dj meg r�la, hogy a tornyok megfelel�en vannak c�mk�zve
            {
                // N�velj�k a pontsz�mot, ha torony tal�lhat� az el�gaz�s k�zel�ben
                score += 1f;
            }
        }

        return score;
    }
}
