using UnityEngine;

public class GridCreator : MonoBehaviour
{
    [SerializeField] private int gridWidth = 10;     // Grid sz�less�ge
    [SerializeField] private int gridHeight = 10;    // Grid magass�ga
    [SerializeField] private float cellSize = 1f;    // Cella m�rete
    [SerializeField] private GameObject cellPrefab;  // Prefab a cell�khoz
    [SerializeField] private float yOffset = 0.1f;   // Eltol�s a padl� f�l�
    [SerializeField] private float rotationAngle = -45f; // Forgat�si sz�g

    void Start()
    {
        // Alkalmazzuk a forgat�st
        transform.rotation = Quaternion.Euler(0, rotationAngle, 0);
        CreateGrid();
    }

    // Grid l�trehoz�sa prefabokkal
    void CreateGrid()
    {
        if (cellPrefab == null)
        {
            Debug.LogError("Cell Prefab is not assigned!");
            return;
        }

        // A gridet a GameObject poz�ci�j�hoz k�pest hozza l�tre
        Vector3 offset = transform.position;

        // Forgat�s kvaternion
        Quaternion rotation = transform.rotation;

        // A teljes grid cell�it l�trehozzuk
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Alap helyzet a cell�khoz
                Vector3 cellLocalPosition = new Vector3(x * cellSize, yOffset, y * cellSize);

                // Forgat�s a cell�k poz�ci�j�ra
                Vector3 rotatedPosition = rotation * cellLocalPosition;

                // Absol�t poz�ci�
                Vector3 cellPosition = offset + rotatedPosition;

                // Prefab l�trehoz�sa az adott poz�ci�ban, forgatva a grid rot�ci�j�val
                GameObject cellInstance = Instantiate(cellPrefab, cellPosition, rotation);

                // A cellInstance sz�l�je legyen a GridCreator GameObject
                cellInstance.transform.SetParent(transform);
            }
        }
    }
}
