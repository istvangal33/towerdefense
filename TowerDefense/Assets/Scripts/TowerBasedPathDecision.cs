using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class TowerBasedPathDecision : MonoBehaviour
{
    public NavMeshAgent agent;        // Az ellenség NavMeshAgent-je
    public Transform endPoint;        // Célpont, ahová az ellenség eljut
    private bool hasReachedDestination = false;  // Boolean flag
    public float navMeshSearchRadius = 10.0f;    // Further increased radius to search for NavMesh points

    // Fallback position if no valid NavMesh point is found for the endpoint
    public Vector3 fallbackEndPointPosition = new Vector3(0, 0, 0);  // You can set this manually to a known valid location

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent nincs hozzárendelve az ellenséghez!");
            return;
        }

        if (endPoint == null)
        {
            Debug.LogError("EndPoint nincs hozzárendelve!");
            return;
        }

        // Snap agent to the nearest NavMesh if not already on it
        if (!IsOnNavMesh(agent.transform.position))
        {
            Vector3 closestAgentPos = GetNearestPointOnNavMesh(agent.transform.position);
            if (closestAgentPos != Vector3.zero)  // Only warp if a valid point is found
            {
                agent.Warp(closestAgentPos);  // Snap the agent to the nearest NavMesh point
                Debug.Log($"Agent snapped to NavMesh: {closestAgentPos}");
            }
            else
            {
                Debug.LogError("Nem található érvényes NavMesh pont az agenthez!");
                return;
            }
        }

        // Snap endPoint to the nearest NavMesh if not already on it
        if (!IsOnNavMesh(endPoint.position))
        {
            Vector3 closestEndPoint = GetNearestPointOnNavMesh(endPoint.position);
            if (closestEndPoint != Vector3.zero)  // Only move if a valid point is found
            {
                endPoint.position = closestEndPoint;  // Snap the endPoint to the nearest NavMesh point
                Debug.Log($"EndPoint snapped to NavMesh: {closestEndPoint}");
            }
            else
            {
                Debug.LogError("Nem található érvényes NavMesh pont a célponthoz! Using fallback.");
                endPoint.position = fallbackEndPointPosition;  // Use fallback if no valid point is found
            }
        }

        // Állítsuk be az agent stoppingDistance-ét kisebb értékre
        agent.stoppingDistance = 0.1f; // Kisebb távolság a megálláshoz

        StartCoroutine(SetDestinationAfterDelay(0.1f));
    }

    IEnumerator SetDestinationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(endPoint.position, path);

        if (path.status == NavMeshPathStatus.PathComplete)
        {
            Debug.Log("Valid path found. Setting destination.");
            agent.SetDestination(endPoint.position);
        }
        else
        {
            Debug.LogError("No valid path could be found!");
        }
    }

    void Update()
    {
        if (agent.isStopped)
        {
            Debug.Log("NavMeshAgent is stopped.");
        }

        if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            Debug.LogError("Az ellenség útvonala érvénytelen!");
            return;
        }

        if (!hasReachedDestination && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            Debug.Log($"Ellenség távolsága a végponttól: {agent.remainingDistance}");
            hasReachedDestination = true;
            StartCoroutine(DestroyAfterDelay(2f));
        }
    }

    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        OnReachDestination();
    }

    void OnReachDestination()
    {
        Debug.Log("Ellenség elérte a végpontot!");
        Destroy(gameObject); // Ellenség eltávolítása
    }

    // Check if the position is on the NavMesh
    bool IsOnNavMesh(Vector3 position)
    {
        NavMeshHit hit;
        return NavMesh.SamplePosition(position, out hit, navMeshSearchRadius, NavMesh.AllAreas);
    }

    // Get the nearest valid position on the NavMesh
    Vector3 GetNearestPointOnNavMesh(Vector3 position)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(position, out hit, navMeshSearchRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return Vector3.zero; // Return Vector3.zero if no valid point is found
    }
}
