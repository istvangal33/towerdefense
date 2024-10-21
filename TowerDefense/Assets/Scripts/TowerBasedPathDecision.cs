using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class TowerBasedPathDecision : MonoBehaviour
{
    public NavMeshAgent agent;        
    public Transform endPoint;        
    private bool hasReachedDestination = false;  
    public float navMeshSearchRadius = 10.0f;    

    
    public Vector3 fallbackEndPointPosition = new Vector3(0, 0, 0);  

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

        
        if (!IsOnNavMesh(agent.transform.position))
        {
            Vector3 closestAgentPos = GetNearestPointOnNavMesh(agent.transform.position);
            if (closestAgentPos != Vector3.zero)  
            {
                agent.Warp(closestAgentPos);  
                Debug.Log($"Agent snapped to NavMesh: {closestAgentPos}");
            }
            else
            {
                Debug.LogError("Nem található érvényes NavMesh pont az agenthez!");
                return;
            }
        }

        
        if (!IsOnNavMesh(endPoint.position))
        {
            Vector3 closestEndPoint = GetNearestPointOnNavMesh(endPoint.position);
            if (closestEndPoint != Vector3.zero)  
            {
                endPoint.position = closestEndPoint;  
                Debug.Log($"EndPoint snapped to NavMesh: {closestEndPoint}");
            }
            else
            {
                Debug.LogError("Nem található érvényes NavMesh pont a célponthoz! Using fallback.");
                endPoint.position = fallbackEndPointPosition;  
            }
        }

        
        agent.stoppingDistance = 0.1f; 

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
        Destroy(gameObject); 
    }

    
    bool IsOnNavMesh(Vector3 position)
    {
        NavMeshHit hit;
        return NavMesh.SamplePosition(position, out hit, navMeshSearchRadius, NavMesh.AllAreas);
    }

    
    Vector3 GetNearestPointOnNavMesh(Vector3 position)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(position, out hit, navMeshSearchRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return Vector3.zero; 
    }
}
