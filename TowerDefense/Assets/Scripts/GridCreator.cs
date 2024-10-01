using UnityEngine;

public class GridCreator : MonoBehaviour
{
    [SerializeField] private int gridWidth = 10;     // Grid szélessége
    [SerializeField] private int gridHeight = 10;    // Grid magassága
    [SerializeField] private float cellSize = 1f;    // Cella mérete
    [SerializeField] private GameObject cellPrefab;  // Prefab a cellákhoz
    [SerializeField] private float yOffset = 0.1f;   // Eltolás a padló fölé
    [SerializeField] private float rotationAngle = -45f; // Forgatási szög

    void Start()
    {
        // Alkalmazzuk a forgatást
        transform.rotation = Quaternion.Euler(0, rotationAngle, 0);
        CreateGrid();
    }

    // Grid létrehozása prefabokkal
    void CreateGrid()
    {
        if (cellPrefab == null)
        {
            Debug.LogError("Cell Prefab is not assigned!");
            return;
        }

        // A gridet a GameObject pozíciójához képest hozza létre
        Vector3 offset = transform.position;

        // Forgatás kvaternion
        Quaternion rotation = transform.rotation;

        // A teljes grid celláit létrehozzuk
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Alap helyzet a cellákhoz
                Vector3 cellLocalPosition = new Vector3(x * cellSize, yOffset, y * cellSize);

                // Forgatás a cellák pozíciójára
                Vector3 rotatedPosition = rotation * cellLocalPosition;

                // Absolút pozíció
                Vector3 cellPosition = offset + rotatedPosition;

                // Prefab létrehozása az adott pozícióban, forgatva a grid rotációjával
                GameObject cellInstance = Instantiate(cellPrefab, cellPosition, rotation);

                // A cellInstance szülõje legyen a GridCreator GameObject
                cellInstance.transform.SetParent(transform);
            }
        }
    }
}
