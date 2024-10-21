using UnityEngine;

public class RotationScript : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 5f; 

    
    public void RotateObjectTowards(Vector3 direction)
    {
        
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }
}
