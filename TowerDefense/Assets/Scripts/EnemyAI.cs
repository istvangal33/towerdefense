using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float heightOffset = 0f; // Magasság állítása
    private Transform target;

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

    private void Update()
    {
        if (target != null)
        {
            // Állítsuk be a célpont pozícióját, hozzáadva a magasságot (Y tengelyen)
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y + heightOffset, target.position.z);
            agent.SetDestination(targetPosition);
        }
    }
}
