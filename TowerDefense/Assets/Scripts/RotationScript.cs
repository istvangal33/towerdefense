using UnityEngine;

public class RotationScript : MonoBehaviour
{
    [SerializeField] private float rotationAngle = 45f; // A forgatás szöge
    [SerializeField] private Vector3 rotationAxis = Vector3.up; // Forgatási tengely, alapértelmezetten Y tengely

    // Elindításkor végrehajtott forgatás
    void Start()
    {
        // Forgatás a megadott szögben és tengely körül
        RotateObject(rotationAngle, rotationAxis);
    }

    // Metódus, amely elvégzi a forgatást
    public void RotateObject(float angle, Vector3 axis)
    {
        // A GameObject forgatása a megadott tengely és szög szerint
        transform.rotation = Quaternion.AngleAxis(angle, axis);
    }
}
