using UnityEngine;

public class RotationScript : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 5f; // Forgatási sebesség

    // Forgatás egy adott irányba (Vector3 direction)
    public void RotateObjectTowards(Vector3 direction)
    {
        // Forgatás a megadott irányba (csak az Y tengelyen)
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }
}
