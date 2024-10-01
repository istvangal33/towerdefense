using UnityEngine;

public class RotationScript : MonoBehaviour
{
    [SerializeField] private float rotationAngle = 45f; // A forgat�s sz�ge
    [SerializeField] private Vector3 rotationAxis = Vector3.up; // Forgat�si tengely, alap�rtelmezetten Y tengely

    // Elind�t�skor v�grehajtott forgat�s
    void Start()
    {
        // Forgat�s a megadott sz�gben �s tengely k�r�l
        RotateObject(rotationAngle, rotationAxis);
    }

    // Met�dus, amely elv�gzi a forgat�st
    public void RotateObject(float angle, Vector3 axis)
    {
        // A GameObject forgat�sa a megadott tengely �s sz�g szerint
        transform.rotation = Quaternion.AngleAxis(angle, axis);
    }
}
