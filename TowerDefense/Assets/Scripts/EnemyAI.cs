using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float heightOffset = 0f; // Magass�g �ll�t�sa
    private Transform target;

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

    private void Update()
    {
        if (target != null)
        {
            // �ll�tsuk be a c�lpont poz�ci�j�t, hozz�adva a magass�got (Y tengelyen)
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y + heightOffset, target.position.z);
            agent.SetDestination(targetPosition);
        }
    }
}
