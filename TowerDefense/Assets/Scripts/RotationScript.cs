using UnityEngine;

public class RotationScript : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 5f; // Forgat�si sebess�g

    // Forgat�s egy adott ir�nyba (Vector3 direction)
    public void RotateObjectTowards(Vector3 direction)
    {
        // Forgat�s a megadott ir�nyba (csak az Y tengelyen)
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }
}
